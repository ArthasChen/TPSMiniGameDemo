using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShootMode { FullAuto,MidAuto,Hand}

public class ShootControl : MonoBehaviour
{
    public TPSCamera tpsCamera;//控制相机视角的脚本
    public Camera cam;//摄像机
    public float range;//射线距离
    private float offsetDis;//摄像机与玩家的距离
    public Transform targetTrans;//瞄准的物体
    public Vector3 targetPos;//目标位置
    public Vector3 randomTarget;//偏移后的目标位置
    private float angleX;
    private float angleY;
    private AimIK aimIK;//对应的final ik组件
    private MoveState moveState;
    [Header("是否打算射击")]
    public bool isWantShoot = false;
    [Header("是否准备好射击")]
    public bool isReadyShoot = false;
    //是否是第一次射击
    private bool isFirstShoot = false;
    private float shootTimer = 0;

    [Header("瞄准速度")]
    public float aimSpeed;
    public bool isFreeView = false;

    private Animator anim;

    #region properties of weapon
    [Header("射击间隔")]
    public float interval;
    private float timer = 0;
    [Header("当前武器")]
    public Transform weapon;
    [Header("所有武器")]
    public GameObject[] weapons;
    private Transform firePos;
    [Header("子弹速度")]
    public float bulletSpeed = 50;

    private ParticleSystem shotEfx;//开火特效
    //private Light muzzleLight;//开火光照

    [Header("开枪音效")]
    public AudioClip[] fireAudios;

    private float targetOffset; //射击精度偏移量
    [Header("站立时射击精度偏移量")]
    public float targetOffsetNomal;
    [Header("下蹲时射击精度偏移量")]
    public float targetOffsetCrouch;
    [Header("准星扩大量")]
    public float crossHairPlus;
    [Header("水平后坐力旋转角度最大值")]
    public float backForceXMax;
    [Header("垂直后坐力旋转角度最大值")]
    public float backForceYMax;
    private float backForceXAdd;//后座力导致的摄像机水平旋转的角度
    [Header("弹夹剩余子弹")]
    public int ammoLeft;
    [Header("总共剩余子弹")]
    public int ammoTotal;
    [Header("弹夹容量")]
    public int ammuniton;
    [Header("射击模式")]
    public ShootMode shootMode = ShootMode.FullAuto;
    [Header("是否正在换弹")]
    public bool isReloading = false;
    [Header("换弹的手")]
    public Transform reloadHand;

    static int counts = 0;
    #endregion
    private void Awake()
    {
        tpsCamera = GameObject.Find("TPSCameraParent").GetComponent<TPSCamera>();//获取相机的父物体
        cam = tpsCamera.GetComponentInChildren<Camera>();//相机为tpsCamera的子物体
        aimIK = GetComponent<AimIK>();//获取AimIk组件
        targetTrans = aimIK.solver.target;
        offsetDis = Vector3.Distance(transform.position, cam.transform.position);//初始化offsetDis
        anim = GetComponent<Animator>();
        //初始化射击偏移量
        SetTargetOffset(MoveState.stand);
        //隐藏所有武器
        foreach(GameObject w in weapons)
        {
            w.SetActive(false);
        }
    }
    
    private void Start()
    {
        //设置初始武器
        SwithWeapon("Pistol");
        moveState = GetComponent<PlayerMove>().moveState;
        StartCoroutine(HoldWeapon());
    }

    private void InitWeapon()
    {
        firePos = weapon.Find("FirePos");
        shotEfx = weapon.Find("ShotEfx").GetComponent<ParticleSystem>();
        aimIK.solver.transform = firePos;
    }

    public void SetTargetOffset(MoveState state)
    {
        switch (state)
        {
            case MoveState.stand:
                targetOffset = targetOffsetNomal;
                break;
            case MoveState.crouch:
                targetOffset = targetOffsetCrouch;
                break;
        }
    }

