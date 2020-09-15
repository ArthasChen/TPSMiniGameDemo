using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl:MonoBehaviour
{
    public static UIControl _instance;

    public Text textAmmoLeft;
    public Text textAmmoTotal;

    private void Awake()
    {
        _instance = this;
        textAmmoLeft = transform.Find("PanelAmmo/TextAmmoLeft").GetComponent<Text>();
        textAmmoTotal = transform.Find("PanelAmmo/TextAmmoTotal").GetComponent<Text>();
    }

    /// <summary>
    /// 更新弹药数量显示
    /// </summary>
    /// <param name="ammoleft"></param>
    /// <param name="ammoTotal"></param>
    public void UpdateAmmoDisplay(int ammoleft,int ammoTotal)
    {
        textAmmoLeft.text = ammoleft.ToString();
        textAmmoTotal.text = ammoTotal.ToString();
    }
}
