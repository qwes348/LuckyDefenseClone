using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class UnitController : MonoBehaviour
{
    [SerializeField]
    private UnitData myUnitData;
    
    private Collider2D[] overlapColliders;
    private float attackTimer;
    private Animator anim;
    private GridSystem.Cell myCell;
    private bool isSelected;
    
    #region AnimParam

    private readonly int animParamAttack = Animator.StringToHash("Attack");
    #endregion
    
    #region Properties
    public UnitData MyUnitData => myUnitData;
    public GridSystem.Cell MyCell { get; set; }
    #endregion

    public void Init(UnitData data)
    {
        myUnitData = data;
        attackTimer = data.AttackSpeed;
        overlapColliders = new Collider2D[10];
        if(anim == null)
            anim = GetComponentInChildren<Animator>();
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
            var enemy = coll.GetComponent<EnemyController>();
            if (enemy == null || enemy.CurrentState == Define.EnemyState.Died) // 이미 죽은 enemy인 경우
                continue;
            
            nearestDistance = distance;
            nearestEnemy = enemy;
        }
        
        if (nearestEnemy == null)
            return;
        
        anim.SetTrigger(animParamAttack);
        nearestEnemy.GetDamage(myUnitData.AttackPower, Define.DamageType.Physical);
        attackTimer = myUnitData.AttackSpeed;
        
        Debug.LogFormat("{0}에게 {1}가 공격!", nearestEnemy.name, transform.name);
    }

    private void CheckAttackRange()
    {
        Physics2D.OverlapCircleNonAlloc(transform.position, myUnitData.AttackRagne, overlapColliders, LayerMask.GetMask("Enemy"));
    }

    public void SelectUnit()
    {
        
    }

    public void DeselectUnit()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        if (myUnitData == null)
            return;
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, myUnitData.AttackRagne);
    }
}