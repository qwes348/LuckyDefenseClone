using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AiPlayerController : MonoBehaviour
{
    [SerializeField]
    // 마지막으로 결정한 행동의 경과시간 딕셔너리
    private SerializedDictionary<Define.AiActionType, float> lastDecisionElapsedTimeDict = new SerializedDictionary<Define.AiActionType, float>();
    [SerializeField]
    // 행동 우선순위 값 딕셔너리 (디버그용)
    private SerializedDictionary<Define.AiActionType, int> utilityActionsDict = new SerializedDictionary<Define.AiActionType, int>();
    private GridSystem myGrid;
    private MythCombineData availableMythCombineData;
    
    // 한프레임 늦는 Start
    private IEnumerator Start()
    {
        while (Managers.Game.GameState != Define.GameState.Running)
            yield return null;
        
        foreach (Define.AiActionType act in Enum.GetValues(typeof(Define.AiActionType)))
        {
            lastDecisionElapsedTimeDict.Add(act, 0);
        }
        yield return null;

        myGrid = InGameManagers.FieldMgr.aiPlayerGrid;
        DecisionTick();
    }

    private async UniTaskVoid DecisionTick()
    {
        while (true)
        {
            if (Managers.Game.GameState == Define.GameState.GameOver)
                break;
            await UniTask.Delay(TimeSpan.FromSeconds(0.5));

            int randomSpawnUtility = CalculateRandomSpawnUtility();
            int mergeUtility = CalculateMergeUtility();
            int gambleUtility = CalculateGamblingUtility();
            int mythCombineUtility = CalculateMythCombineUtility();
            int upgradeUtility = CalculateUpgradeUtility();

            int maxUtility = Mathf.Max(randomSpawnUtility, mergeUtility, gambleUtility, mythCombineUtility);
            if(maxUtility > 0)
            {
                if (maxUtility == randomSpawnUtility)
                {
                    lastDecisionElapsedTimeDict[Define.AiActionType.RandomSpawn] = 0;
                    DoRandomSpawn();
                }
                else if (maxUtility == mergeUtility)
                {
                    lastDecisionElapsedTimeDict[Define.AiActionType.Merge] = 0;
                    DoMerge();
                }
                else if (maxUtility == gambleUtility)
                {
                    lastDecisionElapsedTimeDict[Define.AiActionType.Gambling] = 0;
                    DoGambling();
                }
                else if (maxUtility == mythCombineUtility)
                {
                    lastDecisionElapsedTimeDict[Define.AiActionType.MythCombine] = 0;
                    DoMythCombine();
                }
                else if (maxUtility == upgradeUtility)
                {
                    lastDecisionElapsedTimeDict[Define.AiActionType.Upgrade] = 0;
                    DoUpgrade();
                }
            }

            foreach (Define.AiActionType actionType in Enum.GetValues(typeof(Define.AiActionType)))
            {
                lastDecisionElapsedTimeDict[actionType] += 0.5f;
            }
        }
    }

    #region 유틸리티(우선순위) 계산

    private int CalculateRandomSpawnUtility()
    {
        int result = 0;
        if (InGameManagers.CurrencyMgr.GetCurrency(Define.CurrencyType.Coin, Define.PlayerType.AiPlayer) < InGameManagers.UnitSpawnMgr.CurrentAiSpawnCost)
            result -= 999;
        else
        {
            result += 1;
            result += Mathf.RoundToInt(lastDecisionElapsedTimeDict[Define.AiActionType.RandomSpawn]);
        }
        if (InGameManagers.WaveMgr.CurrentWaveNumber == 1)
        {
            result += 100;
        }
        
        if(!utilityActionsDict.TryAdd(Define.AiActionType.RandomSpawn, result))
            utilityActionsDict[Define.AiActionType.RandomSpawn] = result;
        return result;
    }

    private int CalculateMergeUtility()
    {
        int result = 0;
        GridSystem grid = InGameManagers.FieldMgr.aiPlayerGrid;
        foreach (var pair in grid.UnitCoordDict)
        {
            if(pair.Value == null || pair.Value.Count == 0)
                continue;
            // 합성 가능한 셀 하나당 10점 추가
            result += pair.Value.Where(cell => cell.IsCanMerge).Sum(cell => 10);
        }

        // 합성가능한 셀이 하나 이상 존재한다면 경과시간 가산치 더함
        if (result >= 10)
        {
            result += Mathf.RoundToInt(lastDecisionElapsedTimeDict[Define.AiActionType.Merge]);
        }
        else  // 합성 가능한 셀이 없다면 시도하지않게 가산치치를 크게 낮춤
        {
            result -= 999;
        }
        
        if(!utilityActionsDict.TryAdd(Define.AiActionType.Merge, result))
            utilityActionsDict[Define.AiActionType.Merge] = result;
        return result;
    }

    private int CalculateGamblingUtility()
    {
        int result = 0;
        int chipAmount = InGameManagers.CurrencyMgr.GetCurrency(Define.CurrencyType.Chip, Define.PlayerType.AiPlayer);
        if (chipAmount >= 1)
        {
            result += 10;
        }
        else
        {
            result -= 999;
        }
        
        // 도박 가능하다면 경과시간 가산치 더함
        if (result >= 10)
        {
            result += Mathf.RoundToInt(lastDecisionElapsedTimeDict[Define.AiActionType.Gambling]);
        }
        
        if(!utilityActionsDict.TryAdd(Define.AiActionType.Gambling, result))
            utilityActionsDict[Define.AiActionType.Gambling] = result;
        return result;
    }

    private int CalculateUpgradeUtility()
    {
        int result = 0;
        int chipAmount = InGameManagers.CurrencyMgr.GetCurrency(Define.CurrencyType.Chip, Define.PlayerType.AiPlayer);
        int coinAmount = InGameManagers.CurrencyMgr.GetCurrency(Define.CurrencyType.Coin, Define.PlayerType.AiPlayer);
        
        foreach (Define.UpgradeType upgradeType in Enum.GetValues(typeof(Define.UpgradeType)))
        {
            var upgradeCost = InGameManagers.UpgradeMgr.GetCurrentUpgradeCost(upgradeType, Define.PlayerType.AiPlayer);
            switch (upgradeCost.currencyType)
            {
                case Define.CurrencyType.Coin:
                    if (coinAmount >= upgradeCost.cost)
                        result += 2;
                    break;
                case Define.CurrencyType.Chip:
                    if (chipAmount >= upgradeCost.cost)
                        result += 2;
                    break;
            }
        }

        if(!utilityActionsDict.TryAdd(Define.AiActionType.Upgrade, result))
            utilityActionsDict[Define.AiActionType.Upgrade] = result;
        return result;
    }

    private int CalculateMythCombineUtility()
    {
        int result = 0;
        availableMythCombineData = InGameManagers.UnitSpawnMgr.GetAnyMythCombineAvailable(Define.PlayerType.AiPlayer);
        if (availableMythCombineData == null)
            result -= 999;
        else
            result += 10;
        
        if(result >= 10)
            result += Mathf.RoundToInt(lastDecisionElapsedTimeDict[Define.AiActionType.MythCombine]);

        if(!utilityActionsDict.TryAdd(Define.AiActionType.MythCombine, result))
            utilityActionsDict[Define.AiActionType.MythCombine] = result;
        return result;
    }
    #endregion
    
    #region 행동

    private void DoRandomSpawn()
    {
        InGameManagers.UnitSpawnMgr.SpawnRandomUnit(Define.PlayerType.AiPlayer);
    }

    private void DoMerge()
    {
        GridSystem grid = InGameManagers.FieldMgr.aiPlayerGrid;
        foreach (var pair in grid.UnitCoordDict)
        {
            if(pair.Value == null || pair.Value.Count == 0)
                continue;
            // 합성 가능한 셀 하나당 10점 추가
            foreach (var cell in pair.Value)
            {
                if (cell.IsCanMerge)
                {
                    InGameManagers.UnitSpawnMgr.MergeUnits(cell, Define.PlayerType.AiPlayer);
                    return;
                }
                
            }
        }
    }
    
    private void DoGambling()
    {
        var randomValue = Random.value;
        if (randomValue < 0.8f)
        {
            InGameManagers.UnitSpawnMgr.Gamble(Define.PlayerType.AiPlayer, Define.UnitGrade.Rare);
        }
        {
            InGameManagers.UnitSpawnMgr.Gamble(Define.PlayerType.AiPlayer, Define.UnitGrade.Hero);
        }
    }

    private void DoUpgrade()
    {
        List<Define.UpgradeType> availableUpgradeTypes = new List<Define.UpgradeType>();
        int coinAmount = InGameManagers.CurrencyMgr.GetCurrency(Define.CurrencyType.Coin, Define.PlayerType.AiPlayer);
        int chipAmount = InGameManagers.CurrencyMgr.GetCurrency(Define.CurrencyType.Chip, Define.PlayerType.AiPlayer);
        foreach (Define.UpgradeType upgradeType in Enum.GetValues(typeof(Define.UpgradeType)))
        {
            var upgradeCost = InGameManagers.UpgradeMgr.GetCurrentUpgradeCost(upgradeType, Define.PlayerType.AiPlayer);
            switch (upgradeCost.currencyType)
            {
                case Define.CurrencyType.Coin:
                    if (coinAmount >= upgradeCost.cost)
                        availableUpgradeTypes.Add(upgradeType);
                    break;
                case Define.CurrencyType.Chip:
                    if (chipAmount >= upgradeCost.cost)
                        availableUpgradeTypes.Add(upgradeType);
                    break;
            }
        }

        if (availableUpgradeTypes.Count <= 0)
            return;

        int minLevel = int.MaxValue;
        Define.UpgradeType minUpgradeType = availableUpgradeTypes[0];
        foreach (var upgradeType in availableUpgradeTypes)
        {
            int level = InGameManagers.UpgradeMgr.GetUpgradeLevel(upgradeType, Define.PlayerType.AiPlayer);
            if (level < minLevel)
            {
                minLevel = level;
                minUpgradeType = upgradeType;
            }
        }
        
        InGameManagers.UpgradeMgr.Upgrade(minUpgradeType, Define.PlayerType.AiPlayer);
        Debug.Log($"AI가 {minUpgradeType} 업그레이드 진행");
    }

    private void DoMythCombine()
    {
        InGameManagers.UnitSpawnMgr.CombineMythUnit(availableMythCombineData, Define.PlayerType.AiPlayer);
        availableMythCombineData = null;
    }
    #endregion
}
