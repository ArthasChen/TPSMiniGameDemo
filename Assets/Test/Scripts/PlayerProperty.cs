using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储与人物有关的属性，包括：
/// 1.背包
/// 2.血量
/// 3.。。。
/// </summary>
public class PlayerProperty : MonoBehaviour
{
    public GameObject[] bag;//背包

    private void Start()
    {
        InitBag();
    }

    /// <summary>
    /// 初始化背包
    /// </summary>
    public void InitBag()
    {

    }

    /// <summary>
    /// 设置背包内容
    /// </summary>
    /// <param name="go"></param>
    /// <param name="index"></param>
    public void SetBag(GameObject item,int index)
    {
        bag[index] = item;
    }

    /// <summary>
    /// 获取背包内容
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GetBag(int index)
    {
        return bag[index];
    }
}
