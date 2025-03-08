using Cysharp.Threading.Tasks;
using EditorAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public Action<int> onWaveTimerTick;
    public Action<int> onWaveNumberChange;
    public Action<int> onEnemiesCountChange;
    public Action<EnemyController> onEnemySpawned;

    public async UniTask Init()
    {
        currentWaveNumber = 1;
        currentSpawnedEnemies = new List<EnemyController>();
        allEnemyDatasPool = await Managers.Resource.LoadAssetsByLabel<EnemyData>("enemyData");
        LoadWaveData();
        InGameManagers.Instance.onInitialized += () => StartWaveTimer(5);
    }

    private void LoadWaveData()
    {
        string filePath = Application.streamingAssetsPath + "/wave_data.json";
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            waveData = JsonConvert.DeserializeObject<WaveData>(jsonData);
        }
        else
        {
            Debug.LogError("웨이브 데이터 json파일 찾을 수 없음");
            return;
        }
    }
    
    public async UniTask StartWaveTimer(int time)
    {
        while (time > -1)
        {
            onWaveTimerTick?.Invoke(time);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            time--;
        }

        SpawnNextWave();
        currentWaveNumber++;
        onWaveNumberChange?.Invoke(currentWaveNumber);
        
        // 스폰이 다되길 기다리지않고 바로 다음 웨이브 타이머 시작
        var nextWave = FindWave(currentWaveNumber);
        if (nextWave != null)
            StartWaveTimer(nextWave.nextWaveTime);
    }

    private Wave FindWave(int waveNumber)
    {
        foreach (var wv in waveData.waves)
        {
            if (wv.waveRange[0] <= waveNumber && wv.waveRange[1] >= waveNumber)
                return wv;
        }
        return null;
    }

    private async UniTask SpawnNextWave()
    {
        Wave wave = FindWave(currentWaveNumber);
        for (int i = 0; i < wave.enemies.Length; i++)   // 지금은 없지만 한 웨이브에 여러 종류의 몬스터가 나올 경우를 위한 for문
        {
            var data = allEnemyDatasPool.Find(ed => ed.UnitName == wave.enemies[i].template);
            var template = waveData.enemyTemplates[wave.enemies[i].template];
            for (int j = 0; j < wave.enemies[i].count; j++) // 한 종류씩 count만큼 소환한다
            {
                // 짝수번째는 플레이어측 소환 홀수번째는 ai플레이어측 소환
                await SpawnEnemy(data, j % 2 == 0 ? Define.PlayerType.LocalPlayer : Define.PlayerType.AiPlayer);
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
        onEnemiesCountChange?.Invoke(currentSpawnedEnemies.Count);
        onEnemySpawned?.Invoke(enemy);
    }

    public void OnEnemyDie(EnemyController enemy)
    {
        InGameManagers.CurrencyMgr.CoinAmount += enemy.MyEnemyData.Prize;
        currentSpawnedEnemies.Remove(enemy);
        onEnemiesCountChange?.Invoke(currentSpawnedEnemies.Count);
        Managers.Pool.Push(enemy.GetComponent<Poolable>());
    }
}
