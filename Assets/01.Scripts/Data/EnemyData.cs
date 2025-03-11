using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField]
    private Define.EnemyGrade grade;
    [SerializeField]
    private int unitId;
    [SerializeField]
    private string unitName;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private int defensePower;
    [SerializeField]
    private Define.CurrencyType prizeType;
    [SerializeField]
    private int prize;
    [SerializeField]
    private AssetReferenceGameObject enemyPrefab;
    [SerializeField]
    private AnimatorOverrideController animator;
    
    #region Properties
    public Define.EnemyGrade Grade => grade;
    public int UnitId => unitId;
    public string UnitName => unitName;
    public float MoveSpeed => moveSpeed;
    public float MaxHealth => maxHealth;
    public int DefensePower => defensePower;
    public Define.CurrencyType PrizeType => prizeType;
    public int Prize => prize;
    public AssetReferenceGameObject EnemyPrefab => enemyPrefab;
    public AnimatorOverrideController Animator => animator;

    #endregion

    public void AddMaxHealth(int amount)
    {
        if (!unitId.Equals(9999))
        {
            Debug.LogError($"일반 웨이브 MaxHealth 수정 감지됨: {name}, unitId: {unitId}");
            return;
        }
        maxHealth += amount;
    }

    public void SetMaxHealth(int amount)
    {
        if (!unitId.Equals(9999))
        {
            Debug.LogError($"일반 웨이브 MaxHealth 수정 감지됨: {name}, unitId: {unitId}");
            return;
        }
        maxHealth += amount;
    }
}
