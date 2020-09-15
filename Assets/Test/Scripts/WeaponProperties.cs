using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WeaponProperties
{
    private static WeaponProperties weaponProperties;
    private Dictionary<string, WeaponProperty> weaponPropertyDic;
    public static WeaponProperties GetInstance()
    {
        if (weaponProperties == null)
        {
            weaponProperties = new WeaponProperties();
        }
        return weaponProperties;
    }

    /// <summary>
    /// 构造函数，读取所有武器信息
    /// </summary>
    private WeaponProperties()
    {
        weaponPropertyDic=new Dictionary<string, WeaponProperty>();
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(Application.streamingAssetsPath+"/WeaponProperties.txt");
        }
        catch
        {
            Debug.Log("file not found");
            return;
        }
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            WeaponProperty wp = new WeaponProperty();
            string[] properties = line.Split(',');
            //武器名
            wp.name = properties[0];
            //射击冷却
            wp.interval = float.Parse(properties[1]);
            //弹夹容量
            wp.ammunition = int.Parse(properties[2]);
            //初始弹量
            wp.ammoInit = int.Parse(properties[3]);
            //射击模式
            switch (properties[4])
            {
                case "FullAuto":
                    wp.shootMode = ShootMode.FullAuto;
                    break;
                case "MidAuto":
                    wp.shootMode = ShootMode.MidAuto;
                    break;
                case "Hand":
                    wp.shootMode = ShootMode.Hand;
                    break;
            }

            weaponPropertyDic.Add(wp.name, wp);
        }
    }

    /// <summary>
    /// 更据武器名字获取属性
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public WeaponProperty GetWeaponProperty(string name)
    {
        return weaponPropertyDic[name];
    }
}

public class WeaponProperty
{
    

    public string name;//武器名
    public float interval;//射击冷却间隔
    public int ammunition;//弹夹容量
    public int ammoInit;//初始弹量
    public ShootMode shootMode;//射击模式
    //伤害值
    //后座力
    //枪口特效
    //
}