    private void Update()
    {
        SetTarget();//设置瞄准的目标位置
        //Debug.DrawRay(firePos.position, firePos.forward, Color.yellow);
        if (isFirstShoot)
        {
            Shoot();
            isFirstShoot = false;
        }

        if (timer < interval)
            timer += Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            if (isReadyShoot)//如果是收枪状态
                shootTimer = 4f;
            else
            {
                if (!isWantShoot)
                {
                    shootTimer = 3.7f;
                    //Invoke("Shoot", 1f);
                }
                    
                isWantShoot = true;
            }

            if (timer < interval || isReloading ||!isReadyShoot)
                return;
            else
            {
                Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R)&& ammoTotal > 0 && ammoLeft != ammuniton)
        {
            StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (isFreeView&&isReadyShoot)//如果原来是自由视角则关闭自由视角和开启ik
                EnableIk();
            else
                DisableIk();
            isFreeView = !isFreeView;
        }

        //背包快捷键
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwithWeapon(GetComponent<PlayerProperty>().GetBag(0).name);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwithWeapon(GetComponent<PlayerProperty>().GetBag(1).name);
        }
    }

    /// <summary>
    /// 设置瞄准的目标
    /// 从摄像机位置向摄像机正方向发射射线（即从屏幕视口中心发出）
    /// 射线的长度=range，可以近似设为子弹的射程
    /// 若射线打到非玩家的物体则将该物体设为目标
    /// 若射线没有打到物体则将目标设为射线的终点
    /// </summary>
    public void SetTarget()
    {
        //从摄像机位置向摄像机正方向发射射线（即从屏幕视口中心发出）
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range,~(1<<9)))
        {
            //若射线打到非玩家的物体则将该物体设为目标
            //我这里并没有进行判断该物体是否是玩家，因为我设置的玩家位于屏幕的偏左下位置，射线不会穿过玩家
            //需要的话，可以给玩家设定layer,然后让射线屏蔽这个layer
            targetPos = hit.point;

            //print("True " + counts.ToString());
            counts++;
        }
        else
        {
            //若射线没有打到物体则将目标设为射线的终点
            targetPos = cam.transform.position + (cam.transform.forward * range);

            //print("False " + counts.ToString());
            counts++;
        }
        //画出射线便于观察（不会显示在game中）
        Debug.DrawRay(cam.transform.position, cam.transform.forward * range, Color.green);

        if(!isFreeView)
            SetAimIKTarget();//设置瞄准目标
       
    }

    /// <summary>
    /// 更新AimIK的target的位置
    /// </summary>
    private void SetAimIKTarget()
    {
        //将AimIK的target位置设为之前射线检测到的位置
        targetTrans.position = Vector3.Slerp(targetTrans.position, targetPos,aimSpeed*Time.deltaTime);
    }

    /// <summary>
    /// 射击
    /// </summary>
    private void Shoot()
    {
        //子弹数更新
        if (ammoLeft > 0)
        {
            print("left");
            ammoLeft--;
            UIControl._instance.UpdateAmmoDisplay(ammoLeft, ammoTotal);
            timer = 0;
        }
        else if (ammoTotal > 0)
        {
            StartCoroutine(Reload());
            return;
        } 
        else
            return;

        // 计算血量
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, ~(1 << 9)))
        {
            //print(hit.collider.ToString());

            //Health health = hit.collider.GetComponent<Health>();
            Health health = hit.collider.GetComponentInChildren<Health>();
            print("Hit people success");
            // 如果组件存在
            if (health != null)
            {
                //print("health people success");
                // 调用组件的Damage函数计算伤害
                health.Damage(1);
                print("Damgea 1");
            }
            else
            {
                //print("health people faile");

            }
        }



        //动画播放
        print("trigger");
        anim.SetTrigger(AnimPara._instance.shootTrigger);
        //随机位置
        angleX = Random.Range(-targetOffset, targetOffset);
        angleY = Random.Range(0, targetOffset);
        randomTarget = targetPos+new Vector3(0,angleY,0)+firePos.right*angleX;
        //模拟后坐力
        BackForce();
        //生成子弹
        //GameObject bulletGo = ObjectPool.GetInstance().GetObj(ObjType.Bullet);
        //bulletGo.GetComponent<Bullet>().Init(firePos);
        //枪口特效
        StartCoroutine(ShowShootEfx());
        //准星变大
        DrawCrossHair._instance.Expand(crossHairPlus);
    }

    /// <summary>
    /// 模拟后坐力，限制摄像机的水平旋转
    /// </summary>
    private void BackForce()
    { 
        if (tpsCamera.axisX!=0)//玩家水平拉枪时重置angleXTotal
            backForceXAdd = 0;
        float forceX = 0;
        if (backForceXAdd>0)
            forceX = Random.Range(-backForceXMax, backForceXMax - backForceXAdd);
        else
            forceX = Random.Range(-backForceXMax-backForceXAdd, backForceXMax);
        backForceXAdd += forceX;
        float forceY = Random.Range(0, backForceYMax);
        tpsCamera.SetPosAndRot(forceX,forceY);
    }

    IEnumerator ShowShootEfx()
    {
        //开火特效
        shotEfx.Play();
        //开火音效
        AudioSource.PlayClipAtPoint(fireAudios[0], firePos.position);
        yield return new WaitForSeconds(0.05f);
    }

    IEnumerator Reload()
    {
        //公共操作
        isReloading = true;
        anim.SetTrigger(AnimPara._instance.reloadTrigger);
        Transform mag = weapon.Find("Mag");
        Vector3 pos = mag.localPosition;
        Quaternion rot = mag.localRotation;
        switch (weapon.name)
        {
            case "Pistol":
                mag.SetParent(reloadHand);
                mag.localPosition = Vector3.zero;
                yield return new WaitForSeconds(0.4f);
                mag.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.6f);
                mag.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.4f);
                mag.SetParent(weapon);
                mag.localRotation = rot;
                mag.localPosition = pos;
                break;
            case "Rifle":
                yield return new WaitForSeconds(1f);
                break;

        }
        //公共操作
        ammoTotal += ammoLeft;
        ammoLeft = ammoTotal > ammuniton ? ammuniton : ammoTotal;
        ammoTotal -= ammoTotal > ammuniton ? ammuniton : ammoTotal;
        yield return new WaitForSeconds(0.3f);
        isReloading = false;
        //更新显示
        UIControl._instance.UpdateAmmoDisplay(ammoLeft, ammoTotal);
        StopCoroutine("Reload");
    }

    public void SwithWeapon(string weaponName)
    {
        //动作播放
        DisableIk();
        Invoke("EnableIk", 0.5f);
        switch (weaponName)
        {
            case "Pistol":
                anim.SetTrigger(AnimPara._instance.drawPistolTrigger);
                break;
            case "Rifle":
                anim.SetTrigger(AnimPara._instance.drawRifleTrigger);
                break;
        }
        anim.SetBool(AnimPara._instance.isHoldWeaponBool, false);
        isReadyShoot = true;
        shootTimer = 4f;
        //更新相关变量
        if (weapon != null) weapon.gameObject.SetActive(false);
        foreach(GameObject w in weapons)
        {
            if (w.name == weaponName)
            {
                w.SetActive(true);
                weapon = w.transform;
                break;
            }
        }
        InitWeapon();
        //更新属性
        WeaponProperty wp = WeaponProperties.GetInstance().GetWeaponProperty(weaponName);
        ammuniton = wp.ammunition;
        ammoLeft = wp.ammunition;
        ammoTotal = wp.ammoInit - ammuniton;
        interval = wp.interval;
        shootMode = wp.shootMode;
        //更新显示
        UIControl._instance.UpdateAmmoDisplay(ammoLeft, ammoTotal);

        //...
        
    }
    
    IEnumerator HoldWeapon()
    {
        //anim.SetBool(AnimPara._instance.isHoldWeaponBool, true);//初始为收枪
        while (true)
        {
            if (isWantShoot)//正在抬枪过程中，暂停收枪倒计时
            {
                shootTimer += Time.deltaTime;
                anim.SetBool(AnimPara._instance.isHoldWeaponBool, false);//抬枪动作
                EnableIk();//开启ik
            }
            else if(shootTimer > 0)//收枪倒计时
                shootTimer -= Time.deltaTime;

            if (shootTimer >= 4f && !isReadyShoot)//抬枪完成
            {
                isWantShoot = false;
                isReadyShoot = true;
                isFirstShoot = true;
            }
            
            if(shootTimer<=0f&&isReadyShoot)//收枪倒计时结束，收枪
            {
                anim.SetBool(AnimPara._instance.isHoldWeaponBool, true);//收枪动作
                DisableIk();//关闭ik
                isReadyShoot = false;
            }
            yield return null;
        }
    }

    private void DisableIk()
    {
        aimIK.Disable();
    }

    private void EnableIk() 
    {
        aimIK.enabled = true;
    }
}