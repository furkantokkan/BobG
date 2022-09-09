using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator anim;

    public enum GunState
    {
        RunPistol,
        RunRifle
    }
    public GunState gunState;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Joystick.Instance != null && Joystick.Instance.active)
        {
            anim.SetBool("IsRunning",true);
            RunAnim();
        }
        else
        {
            anim.SetBool("IsRunning",false);
            IdleAnim();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetBool("IsFire",true);
            if(anim.GetBool("IsRunning"))
                anim.SetLayerWeight(1,1);
            else anim.SetLayerWeight(1,0);
        }
        else if(Input.GetKeyDown(KeyCode.S))    
            anim.SetBool("IsFire",false);
    }

    void DanceAnim()
    {
        anim.SetFloat("DanceState",Random.Range(0,2));
        anim.SetTrigger("Dance");
    }
    void DeathAnim()
    {
        anim.SetTrigger("Death");
    }
    void IdleAnim()
    {
        switch (gunState)
        {
            case GunState.RunPistol:
                anim.SetFloat("IdleState", 0);
                break;
            case GunState.RunRifle:
                anim.SetFloat("IdleState", 1);
                break;
            default: anim.SetFloat("IdleState", 0.5f);
                break;
        }
    }
    void RunAnim()
    {
        switch (gunState)
        {
            case GunState.RunPistol:
                anim.SetFloat("RunState", 0.1f);
                break;
            case GunState.RunRifle:
                anim.SetFloat("RunState", 1);
                break;
        }
    }

    void FireAnim()
    {
        switch (gunState)
        {
            case GunState.RunPistol:
                anim.SetFloat("FireState", 0.1f);
                break;
            case GunState.RunRifle:
                anim.SetFloat("FireState", 1);
                break;
        }
    }
}
