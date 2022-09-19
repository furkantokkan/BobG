using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    [HideInInspector]
    public Animator anim;
    public enum GunState
    {
        RunPistol,
        RunRifle
    }
    public GunState gunState;
    void Awake()
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
        if (PlayerPrefs.GetInt("PowerLevel", 0) <= 6)
            anim.SetFloat("GunState", 0);        
        else anim.SetFloat("GunState", 1);;
 
    }
}
