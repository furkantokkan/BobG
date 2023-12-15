using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieMode : MonoBehaviour
{
    public void ZombiMode()
    {
        GameManager.Instance.ZombieMode = true;
    }
    public void PVPMode()
    {
        GameManager.Instance.ZombieMode = false;
    }
    public void OnAnimationFinish()
    {
        SpawnManager.Instance.waitForSpawn = false;
        gameObject.SetActive(false);
    }
}
