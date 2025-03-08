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

    private Dictionary<Define.PlayerType, int> currentUnitsCountDict;
    
    #region Actions

    public Action<int> onSpawnCostChanged;
    public Action<int> onLocalPlayerUnitCountChanged;
    #endregion
    
    #region Properties
    public int CurrentSpawnCost => currentSpawnCost;
    #endregion

    public async UniTask Init()
    {
        allUnitDatasPool = await Managers.Resource.LoadAssetsByLabel<UnitData>("unitData");
        currentSpawnCost = Define.StartSpawnCost;
        currentUnitsCountDict = new Dictionary<Define.PlayerType, int>()
        {
            {Define.PlayerType.LocalPlayer, 0},
            {Define.PlayerType.AiPlayer, 0}
        };
    }
    
    public async UniTask SpawnRandomUnit(Define.PlayerType playerType)
    {
        if (InGameManagers.CurrencyMgr.CoinAmount < currentSpawnCost)
            return;
        InGameManagers.CurrencyMgr.CoinAmount -= currentSpawnCost;
        currentUnitsCountDict[playerType]++;
        if(playerType == Define.PlayerType.LocalPlayer)
            onLocalPlayerUnitCountChanged?.Invoke(currentUnitsCountDict[playerType]);
        
        // TODO: AI측의 뽑기도 구현
        // TODO: 히어로 등급 뽑으면 UI에 축하메세지 출력
        var randomResult = GetRandomUnitDatasPool(playerType);
        var unitData = randomResult.gradeUnitDatas[Random.Range(0, randomResult.gradeUnitDatas.Count)];
        var unit = (await Managers.Pool.PopAsync(unitData.UnitPrefab)).GetComponent<UnitController>();
        unit.Init(unitData);
        InGameManagers.FieldMgr.playerGrid.OnNewUnitspawned(unit);
        currentSpawnCost += Define.SpawnCostIncrease;
        onSpawnCostChanged?.Invoke(currentSpawnCost);
    }

    public async UniTask SpawnGradeUnit(Define.UnitGrade unitGrade, Define.PlayerType playerType)
    {
        currentUnitsCountDict[playerType]++;
        if(playerType == Define.PlayerType.LocalPlayer)
            onLocalPlayerUnitCountChanged?.Invoke(currentUnitsCountDict[playerType]);
        
        var randomResult = allUnitDatasPool.Where(u => u.Grade == unitGrade).ToList();
        var unitData = randomResult[Random.Range(0, randomResult.Count)];
        var unit = (await Managers.Pool.PopAsync(unitData.UnitPrefab)).GetComponent<UnitController>();
        unit.Init(unitData);
        InGameManagers.FieldMgr.playerGrid.OnNewUnitspawned(unit);
    }
    
    // 소환확률에 따라 랜덤으로 등급을 뽑고 그 등급과 등급의 유닛데이터들을 리턴
    private (List<UnitData> gradeUnitDatas, Define.UnitGrade grade) GetRandomUnitDatasPool(Define.PlayerType playerType)
    {
        var randomValue = Random.value;
        List<UnitData> gradeUnitDatas;
        Define.UnitGrade grade;
        
        // 확률 강화가 반영된 확률값들을 변수에 담음
        int upgradeLevel = InGameManagers.UpgradeMgr.GetUpgradeLevel(Define.UpgradeType.SpawnProbability, playerType);
        var probabilityDict = Calculator.GetFinalSpawnProbability(upgradeLevel);
        
        if (randomValue <= probabilityDict[Define.UnitGrade.Hero])
        {
            gradeUnitDatas = allUnitDatasPool.Where(u => u.Grade == Define.UnitGrade.Hero).ToList();
            grade = Define.UnitGrade.Hero;
        }
        else if (randomValue <= probabilityDict[Define.UnitGrade.Hero] + probabilityDict[Define.UnitGrade.Rare])
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

    public void MergeUnits(GridSystem.Cell targetCell, Define.PlayerType playerType)
    {
        if (!targetCell.IsCanMerge)
            return;

        var nextGrade = targetCell.UnitGrade + 1;
        foreach (var unit in targetCell.MyUnits)
        {
            Managers.Pool.Push(unit.GetComponent<Poolable>());
        }
        targetCell.ClearUnits();
        currentUnitsCountDict[playerType] -= 3;
        SpawnGradeUnit(nextGrade, playerType);
    }

    public void TrashUnit(GridSystem.Cell targetCell, Define.PlayerType playerType)
    {
        if (targetCell.MyUnits.Count == 0)
            return;
        
        var unit = targetCell.RemoveAndGetUnit();
        Managers.Pool.Push(unit.GetComponent<Poolable>());
        
        currentUnitsCountDict[playerType]--;
        if(playerType == Define.PlayerType.LocalPlayer)
            onLocalPlayerUnitCountChanged?.Invoke(currentUnitsCountDict[playerType]);
    }

    public void SellUnit(GridSystem.Cell targetCell, Define.PlayerType playerType)
    {
        switch (targetCell.UnitGrade)
        {

            case Define.UnitGrade.Normal:
                return;
            case Define.UnitGrade.Rare:
                InGameManagers.CurrencyMgr.ChipAmount += 1;
                break;
            case Define.UnitGrade.Hero:
                InGameManagers.CurrencyMgr.ChipAmount += 2;
                break;
            case Define.UnitGrade.Mythical:
                return;
        }
        
        TrashUnit(targetCell, playerType);
    }
}
