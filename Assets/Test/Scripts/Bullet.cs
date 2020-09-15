using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("子弹速度")]
    public float velocity;
    [Header("击中特效")]
    public GameObject hitEfx;
    [Header("弹孔特效")]
    public GameObject bulletHole;
    [Header("子弹停留时间")]
    public float destroyDelay;
    //是否已初始化
    private bool isInitialized = false;
    [Header("角速度")]
    public float angularSpeed;
    private Rigidbody rigid;
    private void Update()
    {
        Vector3 angular = rigid.velocity;
        transform.rotation = Quaternion.LookRotation(angular);
    }

    private void OnEnable()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(RecycleObj());
        isInitialized = false;
    }

    IEnumerator RecycleObj()
    {
        yield return new WaitForSeconds(destroyDelay);
        ObjectPool.GetInstance().RecycleObj(this.gameObject, ObjType.Bullet);
    }

    public void Init(Transform firePos)
    {
        transform.position = firePos.position;
        transform.rotation = firePos.rotation;
        GetComponent<Rigidbody>().velocity = transform.forward * velocity;
        isInitialized = true;
    }

    //碰撞检测
    private void OnCollisionEnter(Collision collision)
    {
        if (!isInitialized) return;
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.forward,out hit, 3))
        {
            //击中特效
            GameObject hitEfx = ObjectPool.GetInstance().GetObj(ObjType.HitEfx);
            hitEfx.transform.position = hit.point;
            hitEfx.transform.LookAt(transform);

            //弹孔特效
            bulletHole = ObjectPool.GetInstance().GetObj(ObjType.BulletHole);
            bulletHole.transform.position = hit.point;
            bulletHole.transform.rotation = Quaternion.LookRotation(-hit.normal);
            //回收
            StopCoroutine(RecycleObj());
            ObjectPool.GetInstance().RecycleObj(this.gameObject, ObjType.Bullet);
        }
    }
}
