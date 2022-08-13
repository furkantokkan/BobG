using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Humanoid
{
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private float fireRate;
    private float fireRateStorage;
    public bool CanAttackPlayer { get; set; } = false;
    public Collider PlayerCollider { get; set; }

    private void Awake()
    {
        fireRateStorage = fireRate;
    }

    private void Update()
    {
        Attack(bulletPoint,transform);
        DetectEnemy();
        LookAtEnemy(PlayerCollider);
    }

    protected override void Attack(Transform point, Transform parent)
    {
        if (CanAttackPlayer)
        {
            fireRate -= Time.deltaTime;
            if (fireRate <= 0)
            {
                base.Attack(point,parent);
                fireRate = fireRateStorage;
                CanAttackPlayer = false;
            } 
        }
    }
    
    private void DetectEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, visibleRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer == 7)
            {
                CanAttackPlayer = true;
                PlayerCollider = colliders[i];
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            healthBar.fillAmount -= 0.2f;
            if (healthBar.fillAmount <= 0)
            {
                LootBoxManager.Instance.LootBoxStage();
                Destroy(gameObject);
            }
        }
    }
}
