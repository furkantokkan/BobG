using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZombieMode : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    private void Start()
    {
        startPanel.gameObject.SetActive(false);
    }
    public void ZombMode()
    {
        GameManager.Instance.ZombieMode = true;
        Invoke("OnAnimationFinish", 2f);
    }
    public void PVPMode()
    {
        GameManager.Instance.ZombieMode = false;
        Invoke("OnAnimationFinish", 2f);
    }
    public void OnAnimationFinish()
    {
        SpawnManager.Instance.waitForSpawn = false;
        gameObject.SetActive(false);
    }
}
