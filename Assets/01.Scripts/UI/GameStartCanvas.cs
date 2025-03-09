using UnityEngine;

public class GameStartCanvas : MonoBehaviour
{
    public void SetActiveCanvas(bool active)
    {
        gameObject.SetActive(active);
    }
    
    public void GameStart()
    {
        InGameManagers.Instance.GameStart();
        SetActiveCanvas(false);
    }
}
