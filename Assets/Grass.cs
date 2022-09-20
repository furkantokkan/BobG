using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    public Transform PlayerTrm;
    public Material MaterialRef;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MaterialRef.SetVector("_PlayerPosition", PlayerTrm.position);
    }
}
