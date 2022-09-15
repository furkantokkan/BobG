using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
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
        if (transform.parent.tag != "Player")
        {
            return; 
        }
        if (Joystick.Instance != null && Joystick.Instance.active)
        {
            SetRunAnim(true);
        }
        else
        {
            SetRunAnim(false);
        }
    }
    public void SetFireAnimation(bool isFire)
    {
        anim.SetBool("IsFire", isFire);
    }
    public void SetRunAnim(bool isRun)
    {
        anim.SetBool("IsRunning", isRun);
        SetState();
    }
    public void DanceAnim()
    {
        anim.SetFloat("DanceState",Random.Range(0,2));
        anim.SetTrigger("Dance");
    }
    public void DeathAnim()
    {
        anim.SetTrigger("Death");
        GameManager.Instance.Gamestate = GameManager.GAMESTATE.Empty;
    }
    void SetState()
    {
        switch (gunState)
        {
            case GunState.RunPistol:
                anim.SetFloat("GunState", 0);
                break;
            case GunState.RunRifle:
                anim.SetFloat("GunState", 1);
                break;
            default: anim.SetFloat("GunState", 0.5f);
                break;
        }
    }
}
