using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecyclableObj : MonoBehaviour
{
    [Header("回收延时")]
    public float destroyDelay = 3f;
    [Header("物体类型")]
    public ObjType objType;

    private void OnEnable()
    {
        StartCoroutine(RecycleObj());
    }

    public void StopRecycle()
    {
        StopCoroutine(RecycleObj());
    }

    IEnumerator RecycleObj()
    {
        yield return new WaitForSeconds(destroyDelay);
        ObjectPool.GetInstance().RecycleObj(this.gameObject, objType);
    }
}
