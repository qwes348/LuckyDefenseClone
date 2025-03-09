using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class UnitSpawnManager
{
    [SerializeField]
    private int currentSpawnCost;
    [SerializeField]
    private int currentAiSpawnCost;
    [SerializeField]
    private List<UnitData> allUnitDatasPool;

    private Dictionary<Define.PlayerType, int> currentUnitsCountDict;
    private List<MythCombineData> mythCombineDatas;
    
    #region Actions

    public Action<int> onSpawnCostChanged;
    public Action<int> onLocalPlayerUnitCountChanged;
    #endregion
    
    #region Properties
    public int CurrentSpawnCost
    {
        get => currentSpawnCost;
        private set { currentSpawnCost = value; onSpawnCostChanged?.Invoke(value); }
    }
    public int CurrentAiSpawnCost
    {
        get => currentAiSpawnCost;
        private set => currentAiSpawnCost = value;
    }
    public IReadOnlyList<MythCombineData> MythCombineDatas => mythCombineDatas;
    public IReadOnlyDictionary<Define.PlayerType, int> CurrentUnitsCountDict => currentUnitsCountDict;
    #endregion

    public async UniTask Init()
    {
        allUnitDatasPool = await Managers.Resource.LoadAssetsByLabel<UnitData>("unitData");
        currentSpawnCost = Define.StartSpawnCost;
        currentAiSpawnCost = Define.StartSpawnCost;
        currentUnitsCountDict = new Dictionary<Define.PlayerType, int>()
        {
            {Define.PlayerType.LocalPlayer, 0},
            {Define.PlayerType.AiPlayer, 0}
        };
        LoadMythCombineData();
    }

    public int GetSpawnCost(Define.PlayerType playerType)
    {
        switch (playerType)
        {
            case Define.PlayerType.LocalPlayer:
                return currentSpawnCost;
            case Define.PlayerType.AiPlayer:
                return currentAiSpawnCost;
        }

        return 0;
    }

    public void AddSpawnCost(int amount, Define.PlayerType playerType)
    {
        switch (playerType)
        {
            case Define.PlayerType.LocalPlayer:
                CurrentSpawnCost += amount;
                break;
            case Define.PlayerType.AiPlayer:
                CurrentAiSpawnCost += amount;
                break;
        }
    }

    public async UniTask SpawnRandomUnit(Define.PlayerType playerType)
    {
        if (GetSpawnCost(playerType) > InGameManagers.CurrencyMgr.GetCurrency(Define.CurrencyType.Coin, playerType))
            return;
        if (GetCurrentUnitsCount(playerType) >= Define.MaxUnitCount)
            return;

        currentUnitsCountDict[playerType]++;
        if (playerType == Define.PlayerType.LocalPlayer)
        {
            onLocalPlayerUnitCountChanged?.Invoke(currentUnitsCountDict[playerType]);
        }
        InGameManagers.CurrencyMgr.AddCurrency(GetSpawnCost(playerType) * -1, Define.CurrencyType.Coin, playerType);

        // TODO: AI측의 뽑기도 구현
        // TODO: 히어로 등급 뽑으면 UI에 축하메세지 출력
        var randomResult = GetRandomUnitDatasPool(playerType);
        var unitData = randomResult.gradeUnitDatas[Random.Range(0, randomResult.gradeUnitDatas.Count)];
        var unit = (await Managers.Pool.PopAsync(unitData.UnitPrefab)).GetComponent<UnitController>();
        unit.Init(unitData);

        GridSystem grid = InGameManagers.FieldMgr.GetGridSystem(playerType);
        grid.OnNewUnitspawned(unit);
        AddSpawnCost(Define.SpawnCostIncrease, playerType);
    }
    
    // 특정 등급 유닛을 소환
    public async UniTask SpawnGradeUnit(Define.UnitGrade unitGrade, Define.PlayerType playerType)
    {
        currentUnitsCountDict[playerType]++;
        if(playerType == Define.PlayerType.LocalPlayer)
            onLocalPlayerUnitCountChanged?.Invoke(currentUnitsCountDict[playerType]);
        
        var randomResult = allUnitDatasPool.Where(u => u.Grade == unitGrade).ToList();
        var unitData = randomResult[Random.Range(0, randomResult.Count)];
        var unit = (await Managers.Pool.PopAsync(unitData.UnitPrefab)).GetComponent<UnitController>();
        unit.Init(unitData);
        GridSystem grid = InGameManagers.FieldMgr.GetGridSystem(playerType);
        grid.OnNewUnitspawned(unit);
    }

    public async UniTask CombineMythUnit(MythCombineData combineData, Define.PlayerType playerType)
    {
        currentUnitsCountDict[playerType]++;
        if(playerType == Define.PlayerType.LocalPlayer)
            onLocalPlayerUnitCountChanged?.Invoke(currentUnitsCountDict[playerType]);
        
        GridSystem grid = InGameManagers.FieldMgr.GetGridSystem(playerType);
        List<GridSystem.Cell> materialCells = new List<GridSystem.Cell>();
        foreach (var material in combineData.materialUnits)
        {
            var cell = grid.GetCell(GetUnitDataById(material.unitId));
            if (cell == null)   // 재료 유닛을 하나라도 못찾으면 중단
                return;
            materialCells.Add(cell);
        }
        foreach (var cell in materialCells)
        {
            TrashUnit(cell, playerType);    // 재료유닛 소멸
        }
        
        UnitData mythUnitData = GetUnitDataById(combineData.unitId);
        var unit = (await Managers.Pool.PopAsync(mythUnitData.UnitPrefab)).GetComponent<UnitController>();
        unit.Init(mythUnitData);
        grid.OnNewUnitspawned(unit);        
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
        int amount = 0;
        Define.CurrencyType currencyType = Define.CurrencyType.Chip;
        switch (targetCell.UnitGrade)
        {

            case Define.UnitGrade.Normal:
                return;
            case Define.UnitGrade.Rare:
                amount = 1;
                currencyType = Define.CurrencyType.Chip;
                break;
            case Define.UnitGrade.Hero:
                amount = 2;
                currencyType = Define.CurrencyType.Chip;
                break;
            case Define.UnitGrade.Mythical:
                return;
        }
        
        InGameManagers.CurrencyMgr.AddCurrency(amount, currencyType, playerType);
        TrashUnit(targetCell, playerType);
    }

    // 도박
    public void Gamble(Define.PlayerType playerType, Define.UnitGrade grade, Action<bool> gambleResult = null)
    {
        if (InGameManagers.CurrencyMgr.GetCurrency(Define.CurrencyType.Chip, playerType) < Define.GamblingPriceDict[grade])
            return;
        if (GetCurrentUnitsCount(playerType) >= Define.MaxUnitCount)
            return;
        
        InGameManagers.CurrencyMgr.AddCurrency(Define.GamblingPriceDict[grade] * -1, Define.CurrencyType.Chip, playerType);
        
        var randomValue = Random.value;
        if (randomValue <= Define.GamblingProbabilityDict[grade])
        {
            Debug.Log($"[{playerType.ToString()}]가 [{grade.ToString()}]등급 도박성공");
            InGameManagers.UnitSpawnMgr.SpawnGradeUnit(grade, playerType);
            gambleResult?.Invoke(true);
        }
        else
        {
            Debug.Log($"[{playerType.ToString()}]가 [{grade.ToString()}]등급 도박실패");
            gambleResult?.Invoke(false);
        }
    }

    public int GetCurrentUnitsCount(Define.PlayerType playerType)
    {
        currentUnitsCountDict.TryGetValue(playerType, out int count);
        return count;
    }

    public UnitData GetUnitDataById(int id)
    {
        return allUnitDatasPool.First(u => u.UnitId == id);
    }
    
    private void LoadMythCombineData()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "myth_combine_data.json");
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            mythCombineDatas = JsonConvert.DeserializeObject<MythCombineDataWrapper>(jsonData).mythCombineDatas;
        }
        else
        {
            Debug.LogError("신화 조합 데이터 json을 찾을 수 없음");
            return;
        }
    }

    public MythCombineData GetAnyMythCombineAvailable(Define.PlayerType playerType)
    {
        // TODO: 최적화
        GridSystem grid = InGameManagers.FieldMgr.GetGridSystem(playerType);
        Dictionary<MythCombineData, int> mythMaterialHoldDict = new Dictionary<MythCombineData, int>();
        foreach (var data in mythCombineDatas)
        {
            foreach (var material in data.materialUnits)
            {
                // 진행도가 아닌 가능한 데이터를 찾는거니까 하나라도 없으면 break
                if (!grid.IsUnitExist(allUnitDatasPool.First(u => u.UnitId == material.unitId)))
                    break;
                if(!mythMaterialHoldDict.TryAdd(data, 1))
                    mythMaterialHoldDict[data]++;
            }
        }

        return (from pair in mythMaterialHoldDict where pair.Value >= pair.Key.materialUnits.Count select pair.Key).FirstOrDefault();
    }
}
