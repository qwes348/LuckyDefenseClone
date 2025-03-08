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
    [SerializeField]
    private MythCombineCanvas mythCombine;
    
    public UnitManageCanvas UnitManage => unitManage;
    public UpgradeCanvas Upgrade => upgrade;
    public GamblingCanvas Gambling => gambling;
    public MythCombineCanvas MythCombine => mythCombine;

    private void Awake()
    {
        unitManage.SetActive(false);
        upgrade.SetActiveCanvas(false);
        gambling.SetActiveCanvas(false);
        mythCombine.SetActiveCanvas(false);
    }

    public void OnUpgradeCanvasActivate()
    {
        gambling.SetActiveCanvas(false);
        mythCombine.SetActiveCanvas(false);
    }
}
