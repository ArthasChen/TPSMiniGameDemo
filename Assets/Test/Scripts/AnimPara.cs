using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPara : MonoBehaviour
{
    #region parameter
    public static AnimPara _instance;
    public int horizontalFloat;
    public int verticalFloat;
    public int jumpTrigger;
    public int isFallingBool;
    public int isCrouchingBool;
    public int isProningBool;
    public int shootTrigger;
    public int reloadTrigger;
    public int isHoldWeaponBool;
    public int drawPistolTrigger;
    public int drawRifleTrigger;
    #endregion

    #region State And Blend
    public int fallingState;
    public int jumpBlend;
    public int holdPistolBlend;
    #endregion

    #region Layer
    public int Base = 0;
    public int Arm = 1;
    public int Foot = 2;
    #endregion



    private void Awake()
    {
        _instance = this;
        horizontalFloat = Animator.StringToHash("horizonal");
        verticalFloat = Animator.StringToHash("vertical");
        jumpTrigger = Animator.StringToHash("jump");
        isFallingBool = Animator.StringToHash("isFalling");
        isCrouchingBool = Animator.StringToHash("isCrouching");
        isProningBool = Animator.StringToHash("isProning");
        shootTrigger = Animator.StringToHash("shoot");
        reloadTrigger = Animator.StringToHash("reload");
        isHoldWeaponBool = Animator.StringToHash("isHoldWeapon");
        drawPistolTrigger = Animator.StringToHash("drawPistol");
        drawRifleTrigger = Animator.StringToHash("drawRifle");


        fallingState = Animator.StringToHash("Falling");
        jumpBlend = Animator.StringToHash("Jump Blend");
        holdPistolBlend=Animator.StringToHash("HoldPistolBlend");

    }
}
