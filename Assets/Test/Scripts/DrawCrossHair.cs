using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawCrossHair : MonoBehaviour
{
    public static DrawCrossHair _instance;

    [Header("最大距离")]
    public float maxDistance;
    private Vector2 maxSize;
    [Header("最小距离")]
    public float minDistance;
    private Vector2 minSize;
    [Header("缩小的速度")]
    public float narrowSpeed;
    private RectTransform rectTrans;

    // Use this for initialization
    private void Awake()
    {
        _instance = this;
        rectTrans = GetComponent<RectTransform>();
        minSize = new Vector2(minDistance, minDistance);
        maxSize = new Vector2(maxDistance, maxDistance);
        //Hide();
        StartCoroutine(Narrow());
    }

    public void Expand(float distancePlus)
    {
        if (minDistance + distancePlus < maxDistance)
            rectTrans.sizeDelta += new Vector2(distancePlus, distancePlus);
        else
            rectTrans.sizeDelta = new Vector2(maxDistance, maxDistance);
    }

    IEnumerator Narrow()
    {
        while (true)
        {
            rectTrans.sizeDelta = Vector2.Lerp(rectTrans.sizeDelta, minSize, narrowSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void Hide()
    {
        rectTrans.anchoredPosition = new Vector2(1000, 0);
    }

    public void Show()
    {
        rectTrans.anchoredPosition = Vector2.zero;
    }
}
