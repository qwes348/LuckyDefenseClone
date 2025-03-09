using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HudCanvas : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField]
    private TextMeshProUGUI waveNumberText;
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private TextMeshProUGUI enemiesCountText;
    [SerializeField]
    private TextMeshProUGUI spawnCostText;
    [SerializeField]
    private TextMeshProUGUI bossTimerText;
    [SerializeField]
    private TextMeshProUGUI mythCombineAvailableText;
    
    [Header("Images")]
    [SerializeField]
    private SlicedFilledImage enemiesCountFillImage;

    [Header("Buttons")]
    [SerializeField]
    private Button unitSpawnButton;
    
    [Header("Objects")]
    [SerializeField]
    private GameObject bossTimerObject;
    [SerializeField]
    private GameObject mythCombineAvailableObject;

    private void Start()
    {
        SetActiveBossTimer(false);
        mythCombineAvailableObject.SetActive(false);
        unitSpawnButton.onClick.AddListener(() => InGameManagers.UnitSpawnMgr.SpawnRandomUnit(Define.PlayerType.LocalPlayer));
        
        UpdateWaveNumberText(2);
        UpdateSpawnCostText(Define.StartSpawnCost);
        UpdateCoin(InGameManagers.CurrencyMgr.CoinAmount);
        UpdateEnemiesCount(0);
        UpdateTimerText(0);
        InGameManagers.WaveMgr.onWaveNumberChange += UpdateWaveNumberText;
        InGameManagers.WaveMgr.onWaveTimerTick += UpdateTimerText;
        InGameManagers.WaveMgr.onEnemiesCountChange += UpdateEnemiesCount;
        InGameManagers.WaveMgr.onBossWaveStart += () => SetActiveBossTimer(true);
        InGameManagers.WaveMgr.onBossAllDied += () => SetActiveBossTimer(false);
        InGameManagers.UnitSpawnMgr.onSpawnCostChanged += UpdateSpawnCostText;
        InGameManagers.CurrencyMgr.onCoinAmountChanged += UpdateCoin;
    }

    public void SetActiveBossTimer(bool active)
    {
        bossTimerText.color = Color.white;
        bossTimerObject.SetActive(active);
    }

    private void UpdateWaveNumberText(int waveNumber)
    {
        waveNumberText.text = $"WAVE  {(waveNumber - 1).ToString()}";
    }

    private void UpdateTimerText(int time)
    {
        int min = time / 60;
        int sec = time % 60;

        timerText.text = $"{min:D2}:{sec:D2}";

        if (!bossTimerObject.activeSelf)
        {
            return;
        }
        bossTimerText.text = $"{min:D2}:{sec:D2}";
        bossTimerText.color = time <= 10 ? Color.red : Color.white;
    }

    private void UpdateEnemiesCount(int count)
    {
        enemiesCountText.text = $"{count} / 100";
        enemiesCountFillImage.fillAmount = count / 100f;
    }

    private void UpdateSpawnCostText(int cost)
    {
        spawnCostText.text = $"{cost}";
        spawnCostText.color = InGameManagers.CurrencyMgr.CoinAmount >= cost ? Color.white : Color.red;
    }

    private void UpdateCoin(int amount)
    {
        int spawnCost = InGameManagers.UnitSpawnMgr.CurrentSpawnCost;
        spawnCostText.color = InGameManagers.CurrencyMgr.CoinAmount >= spawnCost ? Color.white : Color.red;
    }

    public void UpdateMythCombineAvailable()
    {
        var datas = InGameManagers.UnitSpawnMgr.GetAvailableMythCombineDatas(Define.PlayerType.LocalPlayer);
        mythCombineAvailableObject.SetActive(datas != null && datas.Count > 0);
        if(datas != null && datas.Count > 0)
            mythCombineAvailableText.text = $"{datas.Count}";
    }
}
