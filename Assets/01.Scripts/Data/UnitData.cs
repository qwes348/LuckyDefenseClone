using UnityEngine;
using EditorAttributes;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    [SerializeField]
    private Define.UnitGrade grade;
    [SerializeField]
    private string unitName;
    [SerializeField]
    private int attackPower;
    [SerializeField]
    private float attackSpeed;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private AssetReferenceGameObject unitPrefab;
    
    #region Properties
    public Define.UnitGrade Grade => grade;
    public string UnitName => unitName;
    public int AttackPower => attackPower;
    public float AttackSpeed => attackSpeed;
    public float AttackRagne => attackRange;
    public AssetReferenceGameObject UnitPrefab => unitPrefab;

    #endregion
}
