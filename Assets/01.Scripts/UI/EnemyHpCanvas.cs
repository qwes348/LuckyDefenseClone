using System;
using UnityEngine;

public class EnemyHpCanvas : MonoBehaviour
{
    [SerializeField]
    private Poolable enemyHpBarPrefab;
    [SerializeField]
    private Poolable bossHpBarPrefab;

    private void Start()
    {
        InGameManagers.WaveMgr.onEnemySpawned += AttachToEnemy;
    }

    public void AttachToEnemy(EnemyController enemy)
    {
        // TODO: 보스 체력바
        EnemyHpBar hpBar;
        if (enemy.MyEnemyData.Grade == Define.EnemyGrade.Normal)
            hpBar = Managers.Pool.Pop(enemyHpBarPrefab).GetComponent<EnemyHpBar>();
        else
            hpBar = Managers.Pool.Pop(bossHpBarPrefab).GetComponent<EnemyHpBar>();
        hpBar.transform.SetParent(transform);
        hpBar.Init(enemy);
    }
}
