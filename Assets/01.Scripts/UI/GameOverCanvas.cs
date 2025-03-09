using Cysharp.Threading.Tasks;
using DG.Tweening;
using EditorAttributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverCanvas : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI gameOverText;
    [SerializeField]
    private TextMeshProUGUI reachedWaveText;
    [SerializeField]
    private Button retryButton;

    public void SetActiveCanvas(bool active)
    {
        if (active)
        {
            if (gameObject.activeSelf)
                return;
            ShowGameOver();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private async UniTaskVoid ShowGameOver()
    {
        reachedWaveText.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
        gameOverText.rectTransform.localScale = Vector3.right + Vector3.forward + Vector3.up * 1.5f;
        gameOverText.gameObject.SetActive(true);
        
        reachedWaveText.text = $"최종 웨이브: {InGameManagers.WaveMgr.CurrentWaveNumber - 1}";
        gameObject.SetActive(true);

        gameOverText.rectTransform.anchoredPosition = Vector2.up * 10f;
        gameOverText.rectTransform.DORotate(Vector3.forward * 15f, 0f);
        await gameOverText.rectTransform.DORotate(Vector3.forward * -15f, 0.1f).SetLoops(8, LoopType.Yoyo);
        gameOverText.rectTransform.DOScale(1f, 0.1f);
        await gameOverText.rectTransform.DORotate(Vector3.zero, 0.1f);
        await gameOverText.rectTransform.DOAnchorPosY(350f, 0.5f);

        reachedWaveText.rectTransform.localScale = Vector3.right + Vector3.forward;
        reachedWaveText.gameObject.SetActive(true);
        await reachedWaveText.rectTransform.DOScale(1f, 0.5f);
        retryButton.gameObject.SetActive(true);
    }

    public void Retry()
    {
        InGameManagers.Instance.Retry();
    }
}
