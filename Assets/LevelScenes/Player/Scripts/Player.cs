using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Humanoid
{
    [Header("Stats")]
    [SerializeField] int currentDamage = 2;
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;
    [SerializeField] int currentSpeed;
    [SerializeField] int maxSpeed;
    [SerializeField] int currentArmor;
    [SerializeField] int maxArmor;
    [SerializeField] int currentIncome;
    [Header("Values")]
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private float fireRate;
    [SerializeField] private Transform rotateRoot;
    [SerializeField] private float playerSpeed;
    EffectManager effectManager;
    private float fireRateStorage;
    public Collider EnemyCollider { get; set; }
    private Vector2 direction => Joystick.Instance.direction;

    private ProgressController progressController;

    private void Awake()
    {
        fireRateStorage = fireRate;
        effectManager = transform.GetChild(0).GetComponent<EffectManager>();
        progressController = GetComponent<ProgressController>();
    }
    private void Start()
    {
        maxHealth = progressController.GetStat(Stat.HEALTH);
        maxArmor = progressController.GetStat(Stat.ARMOR);
        maxSpeed = progressController.GetStat(Stat.SPEED);

        UpdateStats();
    }

    private void Update()
    {
        Attack(bulletPoint, transform);
        DetectEnemy();
        LookAtEnemy(EnemyCollider);
        JoystickMove();
    }
    public void UpdateStats()
    {
        currentDamage = progressController.GetStat(Stat.POWER);
        currentHealth = progressController.GetStat(Stat.HEALTH);
        currentArmor = progressController.GetStat(Stat.ARMOR);
        currentSpeed = progressController.GetStat(Stat.SPEED);
        currentIncome = progressController.GetStat(Stat.INCOME);
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
