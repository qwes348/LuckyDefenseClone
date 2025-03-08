using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Define 
{
    #region constant
    
    public const int StartCoinAmount = 100;
    public const int StartSpawnCost = 20;
    public const int SpawnCostIncrease = 2;
    public const int MaxUnitCount = 20;
    
    public const float DEFAULT_BGM_VOLUME = 0.4f;
    public const float DEFAULT_SFX_VOLUME = 0.5f;

    public static readonly float[] BGM_PITCH = new float[] { 1.0f, 1.1f, 1.2f };
    // 노강기준 등급 별 소환 확률
    public static readonly Dictionary<UnitGrade, float> DefaultSpawnProbabilityDict = new Dictionary<UnitGrade, float>()
    {
        {UnitGrade.Normal, 97.47f},
        {UnitGrade.Rare, 1.95f},
        {UnitGrade.Hero, 0.58f},
    };
    // 강화수치 적용한 소환 확률 계산에 사용할 조정값
    public static readonly Dictionary<UnitGrade, float> SpawnProbabilityAdjustmentDict = new Dictionary<UnitGrade, float>()
    {
        {UnitGrade.Normal, 0.8f},
        {UnitGrade.Rare, 0.8f},
        {UnitGrade.Hero, 1.2f},
    };
    // 유닛 발판 컬러
    public static readonly Dictionary<UnitGrade, Color> UnitFoodholdColorDict = new Dictionary<Define.UnitGrade, Color>()
    {
        {UnitGrade.Normal, new Color(0.86f, 0.86f, 0.86f, 0.7f)},
        {UnitGrade.Rare, new Color(0.3820755f, 0.6025897f, 1f, 0.7f)},
        {UnitGrade.Hero, new Color(0.8769979f, 0f, 1f, 0.7f)},
        {UnitGrade.Mythical, new Color(1f, 1f, 0f, 0.7f)}
    };
    // 업그레이드(강화) 시작 가격
    public static readonly Dictionary<UpgradeType, (CurrencyType currencyType, int price)> UpgradeStartPriceDict = new Dictionary<UpgradeType, (CurrencyType, int)>()
    {
        { UpgradeType.NormalRare, (CurrencyType.Coin, 30) },
        { UpgradeType.Hero, (CurrencyType.Coin, 50) },
        { UpgradeType.Mythical, (CurrencyType.Chip, 2) },
        { UpgradeType.SpawnProbability, (CurrencyType.Coin, 100) }
    };
    // 업그레이드(강화) 레벨당 추가 가격
    public static readonly Dictionary<UpgradeType, int> UpgradePriceAdderDict = new Dictionary<UpgradeType, int>()
    {
        { UpgradeType.NormalRare, 30 },
        { UpgradeType.Hero, 50 },
        { UpgradeType.Mythical, 1 },
        { UpgradeType.SpawnProbability, 0 }
    };
    public static readonly Dictionary<UnitGrade, int> GamblingPriceDict = new Dictionary<UnitGrade, int>()
    {
        {UnitGrade.Rare, 1},
        {UnitGrade.Hero, 1},
    };
    public static readonly Dictionary<UnitGrade, float> GamblingProbabilityDict = new Dictionary<UnitGrade, float>()
    {
        {UnitGrade.Rare, 0.6f},
        {UnitGrade.Hero, 0.2f}
    };
    #endregion

    #region enum
    public enum Scene
    {
        Title,
        Game,
        Loading,
        Score
    }
    
    public enum GameState
    {
        None,
        Running,
        GameOver
    }
    public enum UnitGrade
    {
        Normal,
        Rare,
        Hero,
        Mythical
    }
    public enum EnemyGrade
    {
        Normal,
        Boss
    }
    public enum PlayerType
    {
        LocalPlayer,
        AiPlayer
    }
    public enum EnemyState
    {
        None = -1,
        Move,
        Died
    }
    public enum DamageType
    {
        Physical,
        Magical
    }
    public enum UpgradeType
    {
        NormalRare,
        Hero,
        Mythical,
        SpawnProbability
    }
    public enum CurrencyType
    {
        Coin,
        Chip
    }
    public enum Sfx
    {
        Move,
        Click,
        GameStart,
        GameOver
    }
    public enum Bgm
    {
        Title,
        Game,
        Score
    }
    #endregion
}
