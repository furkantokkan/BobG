using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCaller : MonoBehaviour
{
    ProgressController progressController;

    private void Start()
    {
        progressController = GameObject.FindGameObjectWithTag("Player").GetComponent<ProgressController>();
    }

    public void Call()
    {
        FindObjectOfType<AirdropController>().StartCoroutine("RandomSpawn");
        transform.GetChild(0).gameObject.SetActive(false);
        UIManager.Instance.Coin = 50 * progressController.incomeLevel;
    }

    public void Money()
    {
        transform.GetChild(2).GetComponent<ParticleSystem>().Play();
    }
    private void OnCollisionEnter(Collision other)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).GetComponent<ParticleSystem>().Play();
    }
}
