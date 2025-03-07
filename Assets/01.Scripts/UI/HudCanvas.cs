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
    
    [Header("Images")]
    [SerializeField]
    private SlicedFilledImage enemiesCountFillImage;

    [Header("Buttons")]
    [SerializeField]
    private Button unitSpawnButton;

    private void Start()
    {
        unitSpawnButton.onClick.AddListener(() => InGameManagers.UnitSpawnMgr.SpawnRandomUnit(Define.PlayerType.LocalPlayer));
        
        UpdateWaveNumberText(1);
        UpdateSpawnCostText(Define.StartSpawnCost);
        InGameManagers.WaveMgr.onWaveNumberChange += UpdateWaveNumberText;
        InGameManagers.WaveMgr.onWaveTimerTick += UpdateTimerText;
        InGameManagers.WaveMgr.onEnemiesCountChange += UpdateEnemiesCount;
        InGameManagers.UnitSpawnMgr.onSpawnCostChanged += UpdateSpawnCostText;
    }

    private void UpdateWaveNumberText(int waveNumber)
    {
        waveNumberText.text = $"WAVE  {waveNumber.ToString()}";
    }

    private void UpdateTimerText(int time)
    {
        int min = time / 60;
        int sec = time % 60;

        timerText.text = $"{min:D2}:{sec:D2}";
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
}
