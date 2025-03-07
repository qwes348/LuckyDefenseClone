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
    
    public const float DEFAULT_BGM_VOLUME = 0.4f;
    public const float DEFAULT_SFX_VOLUME = 0.5f;

    public static readonly float[] BGM_PITCH = new float[] { 1.0f, 1.1f, 1.2f };
    public static readonly Dictionary<UnitGrade, float> DefaultSpawnRateDict = new Dictionary<UnitGrade, float>()
    {
        {UnitGrade.Normal, 97.47f},
        {UnitGrade.Rare, 1.95f},
        {UnitGrade.Hero, 0.58f},
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
        Dead
    }
    public enum DamageType
    {
        Physical,
        Magical
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
