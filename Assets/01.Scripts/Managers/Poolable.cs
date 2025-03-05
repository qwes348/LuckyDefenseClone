using System;
using UnityEngine;
using EditorAttributes;

public class Poolable : MonoBehaviour
{
    [Tooltip("어드레서블이라면 어드레서블id를 입력")]
    public string id;
    public int createCountInAwake = 0;
    public bool isAutoPooling;
    [ShowField("isAutoPooling")]
    public float autoPoolingTime;
    [ReadOnly]
    public bool isUsing = false;

    public Action onPop;
    public Action onPush;

    private float poolingTimer = 0f;

    private void Update()
    {
        if(isAutoPooling && isUsing)
        {
            if(poolingTimer < autoPoolingTime)
                poolingTimer += Time.deltaTime;
            else
            {
                poolingTimer = 0f;
                Managers.Pool.Push(this);
            }
        }
    }

    public void ResetAutoPoolingTimer()
    {
        poolingTimer = 0f;
    }    
}
