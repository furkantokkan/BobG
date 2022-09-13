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
            SetState();
        }
        else
        {
            anim.SetBool("IsRunning",false);
            SetState();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetBool("IsFire",true);
        }
        else if(Input.GetKeyDown(KeyCode.S))    
            anim.SetBool("IsFire",false);
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
