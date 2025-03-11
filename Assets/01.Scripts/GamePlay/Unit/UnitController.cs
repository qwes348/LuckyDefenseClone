using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class UnitController : MonoBehaviour
{
    [SerializeField]
    private UnitData myUnitData;
    [SerializeField]
    private SpriteRenderer foothold;
    
    private Collider2D[] overlapColliders;
    private float attackTimer;
    private Animator anim;
    private GridSystem.Cell myCell;
    private bool isSelected;
    
    #region AnimParam

    private readonly int animParamAttack = Animator.StringToHash("Attack");
    private readonly int animParamAttackSpeed = Animator.StringToHash("AttackSpeed");
    #endregion
    
    #region Properties
    public UnitData MyUnitData => myUnitData;
    public GridSystem.Cell MyCell { get => myCell; set => myCell = value; }
    #endregion

    public async UniTask Init(UnitData data)
    {
        myUnitData = data;
        attackTimer = data.AttackSpeed;
        overlapColliders = new Collider2D[10];
        if(anim == null)
            anim = GetComponentInChildren<Animator>();
        anim.runtimeAnimatorController = data.Animator;
        // Debug.LogErrorFormat("data: {0}, anim: {1}", data.AttackSpeed, anim.GetFloat("AttackSpeed"));
        gameObject.name = data.UnitName;
        foothold.color = Define.UnitFootholdColorDict[myUnitData.Grade];
        anim.transform.localScale = data.Grade == Define.UnitGrade.Mythical ? Vector3.one * 2f : Vector3.one * 1.5f;    // 신화등급은 크게

        await UniTask.Yield();
        anim.SetFloat(animParamAttackSpeed, 1f / data.AttackSpeed);
    }

    private void Update()
    {
        if (myUnitData == null)
            return;
        
        attackTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (myCell == null)
            return;
        if (attackTimer > 0 || myUnitData == null)
            return;
        CheckAttackRange();
        if (overlapColliders == null || overlapColliders.Length == 0)
            return;
        
        EnemyController nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        foreach (Collider2D coll in overlapColliders)
        {
            if(coll == null)
                continue;
            float distance = Vector2.Distance(transform.position, coll.transform.position);
            if (!(distance < nearestDistance))
            {
                continue;
            }
            var enemy = coll.GetComponent<EnemyController>();
            if (enemy == null || enemy.CurrentState == Define.EnemyState.Died) // 이미 죽은 enemy인 경우
                continue;
            
            nearestDistance = distance;
            nearestEnemy = enemy;
        }
        
        if (nearestEnemy == null)
            return;
        Vector3 direction = (nearestEnemy.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.right, direction);
        if (dotProduct > 0) // 타겟이 오른쪽에있음
        {
            anim.transform.rotation = Quaternion.Euler(myUnitData.IsFilpSprite ? Vector3.up * 180f : Vector3.zero);
        }
        else // 타겟이 왼쪽에있음
        {
            anim.transform.rotation = Quaternion.Euler(myUnitData.IsFilpSprite ? Vector3.zero : Vector3.up * 180f);
        }
        
        anim.SetTrigger(animParamAttack);

        nearestEnemy.GetDamage(CalculateUpgradeDamage(), Define.DamageType.Physical);
        attackTimer = myUnitData.AttackSpeed;
    }

    private void CheckAttackRange()
    {
        for (int i = 0; i < overlapColliders.Length; i++)
        {
            overlapColliders[i] = null;
        }
        Physics2D.OverlapCircleNonAlloc(transform.position, myUnitData.AttackRagne, overlapColliders, LayerMask.GetMask("Enemy"));
    }

    private void OnDrawGizmosSelected()
    {
        if (myUnitData == null)
            return;
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, myUnitData.AttackRagne);
    }

    /// <summary>
    /// 강화 레벨에따른 공격력을 구함
    /// </summary>
    private float CalculateUpgradeDamage()
    {
        int upgradeLevel = 0;
        switch (myUnitData.Grade)
        {
            case Define.UnitGrade.Normal:
            case Define.UnitGrade.Rare:
                upgradeLevel = InGameManagers.UpgradeMgr.GetUpgradeLevel(Define.UpgradeType.NormalRare, myCell.PlayerType);
                break;
            case Define.UnitGrade.Hero:
                upgradeLevel = InGameManagers.UpgradeMgr.GetUpgradeLevel(Define.UpgradeType.Hero, myCell.PlayerType);
                break;
            case Define.UnitGrade.Mythical:
                upgradeLevel = InGameManagers.UpgradeMgr.GetUpgradeLevel(Define.UpgradeType.Mythical, myCell.PlayerType);
                break;
        }
        
        return myUnitData.AttackPower + (myUnitData.AttackPower * 0.5f * upgradeLevel);
    }
}