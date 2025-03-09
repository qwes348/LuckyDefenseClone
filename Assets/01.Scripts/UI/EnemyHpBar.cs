using System;
using TMPro;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    [SerializeField]
    private SlicedFilledImage fillImage;
    [SerializeField]
    private TextMeshProUGUI hpText;

    private EnemyController target;
    
    public void Init(EnemyController target)
    {
        this.target = target;
        fillImage.fillAmount = 1f;
        target.onHealthChanged += OnEnemyHealthChanged;
        target.onDied += OnEnemyDied;
        if (hpText != null)
            hpText.text = $"{target.CurrentHelth:N0}";
        gameObject.SetActive(true);
    }

    private void OnEnemyDied()
    {
        Managers.Pool.Push(GetComponent<Poolable>());
    }

    private void OnEnemyHealthChanged(float currentHealth)
    {
        float remain = currentHealth / target.MyEnemyData.MaxHealth;
        fillImage.fillAmount = remain;
        if(hpText != null)
            hpText.text = $"{currentHealth:N0}";
    }

    private void Update()
    {
        var screenPos = Camera.main.WorldToScreenPoint(target.BodyTransform.position + Vector3.up * 0.2f);
        transform.position = screenPos;
    }
}
