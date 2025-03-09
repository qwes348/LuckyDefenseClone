using Cysharp.Threading.Tasks;
using EditorAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
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
    private Animator anim;
    
    #region AnimParam
    private readonly int animParamMoveX = Animator.StringToHash("MoveX");
    private readonly int animParamMoveY = Animator.StringToHash("MoveY");
    private readonly int animStateDeath = Animator.StringToHash("Death");
    #endregion

    #region Actions

    public Action<float> onHealthChanged;
    public Action onDied;

  #endregion

    #region Properties

    public EnemyData MyEnemyData => myEnemyData;
    public Define.EnemyState CurrentState => currentState;
    public Transform BodyTransform => anim.transform;
    public float CurrentHelth => currentHelth;

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
        if(anim == null)
            anim = GetComponentInChildren<Animator>();
        anim.runtimeAnimatorController = newData.Animator;
        anim.transform.localScale = newData.Grade == Define.EnemyGrade.Normal ? Vector3.one : Vector3.one * 2f; // 보스는 크게
        GetComponent<Collider2D>().enabled = true;
    }

    private void Update()
    {
        if (myEnemyData == null || CurrentState != Define.EnemyState.Move)
            return;
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        if (waypoints == null)
            return;
        Vector3 targetPos = waypoints[currentWaypointIndex];
        Vector2 direction = (targetPos - transform.position).normalized;
        anim.SetFloat(animParamMoveX, direction.x);
        anim.SetFloat(animParamMoveY, direction.y);
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
        if (currentState != Define.EnemyState.Move)
            return;
        if (myEnemyData == null)
            return;
        
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
        onHealthChanged?.Invoke(currentHelth);

        if (currentHelth <= 0)
        {
            OnDied().Forget();
        }
    }

    private async UniTaskVoid OnDied()
    {
        currentState = Define.EnemyState.Died;
        GetComponent<Collider2D>().enabled = false;
        anim.Play(animStateDeath);
        var animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        await UniTask.Delay(TimeSpan.FromSeconds(animLength));
        onDied?.Invoke();
        Clear();
        InGameManagers.WaveMgr.OnEnemyDie(this);
    }

    private void Clear()
    {
        onDied = null;
        onHealthChanged = null;
    }
}
