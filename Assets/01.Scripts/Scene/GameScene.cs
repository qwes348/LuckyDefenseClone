using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        SceneType = Define.Scene.Game;
        Time.timeScale = 1;
    }
    
    public override void Clear()
    {
        
    }
}
