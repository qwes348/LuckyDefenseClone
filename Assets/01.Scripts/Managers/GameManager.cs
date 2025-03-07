using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameManager
{
    [SerializeField]
    private Define.GameState gameState;

    #region 이벤트
    public Action<Define.GameState> onGameStateChanged;
    #endregion
    
    #region 프로퍼티
    public Define.GameState GameState { get => gameState;
        set
        {
            gameState = value;
            onGameStateChanged?.Invoke(value);
        }
    }
    #endregion
    
    public void Init()
    {
        gameState = Define.GameState.None;
    }

    public void Clear()
    {
        onGameStateChanged = null;
    }
}
