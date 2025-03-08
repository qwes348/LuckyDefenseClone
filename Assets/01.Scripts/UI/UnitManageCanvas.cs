using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitManageCanvas : MonoBehaviour
{
    [SerializeField]
    private Button mergeButton;
    [SerializeField]
    private Button trashButton;
    [SerializeField]
    private Button sellButton;
    [SerializeField]
    private TextMeshProUGUI priceText;
        
    private GridSystem.Cell targetCell;

    public Action<bool> onActiveStateChanged;

    private void Awake()
    {
        mergeButton.onClick.AddListener(Merge);
        trashButton.onClick.AddListener(Trash);
        sellButton.onClick.AddListener(Sell);
    }

    public void Init(GridSystem.Cell cell)
    {
        targetCell = cell;
        transform.position = targetCell.GetWorldPosition();
        trashButton.gameObject.SetActive(cell.MyUnits[0].MyUnitData.Grade == Define.UnitGrade.Normal);
        mergeButton.interactable = cell.IsCanMerge;
        sellButton.gameObject.SetActive(cell.MyUnits[0].MyUnitData.Grade is Define.UnitGrade.Rare or Define.UnitGrade.Hero);
        
        switch (cell.UnitGrade)
        {
            case Define.UnitGrade.Normal:
                sellButton.gameObject.SetActive(false);
                break;
            case Define.UnitGrade.Rare:
                priceText.text = "+1";
                break;
            case Define.UnitGrade.Hero:
                priceText.text = "+2";
                break;
            case Define.UnitGrade.Mythical:
                mergeButton.gameObject.SetActive(false);
                trashButton.gameObject.SetActive(false);
                sellButton.gameObject.SetActive(false);
                break;
        }
    }
    
    public void SetActive(bool active)
    {
        if (!active)
        {
            targetCell = null;
        }

        if (active != gameObject.activeSelf)
        {
            gameObject.SetActive(active);
            onActiveStateChanged?.Invoke(active);
        }
    }

    public void Merge()
    {
        if (targetCell == null)
            return;
        if (!targetCell.IsCanMerge)
            return;
        
        InGameManagers.UnitSpawnMgr.MergeUnits(targetCell, Define.PlayerType.LocalPlayer);
        SetActive(false);
    }

    public void Sell()
    {
        if (targetCell == null)
            return;
        
        InGameManagers.UnitSpawnMgr.SellUnit(targetCell, Define.PlayerType.LocalPlayer);
        SetActive(false);
    }

    public void Trash()
    {
        if(targetCell == null)
            return;
        
        InGameManagers.UnitSpawnMgr.TrashUnit(targetCell, Define.PlayerType.LocalPlayer);
        SetActive(false);
    }
}
