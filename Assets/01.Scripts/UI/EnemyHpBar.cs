using System;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    [SerializeField]
    private SlicedFilledImage fillImage;

    private EnemyController target;
    
    public void Init(EnemyController target)
    {
        this.target = target;
        fillImage.fillAmount = 1f;
        target.onHealthChanged += OnEnemyHealthChanged;
        target.onDied += OnEnemyDied;
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
    }

    private void Update()
    {
        var screenPos = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up * 0.2f);
        transform.position = screenPos;
    }
}
