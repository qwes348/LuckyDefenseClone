using Cysharp.Threading.Tasks;
using DG.Tweening;
using EditorAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class InGameManagers : MonoBehaviour
{
    private static InGameManagers instance;
    public static InGameManagers Instance { get { Init(); return instance; } }
    
    private bool initialized = false;
    public static bool Initialized => Instance.initialized;
    
    public Action onInitialized;

    #region Managers
    [SerializeField]
    private UnitSpawnManager unitSpawnMgr = new UnitSpawnManager();
    public static UnitSpawnManager UnitSpawnMgr => Instance.unitSpawnMgr;
    
    [SerializeField]
    private FieldManager fieldMgr = new FieldManager();
    public static FieldManager FieldMgr => Instance.fieldMgr;
    
    [SerializeField]
    private WaveManager waveMgr = new WaveManager();
    public static WaveManager WaveMgr => Instance.waveMgr;
    
    [SerializeField]
    private CurrencyManager currencyMgr = new CurrencyManager();
    public static CurrencyManager CurrencyMgr => Instance.currencyMgr;
    
    [SerializeField]
    private UpgradeManager upgradeMgr = new UpgradeManager();
    public static UpgradeManager UpgradeMgr => Instance.upgradeMgr;
    #endregion
    
    private void Start()
    {
        Init();
    }

    private static async UniTask Init()
    {
        if (instance == null)
        {
            GameObject go = GameObject.Find("#hInGameManagers");
            if (go == null)
            {
                go = new GameObject("#hInGameManagers");
                instance = go.AddComponent<InGameManagers>();
            }
            else
            {
                instance = go.GetComponent<InGameManagers>();
            }

            Managers.Game.onGameStateChanged += instance.OnGameStateChanged;
            
            instance.currencyMgr.Init();
            instance.upgradeMgr.Init();
            await instance.unitSpawnMgr.Init();
            await instance.waveMgr.Init();
            
            instance.initialized = true;
            instance.onInitialized?.Invoke();
        }
    }

    private void OnDestroy()
    {
        if (instance != this)
        {
            return;
        }
        
        Managers.Game.onGameStateChanged -= OnGameStateChanged;
        
        instance = null;
        unitSpawnMgr = null;
        fieldMgr = null;
        waveMgr = null;
        currencyMgr = null;
        initialized = false;
    }

    private void OnGameStateChanged(Define.GameState state)
    {
        switch (state)
        {
            case Define.GameState.None:
                break;
            case Define.GameState.Running:
                waveMgr.OnGameStart();
                break;
            case Define.GameState.GameOver:
                OnGameOver();
                break;
        }
    }

    private void OnGameOver()
    {
        // DOTween.defaultTimeScaleIndependent = true;
        // await DOVirtual.Float(1f, 0f, 1.5f, v => Time.timeScale = v);
        InGameUiManager.Instance.GameOver.SetActiveCanvas(true);
        fieldMgr.playerGrid.gameObject.SetActive(false);
        fieldMgr.aiPlayerGrid.gameObject.SetActive(false);
    }

    public void Retry()
    {
        Managers.Scene.LoadScene(Define.Scene.Game);
    }

    public void GameStart()
    {
        Managers.Game.GameState = Define.GameState.Running;
    }

    [Button]
    public void GameOverDebug()
    {
        Managers.Game.GameState = Define.GameState.GameOver;
    }
}

[Serializable]
public class FieldManager
{
    public GridSystem playerGrid;
    public GridSystem aiPlayerGrid;
    public EnemyWaypointsContainer enemyWaypointsContainer;

    public GridSystem GetGridSystem(Define.PlayerType playerType)
    {
        switch (playerType)
        {
            case Define.PlayerType.LocalPlayer:
                return playerGrid;
            case Define.PlayerType.AiPlayer:
                return aiPlayerGrid;
        }

        return null;
    }
}
