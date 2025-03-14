using System;
using UnityEngine;

public class InGameUiManager : StaticMono<InGameUiManager>
{
    [SerializeField]
    private HudCanvas hud;
    [SerializeField]
    private UnitManageCanvas unitManage;
    [SerializeField]
    private UpgradeCanvas upgrade;
    [SerializeField]
    private GamblingCanvas gambling;
    [SerializeField]
    private MythCombineCanvas mythCombine;
    [SerializeField]
    private WaveNoticeCanvas waveNotice;
    [SerializeField]
    private GameOverCanvas gameOver;
    [SerializeField]
    private GameStartCanvas gameStart;
    [SerializeField]
    private DamageCanvas damage;

    public HudCanvas HUD => hud;
    public UnitManageCanvas UnitManage => unitManage;
    public UpgradeCanvas Upgrade => upgrade;
    public GamblingCanvas Gambling => gambling;
    public MythCombineCanvas MythCombine => mythCombine;
    public WaveNoticeCanvas WaveNotice => waveNotice;
    public GameOverCanvas GameOver => gameOver;
    public GameStartCanvas GameStart => gameStart;
    public DamageCanvas Damage => damage;

    private void Awake()
    {
        unitManage.SetActiveCanvas(false);
        upgrade.SetActiveCanvas(false);
        gambling.SetActiveCanvas(false);
        mythCombine.SetActiveCanvas(false);
        waveNotice.SetActiveCanvas(false);
        gameOver.SetActiveCanvas(false);
        gameStart.SetActiveCanvas(true);
        
        damage.gameObject.SetActive(true);
    }

    public void OnUpgradeCanvasActivate()
    {
        gambling.SetActiveCanvas(false);
        mythCombine.SetActiveCanvas(false);
    }
}
