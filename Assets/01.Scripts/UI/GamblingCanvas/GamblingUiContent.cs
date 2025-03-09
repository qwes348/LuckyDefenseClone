using TMPro;
using UnityEngine;

public class GamblingUiContent : MonoBehaviour
{
    [SerializeField]
    private Define.UnitGrade grade;
    [SerializeField]
    private TextMeshProUGUI probabilityText;
    [SerializeField]
    private TextMeshProUGUI costText;

    private void Start()
    {
        InGameManagers.CurrencyMgr.onChipAmountChanged += CostColorupdate;
    }

    public void Init()
    {
        probabilityText.text = $"{Define.GamblingProbabilityDict[grade] * 100f:N0}%";
        costText.text = Define.GamblingPriceDict[grade].ToString();
        CostColorupdate(InGameManagers.CurrencyMgr.ChipAmount);
    }

    private void CostColorupdate(int amount)
    {
        costText.color = amount >= Define.GamblingPriceDict[grade] ? Color.white : Color.red;
    }

    public void Gamble()
    {
        // TODO: 도박 성공/실패 연출
        InGameManagers.UnitSpawnMgr.Gamble(Define.PlayerType.LocalPlayer, grade, gambleResult: null);
    }
}
