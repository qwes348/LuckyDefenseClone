using System;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI coinText; 
    [SerializeField]
    private TextMeshProUGUI chipText;
    [SerializeField]
    private TextMeshProUGUI unitCountText;

    private void Start()
    {
        if (coinText != null)
        {
            UpdateCoin(InGameManagers.CurrencyMgr.CoinAmount);
            InGameManagers.CurrencyMgr.onCoinAmountChanged += UpdateCoin;
        }
        if (chipText != null)
        {
            UpdateChip(InGameManagers.CurrencyMgr.ChipAmount);
            InGameManagers.CurrencyMgr.onChipAmountChanged += UpdateChip;
        }
        if (unitCountText != null)
        {
            UpdateUnitCount(0);
            InGameManagers.UnitSpawnMgr.onLocalPlayerUnitCountChanged += UpdateUnitCount;
        }
    }

    public void UpdateCoin(int amount)
    {
        if (coinText == null)
            return;
        coinText.text = amount.ToString();
    }

    public void UpdateChip(int amount)
    {
        if (chipText == null)
            return;
        chipText.text = amount.ToString();
    }

    public void UpdateUnitCount(int amount)
    {
        if (unitCountText == null)
            return;
        unitCountText.text = $"{amount}/{Define.MaxUnitCount}";
    }
}
