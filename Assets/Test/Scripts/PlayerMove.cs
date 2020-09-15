using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveState
{
    stand,
    crouch,
    prone,
    jump,
    run
}

public class PlayerMove : MonoBehaviour
{
    public Animator anim;
    public TPSCamera tpsCamera;
    private ShootControl shootControl;
    public MoveState moveState = MoveState.stand;
    public bool isFalling = false;
    public bool isMoving = false;
    public bool isCrouching = false;
    public float moveSpeed = 5;
    private Vector3 moveDir;
    private float rotateSpeed;
    [Header("正常旋转速度")]
    public float rotateSpeedNormal;
    [Header("瞄准时的旋转速度")]
    public float rotateSpeedAim;

    public float speedUpMultiplier = 1.5f;
    public float speedSlowMultiplier = 0.8f;
    [Header("弹跳力")]
    public float jumpForce = 5;
    [Header("起跳延迟时间")]
    public float jumpDelayTime;

    private float h = 0;
    private float v = 0;

    public float groundTestDepth = 0.2f;
    public bool isGrounded = false;
    [Header("下落判定时间")]
    public float fallingTime;
    private void Start()
    {
        anim = GetComponent<Animator>();
        tpsCamera = TPSCamera._instance;
        shootControl = GetComponent<ShootControl>();
        StartCoroutine(PlayerFall());
    }

    private void Update()
    {
        isGrounded = IsGrounded();

        if (isGrounded)//在空中不改变运动速度
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpKey();
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            OnCrouchKey();
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            OnSpeedUpKey();
        }
        Move();
    }

    /// <summary>
    /// 处理跳跃按钮
    /// </summary>
    private void OnJumpKey()
    {
        if (!isGrounded) return;//着地时才能跳跃
        if (moveState==MoveState.crouch)//站立时跳跃，蹲下时站起
        {
            moveState = MoveState.stand;
            anim.SetBool(AnimPara._instance.isCrouchingBool, false);
            tpsCamera.localOffsetVerticle = 0;
            //降低射击精度
            shootControl.SetTargetOffset(MoveState.stand);
        }
        //判断动画状态，防止连续跳跃
        else if(anim.GetCurrentAnimatorStateInfo(AnimPara._instance.Foot).shortNameHash!=AnimPara._instance.jumpBlend)
        {
            StopCoroutine("PlayerJump");
            StartCoroutine("PlayerJump");
        }
    }

    IEnumerator PlayerJump()
    {
        anim.SetTrigger(AnimPara._instance.jumpTrigger);
         
        //站立时先下蹲再起跳，跑动时直接起跳
        if (moveDir.z < 1f)
            yield return new WaitForSeconds(jumpDelayTime);

        GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0));
        anim.SetBool(AnimPara._instance.isFallingBool, true);
        isFalling = true;
    }

    /// <summary>
    /// 处理蹲下按钮
    /// </summary>
    private void OnCrouchKey()
    {
        if (!isGrounded) return;//只有着地时才能蹲下
        if (moveState == MoveState.crouch)//如果已经是蹲下状态就站起
        {
            OnJumpKey();
        }
        else
        {
            moveState = MoveState.crouch;
            anim.SetBool(AnimPara._instance.isCrouchingBool, true);
            //蹲下时摄像机向下偏移
            tpsCamera.localOffsetVerticle = -0.5f;
            //提高射击精度
            shootControl.SetTargetOffset(MoveState.crouch);
        }
    }

    /// <summary>
    /// 处理奔跑按钮
    /// </summary>
    private void OnSpeedUpKey()
    {
        if (!isGrounded||moveState==MoveState.crouch) return;//着地并站立时才能奔跑
        if (!tpsCamera.isAiming&& v > 0)//瞄准或倒走时不能奔跑
        {
            moveState = MoveState.run;
            //if(!shootControl.isShooting)
            //    anim.SetBool(AnimPara._instance.isHoldWeaponBool, true);
            h *= speedUpMultiplier;
            v *= speedUpMultiplier;
        }       
    }

    /// <summary>
    /// 控制位移
    /// </summary>
    private void Move()
    {
        float th = h > 0 ? h : -h, tv = v > 0 ? v : -v;
        moveDir = new Vector3(h, 0, v).normalized * (th > tv ? th : tv);
        //蹲下时减速
        moveDir *= (moveState == MoveState.crouch ? speedSlowMultiplier : 1);
        //倒走时减速
        moveDir *= v < 0 ? speedSlowMultiplier : 1;
        if(!shootControl.isFreeView)
        {
            //转向摄像机视线前方
            rotateSpeed = tpsCamera.isAiming ? rotateSpeedAim : rotateSpeedNormal;
        }
        //往后走和往前走与左右行走的动画混合相反
        anim.SetFloat("horizontal", v < 0 ? -h : h);
        anim.SetFloat("vertical", v);
        //移动
        if (v <= 0 || !IsBlocked())//如果向前走被阻碍则不移动
        {
            transform.Translate(moveDir * moveSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// 如果非自由视角，玩家朝向摄像机前方
    /// </summary>
    public void RotateTowardsCam()
    {
        if(!shootControl.isFreeView)
            RotateToDir(new Vector3(tpsCamera.transform.forward.x, 0, tpsCamera.transform.forward.z));
    }

    private void RotateToDir(Vector3 dir)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotateSpeed * Time.deltaTime);
    }


    IEnumerator PlayerFall()
    {
        float timer = 0;
        while (true)
        {
            if (!isGrounded)
            {
                timer += Time.deltaTime;
                if (timer > fallingTime&&isFalling==false)
                {
                    anim.SetBool(AnimPara._instance.isFallingBool, true);
                    isFalling = true;
                }
                    
            }
            else
            {
                timer = 0;
                if (isFalling == true)
                {
                    anim.SetBool(AnimPara._instance.isFallingBool, false);
                    isFalling = false;
                }
                    
            }
            yield return null;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position+new Vector3(0,0.1f,0),-transform.up,out hit, 0.101f,~(1<<9)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断人物前方是否有物体阻碍行走
    /// </summary>
    /// <returns></returns>
    private bool IsBlocked()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), transform.forward, out hit, GetComponent<CapsuleCollider>().radius+0.01f, ~(1 << 9))){
            print("block");
            return true;
        }
        return false;
    }
}
