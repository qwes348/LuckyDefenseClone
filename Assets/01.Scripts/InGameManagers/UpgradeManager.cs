using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeManager
{
    private Dictionary<Define.UpgradeType, int> upgradeDict = new Dictionary<Define.UpgradeType, int>();
    private Dictionary<Define.UpgradeType, int> aiPlayerUpgradeDict = new Dictionary<Define.UpgradeType, int>();

    public Action<Define.UpgradeType> onUpgrade;

    public void Init()
    {
        
    }

    public int GetUpgradeLevel(Define.UpgradeType type, Define.PlayerType playerType = Define.PlayerType.LocalPlayer)
    {
        int level = 0;
        if(playerType == Define.PlayerType.LocalPlayer)
            upgradeDict.TryGetValue(type, out level);
        else
            aiPlayerUpgradeDict.TryGetValue(type, out level);
        return level;
    }

    public IReadOnlyDictionary<Define.UpgradeType, int> GetUpgradeDict(Define.PlayerType playerType)
    {
        return playerType == Define.PlayerType.LocalPlayer ? upgradeDict : aiPlayerUpgradeDict;
    }

    public void Upgrade(Define.UpgradeType type, Define.PlayerType playerType)    // TODO: AI플레이어 업그레이드함수 구현
    {
        var costPair = GetCurrentUpgradeCost(type, playerType); 
        int cost = costPair.cost;
        Define.CurrencyType currencyType = costPair.currencyType;
        
        if (InGameManagers.CurrencyMgr.GetCurrency(costPair.currencyType, playerType) < cost)
            return;
        InGameManagers.CurrencyMgr.AddCurrency(cost * -1, currencyType, playerType);
        
        var dict = playerType == Define.PlayerType.LocalPlayer ? upgradeDict : aiPlayerUpgradeDict;
        if (!dict.TryAdd(type, 1))
            dict[type]++;
        
        if(playerType == Define.PlayerType.LocalPlayer)
            onUpgrade?.Invoke(type);
    }

    public (Define.CurrencyType currencyType, int cost) GetCurrentUpgradeCost(Define.UpgradeType type, Define.PlayerType playerType)
    {
        int level = GetUpgradeLevel(type, playerType);
        return (Define.UpgradeStartPriceDict[type].currencyType, Define.UpgradeStartPriceDict[type].price + Define.UpgradePriceAdderDict[type] * level);
    }
}
