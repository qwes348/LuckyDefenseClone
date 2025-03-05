using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class PoolManager
{
    [SerializeField]
    private List<Poolable> currentActivePoolables;

    private Dictionary<string, Stack<Poolable>> poolDictionary;
    private Transform poolParent;

    public void Init()
    {
        poolDictionary = new Dictionary<string, Stack<Poolable>>();
        var go = GameObject.Find("@PoolParent");
        if (go == null)
        {
            go = new GameObject("@PoolParent");
        }
        poolParent = go.transform;

        poolDictionary = new Dictionary<string, Stack<Poolable>>();
        currentActivePoolables = new List<Poolable>();
    }

    /* 프리팹 또는 같은 오브젝트를 파라미터로 받아서
     * 풀러블 오브젝트를 찾아줌
     */
    public Poolable Pop(Poolable poolObj)
    {
        // 풀에 존재한다면 찾음
        if (poolDictionary.ContainsKey(poolObj.id))
        {
            Poolable returnObj = null;
            while (poolDictionary[poolObj.id].Count > 0 && returnObj == null)
            {
                returnObj = poolDictionary[poolObj.id].Pop();
                if (returnObj.isUsing)
                {
                    returnObj = null;
                    continue;
                }

                returnObj.isUsing = true;
                returnObj.onPop?.Invoke();
            }

            if (returnObj == null)
            {
                returnObj = Object.Instantiate(poolObj.gameObject, poolParent).GetComponent<Poolable>();
                returnObj.gameObject.SetActive(false);
                returnObj.isUsing = true;
                returnObj.onPop?.Invoke();
            }

            currentActivePoolables.Add(returnObj);
            return returnObj;
        }
        // 풀에 없다면 풀을 만들고 새로생성해서 반환해줌
        else
        {
            poolDictionary.Add(poolObj.id, new Stack<Poolable>());
            Poolable clone = Object.Instantiate(poolObj, poolParent).GetComponent<Poolable>();
            clone.gameObject.SetActive(false);
            clone.isUsing = true;
            if (clone.isAutoPooling)
                clone.ResetAutoPoolingTimer();
            clone.onPop?.Invoke();

            currentActivePoolables.Add(clone);
            return clone;
        }
    }

    public Poolable Pop(string id)
    {
        if (!poolDictionary.ContainsKey(id))
        {
            Debug.LogError("ID에 해당하는 풀링된 오브젝트 없음");
            return null;
        }

        // 생성한 풀러블 오브젝트가 있다면 탐색
        if (poolDictionary[id].Count > 0)
        {
            Poolable returnObj = null;
            while (poolDictionary[id].Count > 0 && returnObj == null)
            {
                returnObj = poolDictionary[id].Pop();
                if (returnObj.isUsing)
                {
                    returnObj = null;
                    continue;
                }

                returnObj.isUsing = true;
                returnObj.onPop?.Invoke();
            }

            // 미사용중인 풀러블 오브젝트가 없다면 새로생성
            if (returnObj == null)
            {
                returnObj = Object.Instantiate(poolDictionary[id].Peek().gameObject, poolParent).GetComponent<Poolable>();
                returnObj.gameObject.SetActive(false);
                returnObj.isUsing = true;
                returnObj.onPop?.Invoke();
            }

            currentActivePoolables.Add(returnObj);
            return returnObj;
        }
        
        return null;
    }

    /// <summary>
    /// 어드레서블에서 Pop
    /// </summary>
    /// <param name="id">어드레서블 id</param>
    public async UniTask<Poolable> PopAsync(string id)
    {
        // 풀에 존재한다면 찾음
        Poolable returnObj = null;
        if (poolDictionary.ContainsKey(id))
        {
            while (poolDictionary[id].Count > 0 && returnObj == null)
            {
                returnObj = poolDictionary[id].Pop();
                if (returnObj.isUsing)
                {
                    returnObj = null;
                    continue;
                }

                returnObj.isUsing = true;
                returnObj.onPop?.Invoke();
            }

            if (returnObj != null)
            {
                currentActivePoolables.Add(returnObj);
                return returnObj;
            }
        }
        else
        {
            // 풀에 없다면 풀을 만듦
            poolDictionary.Add(id, new Stack<Poolable>());
        }
        
        // 새로생성해서 반환해줌
        returnObj = await Managers.Resource.InstantiateAsset<Poolable>(id);
        returnObj.gameObject.SetActive(false);
        returnObj.isUsing = true;
        if (returnObj.isAutoPooling)
            returnObj.ResetAutoPoolingTimer();
        returnObj.onPop?.Invoke();

        currentActivePoolables.Add(returnObj);
        return returnObj;
    }

    public void Push(Poolable poolObj)
    {
        if (!poolDictionary.ContainsKey(poolObj.id))
        {
            poolDictionary.Add(poolObj.id, new Stack<Poolable>());
        }

        poolObj.onPush?.Invoke();
        poolObj.isUsing = false;
        poolDictionary[poolObj.id].Push(poolObj);
        poolObj.gameObject.SetActive(false);
        poolObj.transform.SetParent(poolParent);

        if (currentActivePoolables.Contains(poolObj))
            currentActivePoolables.Remove(poolObj);
    }

    public void PushAllActivePoolables()
    {
        if (currentActivePoolables.Count <= 0)
            return;

        List<Poolable> copyList = new List<Poolable>();

        foreach (var p in currentActivePoolables)
            copyList.Add(p);

        foreach (var p in copyList)
            Push(p);

        currentActivePoolables.Clear();
    }
}
