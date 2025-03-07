using Cysharp.Threading.Tasks;
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
            
            instance.currencyMgr.Init();
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
        instance = null;
        unitSpawnMgr = null;
        fieldMgr = null;
        waveMgr = null;
        currencyMgr = null;
        initialized = false;
    }

    [Button]
    public void SpawnTest()
    {
        unitSpawnMgr.SpawnRandomUnit(Define.PlayerType.LocalPlayer);
    }

    [Button]
    public void WaveSpawnTest()
    {
        waveMgr.StartWaveTimer(5);
    }
}

[Serializable]
public class FieldManager
{
    public GridSystem playerGrid;
    public GridSystem opponentGrid;
    [FormerlySerializedAs("enemyWaypoints")]
    public EnemyWaypointsContainer enemyWaypointsContainer;
}
