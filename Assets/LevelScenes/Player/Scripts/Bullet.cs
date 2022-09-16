using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;

    public GameObject sender;

    void Update()
    {
        transform.Translate(Vector3.up * (bulletSpeed * Time.deltaTime));
    }
}
