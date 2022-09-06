using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCaller : MonoBehaviour
{
    public void Call()
    {
        FindObjectOfType<AirdropController>().StartCoroutine("RandomSpawn");
    }
}
