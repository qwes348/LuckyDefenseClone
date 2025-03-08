using System;
using UnityEngine;

public class InGameUiManager : StaticMono<InGameUiManager>
{
    [SerializeField]
    private UnitManageCanvas unitManage;
    [SerializeField]
    private UpgradeCanvas upgrade;
    
    public UnitManageCanvas UnitManage => unitManage;
    public UpgradeCanvas Upgrade => upgrade;

    private void Awake()
    {
        unitManage.SetActive(false);
        upgrade.SetActiveCanvas(false);
    }
}
