using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField]
    private Define.EnemyGrade grade;
    [SerializeField]
    private string unitName;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float maxHealth;
    [SerializeField]
    private int defensePower;
    [SerializeField]
    private int prize;
    [SerializeField]
    private AssetReferenceGameObject enemyPrefab;
    
    #region Properties
    public Define.EnemyGrade Grade => grade;
    public string UnitName => unitName;
    public float MoveSpeed => moveSpeed;
    public float MaxHealth => maxHealth;
    public int DefensePower => defensePower;
    public int Prize => prize;
    public AssetReferenceGameObject EnemyPrefab => enemyPrefab;

    #endregion
}
