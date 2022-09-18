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
    [Range(0, 1f)]
    [SerializeField] private float animationFirePosition = 1f;
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private float fireRate;
    [SerializeField] private Transform rotateRoot;
    [SerializeField] private float playerSpeed;
    EffectManager effectManager;
    private float fireRateStorage;

    float distanceToTarget = Mathf.Infinity;

    float enemyDistance;

    public Collider EnemyCollider { get; set; }
    private Vector2 direction => Joystick.Instance.direction;

    private bool onAttack = false;

    private ProgressController progressController;

    [SerializeField] private AnimController meshAnimator;

    private float nextAttackTime = 0.0f;

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
        if (GameManager.Instance.Gamestate != GameManager.GAMESTATE.Ingame || UIManager.Instance.upgradePanel.activeInHierarchy)
        {
            return;
        }
        DetectEnemy();
        JoystickMove();
        if (EnemyCollider != null && Vector3.Distance(EnemyCollider.transform.position, transform.position) <= visibleRadius)
        {
            if (Joystick.Instance.direction == Vector2.zero)
            {
                meshAnimator.SetFireAnimation(true);
                StartCoroutine(AttackRoutine(bulletPoint, transform));
                LookAtEnemy(EnemyCollider);
            }
            else
            {
                onAttack = false;
                nextAttackTime = 0f;
                meshAnimator.SetFireAnimation(false);
            }
        }
        else
        {
            onAttack = false;
            nextAttackTime = 0f;
            meshAnimator.SetFireAnimation(false);
        }
    }
    public void UpdateStats()
    {
        currentDamage = progressController.GetStat(Stat.POWER);
        currentHealth = progressController.GetStat(Stat.HEALTH);
        currentArmor = progressController.GetStat(Stat.ARMOR);
        currentSpeed = progressController.GetStat(Stat.SPEED);
        currentIncome = progressController.GetStat(Stat.INCOME);
    }
    private IEnumerator AttackRoutine(Transform point, Transform parrent)
    {
        if (onAttack)
        {
            yield break;
        }
        onAttack = true;
        Debug.Log("Process Started Player");
        yield return new WaitUntil(() => !meshAnimator.anim.IsInTransition(0) &&
        meshAnimator.anim.GetCurrentAnimatorStateInfo(0).IsTag("AttackAnim") && meshAnimator.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= nextAttackTime);
        nextAttackTime += animationFirePosition;
        Debug.Log("Is Attacking Player");
        yield return new WaitUntil(() => !meshAnimator.anim.IsInTransition(0) &&
            meshAnimator.anim.GetCurrentAnimatorStateInfo(0).IsTag("AttackAnim") && meshAnimator.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= nextAttackTime + (nextAttackTime - 1f));
        if (EnemyCollider.enabled == false)
        {
            EnemyCollider = null;
            onAttack = false;
            yield break;
        }
        Attack(point, transform, currentDamage);
        onAttack = false;
    }
    private void JoystickMove()
    {
        if (Joystick.Instance == null) return;
        transform.position += (Vector3.right * Joystick.Instance.direction.x + Vector3.forward * Joystick.Instance.direction.y) * (Time.deltaTime * playerSpeed);
        if (Joystick.Instance.active)
            rotateRoot.forward = new Vector3(Joystick.Instance.direction.x, 0f, Joystick.Instance.direction.y) * (Time.deltaTime * playerSpeed);
    }

    protected override void Attack(Transform point, Transform parent, int newDamage)
    {
        base.Attack(point, parent, newDamage);
        fireRate = fireRateStorage;
    }
    private void DetectEnemy()
    {
        enemyDistance = float.MaxValue;

        List<Transform> targetList = new List<Transform>();
        foreach (Enemy item in GameManager.Instance.allEnemiesList)
        {
            if (item == this)
            {
                continue;
            }
            targetList.Add(item.transform);
        }


        foreach (Transform item in targetList)
        {
            float distanceToEnemy = Vector3.Distance(item.transform.position, this.transform.position);

            if (distanceToEnemy < enemyDistance)
            {
                enemyDistance = distanceToEnemy;

                RaycastHit hit;
                Vector3 fromPosition = transform.position + Vector3.up * 2;
                Vector3 toPosition = item.transform.position + Vector3.up * 2;
                Vector3 direction = toPosition - fromPosition;

                Debug.DrawRay(fromPosition, direction, Color.red);

                if (Physics.Raycast(fromPosition, direction, out hit))
                {
                    if (hit.collider.gameObject.tag != "Enemy")
                    {
                        print("Not Enemy");
                        continue;
                    }
                    else
                    {
                        EnemyCollider = item.GetComponent<Collider>();
                    }
                }

            }
        }

        //Collider[] colliders = Physics.OverlapSphere(transform.position, visibleRadius);
        //Debug.DrawRay(transform.position, visibleRadius * transform.forward, Color.red);
        //for (int i = 0; i < colliders.Length; i++)
        //{
        //    if (colliders[i].gameObject.layer == 8)
        //    {
        //        EnemyCollider = colliders[i];
        //    }
        //}
        if (EnemyCollider == null) return;
        var Circle = EnemyCollider.GetComponentInChildren<EffectManager>().Circle;
        Circle.SetActive(true);
        Circle.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (other.GetComponent<Bullet>().sender == gameObject)
            {
                return;
            }
            healthBar.fillAmount -= (float)(other.GetComponent<Bullet>().bulletDamage - currentArmor < 5 ? 5 :
                other.GetComponent<Bullet>().bulletDamage - currentArmor) / maxHealth;
            healthBar.color = Color.Lerp(Color.green, Color.red, 1.2f - healthBar.fillAmount);
            Destroy(other.gameObject);
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
