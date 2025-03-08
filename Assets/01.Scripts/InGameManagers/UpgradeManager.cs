using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeManager
{
    [SerializeField]
    private SerializedDictionary<Define.UpgradeType, int> upgradeDict;
    [SerializeField]
    private SerializedDictionary<Define.UpgradeType, int> aiPlayerUpgradeDict;

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

    public void Upgrade(Define.UpgradeType type)    // TODO: AI플레이어 업그레이드함수 구현
    {
        int price = Define.UpgradeStartPriceDict[type].price + GetUpgradeLevel(type) * Define.UpgradePriceAdderDict[type];
        Define.CurrencyType currencyType = Define.UpgradeStartPriceDict[type].currencyType;
        switch (currencyType)
        {
            case Define.CurrencyType.Coin:
                if (InGameManagers.CurrencyMgr.CoinAmount < price)
                    return;
                InGameManagers.CurrencyMgr.CoinAmount -= price;
                break;
            case Define.CurrencyType.Chip:
                if(InGameManagers.CurrencyMgr.ChipAmount < price)
                    return;
                InGameManagers.CurrencyMgr.ChipAmount -= price;
                break;
        }
        
        if (!upgradeDict.TryAdd(type, 1))
            upgradeDict[type]++;
        
        onUpgrade?.Invoke(type);
    }
}
