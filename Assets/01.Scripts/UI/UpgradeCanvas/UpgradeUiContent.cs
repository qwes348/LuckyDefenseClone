using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUiContent : MonoBehaviour
{
    [SerializeField]
    private Define.UpgradeType myUpgradeType;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI costText;
    [SerializeField]
    private Image currencyImage;
    [SerializeField]
    private Sprite coinSprite;
    [SerializeField]
    private Sprite chipSprite;

    private void Start()
    {
        var currencyType = Define.UpgradeStartPriceDict[myUpgradeType].currencyType;
        switch (currencyType)
        {
            case Define.CurrencyType.Coin:
                InGameManagers.CurrencyMgr.onCoinAmountChanged += CostColorUpdate;
                break;
            case Define.CurrencyType.Chip:
                InGameManagers.CurrencyMgr.onChipAmountChanged += CostColorUpdate;
                break;
        }
    }

    public void Init()
    {
        int level = InGameManagers.UpgradeMgr.GetUpgradeLevel(myUpgradeType);
        levelText.text = $"Lv.{level+1}";
        costText.text = (Define.UpgradeStartPriceDict[myUpgradeType].price + level * Define.UpgradePriceAdderDict[myUpgradeType]).ToString();

        var currencyType = Define.UpgradeStartPriceDict[myUpgradeType].currencyType;
        currencyImage.sprite = currencyType == Define.CurrencyType.Coin ? coinSprite : chipSprite;
        currencyImage.GetComponent<Outline>().enabled = currencyType != Define.CurrencyType.Coin;
        int currentCurrencyAmount = currencyType == Define.CurrencyType.Coin ? InGameManagers.CurrencyMgr.CoinAmount : InGameManagers.CurrencyMgr.ChipAmount;
        CostColorUpdate(currentCurrencyAmount);
    }

    private void CostColorUpdate(int currencyAmount)
    {
        int level = InGameManagers.UpgradeMgr.GetUpgradeLevel(myUpgradeType);
        int cost = Define.UpgradeStartPriceDict[myUpgradeType].price + level * Define.UpgradePriceAdderDict[myUpgradeType];
        costText.color = currencyAmount >= cost ? Color.white : Color.red;
    }

    public void Upgrade()
    {
        InGameManagers.UpgradeMgr.Upgrade(myUpgradeType, Define.PlayerType.LocalPlayer);
        Init();
    }
}
