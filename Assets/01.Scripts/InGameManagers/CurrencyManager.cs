using System;
using UnityEngine;

[Serializable]
public class CurrencyManager
{
    [Header("유저 플레이어")]
    [SerializeField]
    private int coinAmount;
    [SerializeField]
    private int chipAmount;
    
    [Header("AI 플레이어")]
    [SerializeField]
    private int aiCoinAmount;
    [SerializeField]
    private int aiChipAmount;
    
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
    public int AiCoinAmount
    {
        get => aiCoinAmount;
        set
        {
            aiCoinAmount = value;
        }
    }
    public int AiChipAmount
    {
        get => aiChipAmount;
        set
        {
            aiChipAmount = value;
        }
    }
    #endregion

    public void Init()
    {
        CoinAmount = Define.StartCoinAmount;
        aiCoinAmount = Define.StartCoinAmount;
    }

    public void AddCurrency(int amount, Define.CurrencyType currencyType, Define.PlayerType playerType)
    {
        switch (playerType)
        {
            case Define.PlayerType.LocalPlayer:
                switch (currencyType)
                {
                    case Define.CurrencyType.Coin:
                        CoinAmount += amount;
                        break;
                    case Define.CurrencyType.Chip:
                        ChipAmount += amount;
                        break;
                }
                break;
            case Define.PlayerType.AiPlayer:
                switch (currencyType)
                {
                    case Define.CurrencyType.Coin:
                        AiCoinAmount += amount;
                        break;
                    case Define.CurrencyType.Chip:
                        AiChipAmount += amount;
                        break;
                }
                break;
        }
    }

    public int GetCurrency(Define.CurrencyType currencyType, Define.PlayerType playerType)
    {
        switch (playerType)
        {
            case Define.PlayerType.LocalPlayer:
                switch (currencyType)
                {
                    case Define.CurrencyType.Coin:
                        return CoinAmount;
                    case Define.CurrencyType.Chip:
                        return ChipAmount;
                }
                break;
            case Define.PlayerType.AiPlayer:
                switch (currencyType)
                {
                    case Define.CurrencyType.Coin:
                        return AiCoinAmount;
                    case Define.CurrencyType.Chip:
                        return AiChipAmount;
                }
                break;
        }
        
        return 0;
    }
}
