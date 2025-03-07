using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class UnitSpawnManager
{
    [SerializeField]
    private int currentSpawnCost;
    [SerializeField]
    private List<UnitData> allUnitDatasPool;
    
    #region Actions

    public Action<int> onSpawnCostChanged;
    #endregion

    public async UniTask Init()
    {
        allUnitDatasPool = await Managers.Resource.LoadAssetsByLabel<UnitData>("unitData");
        currentSpawnCost = Define.StartSpawnCost;
    }
    
    public async UniTask SpawnRandomUnit(Define.PlayerType playerType)
    {
        if (InGameManagers.CurrencyMgr.CoinAmount < currentSpawnCost)
            return;
        InGameManagers.CurrencyMgr.CoinAmount -= currentSpawnCost;
        
        // TODO: AI측의 뽑기도 구현
        // TODO: 히어로 등급 뽑으면 UI에 축하메세지 출력
        var randomResult = GetRandomUnitDatasPool();
        var unitData = randomResult.gradeUnitDatas[Random.Range(0, randomResult.gradeUnitDatas.Count)];
        var unit = (await Managers.Pool.PopAsync(unitData.UnitPrefab)).GetComponent<UnitController>();
        unit.Init(unitData);
        InGameManagers.FieldMgr.playerGrid.OnNewUnitspawned(unit);
        currentSpawnCost += Define.SpawnCostIncrease;
        onSpawnCostChanged?.Invoke(currentSpawnCost);
    }
    
    // 소환확률에 따라 랜덤으로 등급을 뽑고 그 등급과 등급의 유닛데이터들을 리턴
    private (List<UnitData> gradeUnitDatas, Define.UnitGrade grade) GetRandomUnitDatasPool()
    {
        // TODO: 소환확률 강화 반영
        var randomValue = Random.value * 100f;
        List<UnitData> gradeUnitDatas;
        Define.UnitGrade grade;
        if (randomValue <= Define.DefaultSpawnRateDict[Define.UnitGrade.Hero])
        {
            gradeUnitDatas = allUnitDatasPool.Where(u => u.Grade == Define.UnitGrade.Hero).ToList();
            grade = Define.UnitGrade.Hero;
        }
        else if (randomValue <= Define.DefaultSpawnRateDict[Define.UnitGrade.Rare])
        {
            gradeUnitDatas = allUnitDatasPool.Where(u => u.Grade == Define.UnitGrade.Rare).ToList();
            grade = Define.UnitGrade.Rare;
        }
        else
        {
            gradeUnitDatas = allUnitDatasPool.Where(u => u.Grade == Define.UnitGrade.Normal).ToList();
            grade = Define.UnitGrade.Normal;
        }        
        return (gradeUnitDatas, grade);
    }
}
