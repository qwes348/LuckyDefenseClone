using EditorAttributes;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitController : MonoBehaviour
{
    [SerializeField]
    private UnitData myUnitData;
    
    private Collider2D[] overlapColliders;
    private float attackTimer;
    
    #region Properties
    public UnitData MyUnitData => myUnitData;
    #endregion

    public void Init(UnitData data)
    {
        myUnitData = data;
        attackTimer = data.AttackSpeed;
        overlapColliders = new Collider2D[10];
    }

    private void Update()
    {
        if (myUnitData == null)
            return;
        
        attackTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
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
            nearestDistance = distance;
            nearestEnemy = coll.GetComponent<EnemyController>();
        }
        
        if (nearestEnemy == null)
            return;
        
        nearestEnemy.GetDamage(myUnitData.AttackPower, Define.DamageType.Physical);
        attackTimer = myUnitData.AttackSpeed;
        
        Debug.LogFormat("{0}에게 {1}가 공격!", nearestEnemy.name, transform.name);
    }

    private void CheckAttackRange()
    {
        Physics2D.OverlapCircleNonAlloc(transform.position, myUnitData.AttackRagne, overlapColliders, LayerMask.GetMask("Enemy"));
    }

    private void OnDrawGizmosSelected()
    {
        if (myUnitData == null)
            return;
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, myUnitData.AttackRagne);
    }
}