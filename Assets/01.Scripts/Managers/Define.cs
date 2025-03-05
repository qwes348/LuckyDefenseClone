using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Define 
{
    #region constant
    
    public const int MAX_CHARACTERS_COUNT = 20;
    public const int LEVEL_UP_THRESHOLD = 4000;
    public const int MAX_LEVEL = 4;
    public const int FEVER_THRESHOLD = 10;
    public const int SUPER_FEVER_THRESHOLD = 25;
    
    public const float SPECIAL_PROBABILITY = 0.02f;
    public const float GAME_INITIAL_TIME = 60f;
    public const float FEVER_DURATION = 3f;
    public const float DEFAULT_BGM_VOLUME = 0.4f;
    public const float DEFAULT_SFX_VOLUME = 0.5f;

    public static readonly float[] BGM_PITCH = new float[] { 1.0f, 1.1f, 1.2f };
    
    #endregion

    #region enum
    public enum Scene
    {
        Title,
        Game,
        Loading,
        Score
    }

    public enum CharacterClass
    {
        None = -1,
        Normal,
        Special
    }

    public enum Character
    {
        None = -1,
        Bear,
        Chick,
        Chicken,
        Cow,
        Duck,
        Giraffe,
        Hippo,
        Narwhal,
        Panda,
        Parrot,
        Penguin,
        Pig,
        Snake,
        Whale,
        Zebra,
        Bomb = 100
    }

    public enum InputType
    {
        Left,
        Right
    }
    public enum GameState
    {
        None = 0,
        Running = 1,
        GameOver = 2
    }
    public enum FeverState
    {
        Normal = 1,
        Fever = 2,
        SuperFever = 4
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
