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
    EffectManager effectManager;
    private float fireRateStorage;
    public Collider EnemyCollider { get; set; }
    private Vector2 direction => Joystick.Instance.direction;

    private void Awake()
    {
        fireRateStorage = fireRate;
        effectManager = transform.GetChild(0).GetComponent<EffectManager>();
    }

    private void Update()
    {
        healthBar.transform.parent.parent.LookAt(Camera.main.transform);
        Attack(bulletPoint, transform);
        DetectEnemy();
        LookAtEnemy(EnemyCollider);
        JoystickMove();
    }

    private void JoystickMove()
    {
        if (Joystick.Instance == null) return;
        transform.position += (Vector3.right * Joystick.Instance.direction.x + Vector3.forward * Joystick.Instance.direction.y) * (Time.deltaTime * playerSpeed);
        if (Joystick.Instance.active)
            rotateRoot.forward = new Vector3(Joystick.Instance.direction.x, 0f, Joystick.Instance.direction.y) * (Time.deltaTime * playerSpeed);
    }

    protected override void Attack(Transform point, Transform parent)
    {
        fireRate -= Time.deltaTime;
        if (fireRate <= 0)
        {
            base.Attack(point, parent);
            fireRate = fireRateStorage;
        }
    }
    private void DetectEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, visibleRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer == 8)
            {
                EnemyCollider = colliders[i];
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (other.GetComponent<Bullet>().sender == gameObject)
            {
                return;
            }
            Destroy(other.gameObject);
            healthBar.fillAmount -= 0.05f;
            healthBar.color = Color.Lerp(Color.green, Color.red, 1.2f - healthBar.fillAmount);
            if (healthBar.fillAmount <= 0 && GameManager.Instance.Gamestate == GameManager.GAMESTATE.Ingame)
            {
                healthBar.transform.parent.gameObject.SetActive(false);
                transform.GetChild(0).GetComponent<AnimController>().DeathAnim();
                effectManager.Death.Play();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Lava"))
        {
            effectManager.Lava.Play();
            healthBar.fillAmount -= 0.3f * Time.deltaTime;
            healthBar.color = Color.Lerp(Color.green, Color.red, 1.2f - healthBar.fillAmount);
            if (healthBar.fillAmount <= 0 && GameManager.Instance.Gamestate == GameManager.GAMESTATE.Ingame)
            {
                healthBar.transform.parent.gameObject.SetActive(false);
                transform.GetChild(0).GetComponent<AnimController>().DeathAnim();
                effectManager.Death.Play();

            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Lava"))
        {
            effectManager.Lava.Stop();
        }
    }
}
