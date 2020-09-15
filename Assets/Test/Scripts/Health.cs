using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    Animator ani;

    private void Awake()
    {
        ani= GetComponent<Animator>();
        //ani= GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 总血量
    public int currentHealth = 3;

    public void Damage(int damageAmount)
    {
        // 调用Damage时总血量要减去伤害值
        currentHealth -= damageAmount;

        // 检测总血量是否小于等于0 
        if (currentHealth <= 0)
        {
            // 隐藏对象
            //gameObject.SetActive(false);
            Die();
        }
    }


    private void Die()
    {
        ani.SetBool("isDead", transform);
    }
}
