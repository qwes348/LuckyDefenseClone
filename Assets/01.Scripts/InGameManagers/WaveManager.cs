using Cysharp.Threading.Tasks;
using EditorAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class WaveManager
{
    [Header("런타임에 채워지는 데이터들")]
    [SerializeField]
    private int currentWaveNumber;
    [SerializeField]
    private List<EnemyData> allEnemyDatasPool;
    [SerializeField]
    private WaveData waveData;
    [SerializeField, ReadOnly]
    private List<EnemyController> currentSpawnedEnemies;

    private List<EnemyController> currentSpawnedBosses;

    public Action<int> onWaveTimerTick;
    public Action<int> onWaveNumberChange;
    public Action<int> onEnemiesCountChange;
    public Action<EnemyController> onEnemySpawned;
    public Action onBossWaveStart;
    public Action onBossAllDied;
    
    public int CurrentWaveNumber => currentWaveNumber;

    public async UniTask Init()
    {
        currentWaveNumber = 1;
        currentSpawnedEnemies = new List<EnemyController>();
        currentSpawnedBosses = new List<EnemyController>();
        allEnemyDatasPool = await Managers.Resource.LoadAssetsByLabel<EnemyData>("enemyData");
        LoadWaveData();
        // InGameManagers.Instance.onInitialized += () => StartWaveTimer(5);
    }

    public void OnGameStart()
    {
        StartWaveTimer(5);
    }

    private void LoadWaveData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "wave_data.json");
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            waveData = JsonConvert.DeserializeObject<WaveData>(jsonData);
        }
        else
        {
            Debug.LogError("웨이브 데이터 json을 찾을 수 없음");
            return;
        }
    }
    
    public async UniTask StartWaveTimer(int time)
    {
        if(Managers.Game.GameState == Define.GameState.GameOver)
            return;
        
        while (time > -1)
        {
            onWaveTimerTick?.Invoke(time);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            time--;
        }
        
        SpawnNextWave();
        // 스폰이 다되길 기다리지않고 바로 다음 웨이브 타이머 시작
        StartWaveTimer(FindWave(currentWaveNumber).nextWaveTime);
        currentWaveNumber++;
        onWaveNumberChange?.Invoke(currentWaveNumber);
    }

    private Wave FindWave(int waveNumber)
    {
        // 사정 정의된 wave를 넘어섰다면 제일 마지막 wave를 리턴
        // 마지막 wave데이터는 반복 웨이브로 구현됨
        if(waveNumber >= waveData.waves.Length)
            return waveData.waves[^1];
        
        foreach (var wv in waveData.waves)
        {
            if (wv.waveRange[0] <= waveNumber && wv.waveRange[1] >= waveNumber)
                return wv;
        }
        
        return null;
    }

    private async UniTask SpawnNextWave()
    {
        if (currentSpawnedBosses.Count > 0)
        {
            Managers.Game.GameState = Define.GameState.GameOver;
            return;
        }
        
        Wave wave = FindWave(currentWaveNumber);
        bool isStartEventFired = false;
        for (int i = 0; i < wave.enemies.Length; i++)   // 지금은 없지만 한 웨이브에 여러 종류의 몬스터가 나올 경우를 위한 for문
        {
            var template = waveData.enemyTemplates[wave.enemies[i].template];
            var data = allEnemyDatasPool.Find(ed => ed.UnitId == template.unitId);
            if (data.UnitId.Equals(9999))   // 반복 웨이브
            {
                // 반복마다 체력을 200씩 증가시킴
                data.SetMaxHealth(800 + (currentWaveNumber - wave.waveRange[0]) * 200);
            }
            if(!isStartEventFired)
            {
                if (data.Grade == Define.EnemyGrade.Boss)
                {
                    onBossWaveStart?.Invoke();
                    InGameUiManager.Instance.WaveNotice.ShowBossWave(currentWaveNumber, data, wave.nextWaveTime);
                }
                else
                {
                    InGameUiManager.Instance.WaveNotice.ShowNormal(currentWaveNumber);
                }
                isStartEventFired = true;
            }
            for (int j = 0; j < wave.enemies[i].count; j++) // 한 종류씩 count만큼 소환한다
            {
                // 짝수번째는 플레이어측 소환 홀수번째는 ai플레이어측 소환
                await SpawnEnemy(data, j % 2 == 0 ? Define.PlayerType.LocalPlayer : Define.PlayerType.AiPlayer);
                if (Managers.Game.GameState == Define.GameState.GameOver)
                    return;
                await UniTask.Delay(TimeSpan.FromSeconds(template.spawnInterval));
            }            
        }
    }

    private async UniTask SpawnEnemy(EnemyData data, Define.PlayerType playerType)
    {
        var enemy = (await Managers.Pool.PopAsync(data.EnemyPrefab)).GetComponent<EnemyController>();
        enemy.transform.position =
            playerType == Define.PlayerType.LocalPlayer ?
                InGameManagers.FieldMgr.enemyWaypointsContainer.PlayerSideWaypoints[0] : InGameManagers.FieldMgr.enemyWaypointsContainer.OpponentSideWaypoints[0];
        enemy.Init(data, playerType);
        enemy.gameObject.SetActive(true);
        currentSpawnedEnemies.Add(enemy);
        if(enemy.MyEnemyData.Grade == Define.EnemyGrade.Boss)
            currentSpawnedBosses.Add(enemy);
        onEnemiesCountChange?.Invoke(currentSpawnedEnemies.Count);
        onEnemySpawned?.Invoke(enemy);

        if (currentSpawnedEnemies.Count > Define.MaxEnemyCount)
        {
            Managers.Game.GameState = Define.GameState.GameOver;
        }
    }

    public void OnEnemyDie(EnemyController enemy)
    {
        InGameManagers.CurrencyMgr.AddCurrency(enemy.MyEnemyData.Prize, enemy.MyEnemyData.PrizeType, Define.PlayerType.LocalPlayer);
        InGameManagers.CurrencyMgr.AddCurrency(enemy.MyEnemyData.Prize, enemy.MyEnemyData.PrizeType, Define.PlayerType.AiPlayer);
        currentSpawnedEnemies.Remove(enemy);
        onEnemiesCountChange?.Invoke(currentSpawnedEnemies.Count);
        if (enemy.MyEnemyData.Grade == Define.EnemyGrade.Boss)
        {
            currentSpawnedBosses.Remove(enemy);
            if(currentSpawnedBosses.Count == 0)
                onBossAllDied?.Invoke();
        }
        Managers.Pool.Push(enemy.GetComponent<Poolable>());
    }
}
