using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class ResourceManager
{
    private Dictionary<string, AsyncOperationHandle> assetHandles = new Dictionary<string, AsyncOperationHandle>();
    private Dictionary<string, AsyncOperationHandle> labelHandles = new Dictionary<string, AsyncOperationHandle>();
    private List<AsyncOperationHandle<GameObject>> objectHandles = new List<AsyncOperationHandle<GameObject>>();
    
    /// <summary>
    /// key를 사용한 에셋 로드
    /// </summary>
    public async UniTask<T> LoadAsset<T>(string key)
    {
        if (assetHandles.TryGetValue(key, out AsyncOperationHandle value))
        {
            return (T)value.Result;
        }
        
        var handle = Addressables.LoadAssetAsync<T>(key);
        assetHandles.Add(key, handle);
        var asset = await handle.Task;
        
        return asset;
    }

    /// <summary>
    /// 레퍼런스를 사용한 에셋 로드
    /// </summary>
    public async UniTask<T> LoadAsset<T>(AssetReference reference)
    {
        if (assetHandles.TryGetValue(reference.AssetGUID, out AsyncOperationHandle value))
        {
            return (T)value.Result;
        }
        
        var handle = Addressables.LoadAssetAsync<T>(reference);
        assetHandles.Add(reference.AssetGUID, handle);
        var asset = await handle.Task;
        
        return asset;
    }

    /// <summary>
    /// 키를 사용한 오브젝트 생성
    /// </summary>
    public async UniTask<T> InstantiateAsset<T>(string key, Transform parent = null)
    {
        var handle = Addressables.InstantiateAsync(key, parent);
        objectHandles.Add(handle);
        var go = await handle.Task;
        return go.GetComponent<T>();
    }

    /// <summary>
    /// 레퍼런스를 사용한 오브젝트 생성
    /// </summary>
    public async UniTask<T> InstantiateAsset<T>(AssetReference reference, Transform parent = null)
    {
        var handle = Addressables.InstantiateAsync(reference.RuntimeKey, parent);
        objectHandles.Add(handle);
        var go = await handle.Task;
        return go.GetComponent<T>();
    }

    /// <summary>
    /// label을 사용한 에셋들 로드
    /// </summary>
    public async UniTask<List<T>> LoadAssetsByLabel<T>(string label)
    {
        var handle = Addressables.LoadAssetsAsync<T>(label, null);
        labelHandles[label] = handle;
        var assets = await handle.Task;
        return assets.ToList();
    }
    
    // 특정 Key 에셋 릴리즈
    public void ReleaseAssetByKey(string key)
    {
        if (assetHandles.TryGetValue(key, out var handle))
        {
            if (handle.IsValid())
                Addressables.Release(handle);
            assetHandles.Remove(key);
        }
    }

    // 특정 Label 에셋 릴리즈
    public void ReleaseAssetsByLabel(string label)
    {
        if (labelHandles.TryGetValue(label, out var handle))
        {
            if (handle.IsValid())
                Addressables.Release(handle);
            labelHandles.Remove(label);
        }
    }

    /// <summary>
    /// 모든 에셋들 릴리즈
    /// </summary>
    public void ReleaseAll()
    {
        // 에셋
        foreach (var handle in assetHandles.Values)
        {
            if(handle.IsValid())
                Addressables.Release(handle);
        }
        assetHandles.Clear();
        
        // 라벨 에셋
        foreach (var handle in labelHandles.Values)
        {
            if(handle.IsValid())
                Addressables.Release(handle);
        }
        labelHandles.Clear();
        
        // 인스턴스
        foreach (var handle in objectHandles)
        {
            if(handle.IsValid())
                Addressables.ReleaseInstance(handle);
        }
        objectHandles.Clear();
    }
}
