using EditorAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("런타임에 채워지는 데이터들")]
    [SerializeField]
    private EnemyData myEnemyData;
    [SerializeField, ReadOnly]
    private int currentWaypointIndex;
    [SerializeField, ReadOnly]
    private Define.EnemyState currentState = Define.EnemyState.None;
    [SerializeField, ReadOnly]
    private float currentHelth;
    
    private IReadOnlyList<Vector3> waypoints;
    private Define.PlayerType myPlayerType;

    #region Properties

    public EnemyData MyEnemyData => myEnemyData;

    #endregion
    
    public void Init(EnemyData newData, Define.PlayerType playerType)
    {
        myEnemyData = newData;
        myPlayerType = playerType;
        currentWaypointIndex = 1;
        currentHelth = newData.MaxHealth;
        
        waypoints = myPlayerType == Define.PlayerType.LocalPlayer ? 
            InGameManagers.FieldMgr.enemyWaypointsContainer.PlayerSideWaypoints : 
            InGameManagers.FieldMgr.enemyWaypointsContainer.OpponentSideWaypoints;
        
        currentState = Define.EnemyState.Move;
    }

    private void Update()
    {
        if (myEnemyData == null)
            return;
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (waypoints == null)
            return;
        Vector3 targetPos = waypoints[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPos, myEnemyData.MoveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            currentWaypointIndex++;
            if(currentWaypointIndex >= waypoints.Count)
                currentWaypointIndex = 0;
        }
    }

    public void GetDamage(float damage, Define.DamageType damageType)
    {
        // TODO: 체력바에 반영
        float finalDamage = 0f;
        switch (damageType)
        {
            case Define.DamageType.Physical:
                finalDamage = damage * (100f / (100f + myEnemyData.DefensePower));  // 비선형 감쇄
                break;
            case Define.DamageType.Magical:
                finalDamage = damage;
                break;
        }

        currentHelth -= finalDamage;

        if (currentHelth <= 0)
        {
            // TODO: 쥬금 애니메이션 실행
            currentState = Define.EnemyState.Dead;
            InGameManagers.WaveMgr.OnEnemyDie(this);
        }
    }
}
