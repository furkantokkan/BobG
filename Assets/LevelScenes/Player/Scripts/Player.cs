using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Humanoid
{
    [Header("Values")]
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private float fireRate;
    [SerializeField] private Transform rotateRoot;
    [SerializeField] private float playerSpeed;
    private float fireRateStorage;
    public bool CanAttack { get; set; } = false;
    public Collider EnemyCollider { get; set; }
    private Vector2 direction => Joystick.Instance.direction;   
    
    private void Awake()
    {
        gameObject.tag = "Player";
        fireRateStorage = fireRate;
    }

    private void Update()
    {
        healthBar.transform.parent.parent.LookAt(Camera.main.transform);
        Attack(bulletPoint,transform);
        DetectEnemy();
        LookAtEnemy(EnemyCollider);
        JoystickMove();
    }
    
    private void JoystickMove()
    {
        if(Joystick.Instance == null) return;
        transform.position += (Vector3.right * Joystick.Instance.direction.x + Vector3.forward * Joystick.Instance.direction.y) * (Time.deltaTime * playerSpeed);
        if(Joystick.Instance.active)
            rotateRoot.forward = new Vector3(Joystick.Instance.direction.x, 0f, Joystick.Instance.direction.y) * (Time.deltaTime * playerSpeed);
    }

    protected override void Attack(Transform point, Transform parent)
    {
        if (CanAttack)
        {
            fireRate -= Time.deltaTime;
            if (fireRate <= 0)
            {
                base.Attack(point,parent);
                fireRate = fireRateStorage;
                CanAttack = false;
            } 
        }
    }
    
    private void DetectEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, visibleRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer == 8)
            {
                CanAttack = true;
                EnemyCollider = colliders[i];
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
            healthBar.fillAmount -= 0.05f;
            if (healthBar.fillAmount <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
