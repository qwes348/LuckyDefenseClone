using System;
using UnityEngine;

[Serializable]
public class CurrencyManager
{
    [SerializeField]
    private int coinAmount;
    [SerializeField]
    private int chipAmount;
    
    #region Actions
    public Action<int> onCoinAmountChanged;
    public Action<int> onChipAmountChanged;
    #endregion
    
    #region Properties

    public int CoinAmount
    {
        get => coinAmount;
        set
        {
            coinAmount = value;
            onCoinAmountChanged?.Invoke(coinAmount);
        }
    }
    public int ChipAmount
    {
        get => chipAmount;
        set
        {
            chipAmount = value;
            onChipAmountChanged?.Invoke(chipAmount);
        }
    }
    #endregion

    public void Init()
    {
        CoinAmount = Define.StartCoinAmount;
    }
}
