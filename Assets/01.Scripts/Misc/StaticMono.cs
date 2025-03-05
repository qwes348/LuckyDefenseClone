using System;
using UnityEngine;

/// <summary>
/// static이지만 싱글톤은 아닌 MonoBehaviour 클래스
/// </summary>
public abstract class StaticMono<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    /// <summary>
    /// null을 반환할 수 있음
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<T>();
            return instance;
        }
    }
    
    protected virtual void OnDestroy()
    {
        if(instance == this)
            instance = null;
    }
}
