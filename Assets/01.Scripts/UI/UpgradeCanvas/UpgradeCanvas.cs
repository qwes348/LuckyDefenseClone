using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeCanvas : MonoBehaviour
{
    [SerializeField]
    private List<UpgradeUiContent> upgradeUiContents;
    [SerializeField]
    private GameObject SpawnProbabilityObject;
    [SerializeField]
    private TextMeshProUGUI SpawnProbabilityText;

    public void SetActiveCanvas(bool active)
    {
        if (active)
        {
            foreach (var content in upgradeUiContents)
            {
                content.Init();
            }
            SetActiveSpawnProbability(false);
            // 도박이나 신화조합 창이 열린상태에서 강화 버튼을 누를수있으므로 다른 창을 꺼준다
            InGameUiManager.Instance.Gambling.SetActiveCanvas(false);
        }
        gameObject.SetActive(active);
    }

    public void SetActiveCanvasToggle()
    {
        SetActiveCanvas(!gameObject.activeSelf);
    }

    public void SetActiveSpawnProbability(bool active)
    {
        if (active)
        {
            int upgradeLevel = InGameManagers.UpgradeMgr.GetUpgradeLevel(Define.UpgradeType.SpawnProbability, Define.PlayerType.LocalPlayer);
            
            var probabilityDict = Calculator.GetFinalSpawnProbability(upgradeLevel); 
            SpawnProbabilityText.text =
                $"일반: {probabilityDict[Define.UnitGrade.Normal] * 100f:N2}%\n" +
                $"<color=#64b6ff>희귀: {probabilityDict[Define.UnitGrade.Rare] * 100f:N2}%</color>\n" +
                $"<color=#bb85fd>영웅: {probabilityDict[Define.UnitGrade.Hero] * 100f:N2}%</color>";
        }
        SpawnProbabilityObject.SetActive(active);
    }
}
