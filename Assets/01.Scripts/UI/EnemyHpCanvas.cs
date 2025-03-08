using System;
using UnityEngine;

public class EnemyHpCanvas : MonoBehaviour
{
    [SerializeField]
    private Poolable enemyHpBarPrefab;

    private void Start()
    {
        InGameManagers.WaveMgr.onEnemySpawned += AttachToEnemy;
    }

    public void AttachToEnemy(EnemyController enemy)
    {
        // TODO: 보스 체력바
        var hpBar = Managers.Pool.Pop(enemyHpBarPrefab).GetComponent<EnemyHpBar>();
        hpBar.transform.SetParent(transform);
        hpBar.Init(enemy);
    }
}
