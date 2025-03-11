using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DamageCanvas : MonoBehaviour
{
    [SerializeField]
    private AssetReferenceGameObject damageTextPrefab;

    public async UniTaskVoid ShowDamage(Vector3 worldPosition, float damage)
    {
        var damageText = (await Managers.Pool.PopAsync(damageTextPrefab)).GetComponent<DamageText>();
        damageText.transform.SetParent(transform);
        damageText.Init(worldPosition, damage);
    }
}
