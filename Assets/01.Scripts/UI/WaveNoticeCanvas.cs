using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WaveNoticeCanvas : MonoBehaviour
{
    [Header("일반")]
    [SerializeField]
    private Image normalBg;
    [SerializeField]
    private TextMeshProUGUI normalText;

    [Header("보스")]
    [SerializeField]
    private Image bossBg;
    [SerializeField]
    private TextMeshProUGUI bossWaveText;
    [SerializeField]
    private TextMeshProUGUI bossNameText;
    [SerializeField]
    private TextMeshProUGUI bossTimeText;

    public void SetActiveCanvas(bool active)
    {
        gameObject.SetActive(active);
    }

    public async UniTask ShowNormal(int waveNumber)
    {
        normalBg.gameObject.SetActive(false);
        bossBg.gameObject.SetActive(false);
        normalText.gameObject.SetActive(false);
        normalText.transform.localScale = Vector3.one * 0.7f;
        normalText.text = $"WAVE  {waveNumber}";
        gameObject.SetActive(true);
        
        normalBg.gameObject.SetActive(true);
        normalText.gameObject.SetActive(true);
        await DOTween.Sequence()
            .Append(normalText.transform.DOScale(Vector3.one * 1.2f, 0.25f).SetEase(Ease.Linear))
            .Append(normalText.transform.DOScale(Vector3.one, 0.25f));

        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        
        normalBg.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public async UniTask ShowBossWave(int waveNumber, EnemyData enemyData, int waveTime)
    {
        normalBg.gameObject.SetActive(false);
        bossBg.gameObject.SetActive(false);
        bossNameText.gameObject.SetActive(false);
        bossTimeText.gameObject.SetActive(false);
        bossWaveText.gameObject.SetActive(true);
        
        bossBg.rectTransform.sizeDelta = new Vector2(bossBg.rectTransform.sizeDelta.x, 120f);
        bossWaveText.text = $"WAVE  {waveNumber}";
        bossNameText.text = $"\"{enemyData.UnitName}\"보스 등장!";
        int min = waveTime / 60;
        int sec = waveTime % 60;
        bossTimeText.text = $"제한 시간 {min:D2}:{sec:D2}";
        
        gameObject.SetActive(true);
        bossBg.gameObject.SetActive(true);
        
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));

        float rectX = bossBg.rectTransform.sizeDelta.x;
        await bossBg.rectTransform.DOSizeDelta(new Vector2(rectX, 195f), 0.2f).SetEase(Ease.Linear);
        bossNameText.gameObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        
        await bossBg.rectTransform.DOSizeDelta(new Vector2(rectX, 240f), 0.2f).SetEase(Ease.Linear);
        bossTimeText.gameObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        
        bossBg.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
