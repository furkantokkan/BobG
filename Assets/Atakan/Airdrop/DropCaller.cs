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
        UIManager.Instance.Coin = 50 * progressController.incomeLevel;
    }
}
