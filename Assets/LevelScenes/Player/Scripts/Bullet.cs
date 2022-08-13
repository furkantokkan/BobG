using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject asd;

    void Update()
    {
        transform.Translate(Vector3.forward * (bulletSpeed * Time.deltaTime));
    }

    private void OnBecameInvisible()
    {
        //ObjectPool.Instance.SetPooledObject(gameObject,0);
    }
}
