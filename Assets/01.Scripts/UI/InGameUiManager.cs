using System;
using UnityEngine;

public class InGameUiManager : StaticMono<InGameUiManager>
{
    [SerializeField]
    private UnitManageCanvas unitManage;
    [SerializeField]
    private UpgradeCanvas upgrade;
    [SerializeField]
    private GamblingCanvas gambling;
    
    public UnitManageCanvas UnitManage => unitManage;
    public UpgradeCanvas Upgrade => upgrade;
    public GamblingCanvas Gambling => gambling;

    private void Awake()
    {
        unitManage.SetActive(false);
        upgrade.SetActiveCanvas(false);
        gambling.SetActiveCanvas(false);
    }
}
