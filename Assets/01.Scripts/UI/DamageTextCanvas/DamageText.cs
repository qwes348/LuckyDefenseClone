using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI), typeof(Poolable))]
public class DamageText : MonoBehaviour
{
    private TextMeshProUGUI myText;
    private Poolable myPoolable;

    public void Init(Vector3 worldPos, float damage)
    {
        if(myText == null)
            myText = GetComponent<TextMeshProUGUI>();
        if(myPoolable == null)
            myPoolable = GetComponent<Poolable>();
        
        var screenPos = Camera.main.WorldToScreenPoint(worldPos);
        transform.position = screenPos;
        myText.text = damage.ToString("N0");
        myText.DOFade(1f, 0f);
        gameObject.SetActive(true);
        ShowTween().Forget();
    }

    private async UniTask ShowTween()
    {
        await (transform as RectTransform).DOBlendableMoveBy(Vector3.up * 45f, 0.3f).SetEase(Ease.OutCubic);
        await myText.DOFade(0.1f, 0.5f).SetEase(Ease.InQuad);
        Managers.Pool.Push(myPoolable);
    }
}
