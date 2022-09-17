using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State
{
    Search,
    Chase,
    GetBack,
    MoveForward,
    MoveRight,
    MoveLeft,
    Fire,
}

public class Enemy : Humanoid
{
    [Header("Stats")]
    [SerializeField] int currentDamage = 2;
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;
    [SerializeField] int currentSpeed;
    [SerializeField] int maxSpeed;
    [SerializeField] int currentArmor;
    [SerializeField] int maxArmor;
    [Header("Settings")]
    [SerializeField] private Transform bulletPoint;
    [Range(0, 1f)]
    [SerializeField] private float animationFirePosition = 1f;
    [SerializeField] private float fireAnimationSpeed = 1f;
    [SerializeField] float attackRange = 8f;
    [SerializeField] float turnSpeed = 15f;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float patrolRadius = 3f;
    [SerializeField] float patrolWaitTime = 2f;
    [SerializeField] float patrolSpeed = 4f;
    [SerializeField] float chaseSpeed = 5f;
    [SerializeField] float tacticSpeed = 5.5f;
    [SerializeField] float tacticWaitTime = 5f;
    [SerializeField] AnimatorOverrideController AnimatorOverrideController;

    [SerializeField] private AnimController meshAnimator;

    private ProgressController progressController;

    float distanceToTarget = Mathf.Infinity;

    private float tacticCounter;

    private bool isSearched = false;

    private float fireRateStorage;
    public Collider PlayerCollider { get; set; }

    public State currentState = State.Search;

    private NavMeshAgent agent;

    float enemyDistance;

    private Transform target;

    private bool tacticExecute = false;

    private bool onAttackProcess = false;

    private float nextAttackTime = 0.0f;
    
    EffectManager effectManager;

    private void Awake()
    {
        progressController = GetComponent<ProgressController>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        fireRateStorage = animationFirePosition;
        tacticCounter = tacticWaitTime;
        effectManager = transform.GetChild(0).GetComponent<EffectManager>();
    }
    private void Start()
    {
        UpdateStats();
        SetStats();
    }

    private void Update()
    {
        StateSelector();
        StateExecute();
        healthBar.transform.parent.LookAt(Camera.main.transform);
    }

    private void OnEnable()
    {
        GameManager.Instance.allEnemiesList.Add(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.allEnemiesList.Remove(this);
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
            healthBar.fillAmount -= (float)currentDamage / maxHealth;
            healthBar.color = Color.Lerp(Color.green, Color.red, 1.2f - healthBar.fillAmount);
            if (healthBar.fillAmount <= 0)
            {
                healthBar.transform.parent.gameObject.SetActive(false);
                transform.GetChild(0).GetComponent<AnimController>().anim.SetTrigger("Death");
                effectManager.Death.Play();
                UIManager.Instance.Coin += 5 * progressController.incomeLevel;
                GetComponent<Enemy>().enabled = false;
                GetComponent<CapsuleCollider>().enabled = false;
            }
        }
    }
    public void UpdateStats()
    {
        maxHealth = progressController.GetStat(Stat.HEALTH);
        maxArmor = progressController.GetStat(Stat.ARMOR);
        maxSpeed = progressController.GetStat(Stat.SPEED);

        currentDamage = progressController.GetStat(Stat.POWER);
        currentHealth = progressController.GetStat(Stat.HEALTH);
        currentArmor = progressController.GetStat(Stat.ARMOR);
        currentSpeed = progressController.GetStat(Stat.SPEED);
    }

    public void SetStats()
    {
        agent.speed = currentSpeed;

    }
    private void StateSelector()
    {
        GetEnemyToAttack();

        if (target != null)
        {
            distanceToTarget = Vector3.Distance(target.position, transform.position);
        }

        if (tacticCounter > 0 && currentState == State.Fire)
        {
            tacticCounter -= Time.deltaTime;
        }

        if (tacticCounter <= 0 && currentState == State.Fire && !tacticExecute)
        {
            currentState = GetRandomTactic();
            tacticExecute = true;
        }
        else if (distanceToTarget <= chaseRange && distanceToTarget > attackRange && tacticCounter > 0)
        {
            currentState = State.Chase;

        }
        else if (DetectEnemy() && tacticCounter > 0)
        {
            currentState = State.Fire;
        }
        else if (currentState != State.MoveForward || currentState != State.GetBack || currentState != State.MoveLeft
            || currentState != State.MoveRight)
        {
            currentState = State.Search;
        }
    }

    private void StateExecute()
    {
        if (GameManager.Instance.Gamestate != GameManager.GAMESTATE.Ingame)
        {
            return;
        }
        switch (currentState)
        {
            case State.Search:
                SearchNewPlaceToGo();
                if (agent.velocity == Vector3.zero)
                {
                    meshAnimator.SetRunAnim(false);
                }
                else
                {
                    meshAnimator.SetRunAnim(true);
                }
                break;
            case State.Chase:
                meshAnimator.SetFireAnimation(false);
                meshAnimator.SetRunAnim(true);
                ChaseTheTarget();
                break;
            case State.GetBack:
                meshAnimator.SetFireAnimation(false);
                meshAnimator.SetRunAnim(true);
                GetBack();
                break;
            case State.MoveForward:
                meshAnimator.SetFireAnimation(false);
                meshAnimator.SetRunAnim(true);
                MoveForward();
                break;
            case State.MoveRight:
                meshAnimator.SetFireAnimation(false);
                meshAnimator.SetRunAnim(true);
                MoveRight();
                break;
            case State.MoveLeft:
                meshAnimator.SetFireAnimation(false);
                meshAnimator.SetRunAnim(true);
                MoveLeft();
                break;
            case State.Fire:
                meshAnimator.SetFireAnimation(true);
                meshAnimator.SetRunAnim(false);
                AttackState();
                break;
            default:
                break;
        }
    }

    protected override void Attack(Transform point, Transform parent)
    {
        base.Attack(point, parent);
    }

    private IEnumerator AttackAnimationRoutine(Transform point, Transform parent)
    {
        if (onAttackProcess)
        {
            yield break;
        }
        onAttackProcess = true;
        Debug.Log("Process Started");
        yield return new WaitUntil(() => !meshAnimator.anim.IsInTransition(0) &&
        meshAnimator.anim.GetCurrentAnimatorStateInfo(0).IsTag("AttackAnim") && meshAnimator.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= nextAttackTime);
        nextAttackTime += animationFirePosition;
        Debug.Log("Is Attacking");
        yield return new WaitUntil(() => !meshAnimator.anim.IsInTransition(0) &&
            meshAnimator.anim.GetCurrentAnimatorStateInfo(0).IsTag("AttackAnim") && meshAnimator.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= nextAttackTime + (nextAttackTime - 1f));
        Attack(point, parent);
        onAttackProcess = false;
    }

    private bool DetectEnemy()
    {
        if (distanceToTarget <= attackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
        //Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);
        //Debug.DrawRay(transform.position, transform.forward * visibleRadius, Color.red);
        //for (int i = 0; i < colliders.Length; i++)
        //{
        //    if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Enemies"))
        //    {
        //        CanAttackPlayer = true;
        //        PlayerCollider = colliders[i];
        //        return true;
        //    }
        //}
        //return false;
    }


    private void GetEnemyToAttack()
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

        Player player = FindObjectOfType<Player>();

        if (player != null)
        {
            targetList.Add(player.transform);
        }

        foreach (Transform item in targetList)
        {
            float distanceToEnemy = Vector3.Distance(item.transform.position, this.transform.position);

            if (distanceToEnemy < enemyDistance)
            {
                enemyDistance = distanceToEnemy;
                target = item.transform;
            }
        }
    }
    private void AttackState()
    {
        if (target == null)
        {
            return;
        }
        agent.isStopped = true;
        agent.velocity = agent.velocity * 0.1f;
        LookAtEnemy(target.GetComponent<Collider>());
        StartCoroutine(AttackAnimationRoutine(bulletPoint, transform));
    }
    private void ChaseTheTarget()
    {
        if (target == null)
        {
            return;
        }
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(target.position);
    }

    private void SearchNewPlaceToGo()
    {
        if (agent.remainingDistance <= 0.1f && !isSearched ||
                            !agent.hasPath && !isSearched)
        {
            Vector3 agentTarget = new Vector3(agent.destination.x, transform.position.y, agent.destination.z);

            agent.enabled = false;
            transform.position = agentTarget;
            agent.enabled = true;

            Invoke("Search", patrolWaitTime);
            isSearched = true;
        }
    }

    private void Search()
    {
        agent.isStopped = false;
        isSearched = false;
        agent.speed = patrolSpeed;
        agent.SetDestination(GetRandomPosition());
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 sphere = UnityEngine.Random.insideUnitSphere;
        Vector3 randomDirection = new Vector3(sphere.x * 5, sphere.y, sphere.z * 5) * patrolRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1);
        return hit.position;
    }

    private void MoveLeft()
    {
        agent.isStopped = false;
        isSearched = false;
        agent.speed = tacticSpeed;
        agent.SetDestination(transform.position - transform.right * 5);
        StartCoroutine(CheckIsOnTheTargetPoint());
    }

    private void MoveRight()
    {
        agent.isStopped = false;
        isSearched = false;
        agent.speed = tacticSpeed;
        agent.SetDestination(transform.position + transform.right * 5);
        StartCoroutine(CheckIsOnTheTargetPoint());
    }

    public void MoveForward()
    {
        agent.isStopped = false;
        isSearched = false;
        agent.speed = tacticSpeed;
        agent.SetDestination(transform.position + transform.forward * 5);
        StartCoroutine(CheckIsOnTheTargetPoint());
    }

    public void GetBack()
    {
        agent.isStopped = false;
        isSearched = false;
        agent.speed = tacticSpeed;
        agent.SetDestination(transform.position - transform.forward * 15);
        StartCoroutine(CheckIsOnTheTargetPoint());
    }

    public State GetRandomTactic()
    {
        int number = UnityEngine.Random.Range(0, 100);

        if (number > 50)
        {
            if (distanceToTarget <= attackRange)
            {
                return State.GetBack;
            }
            return State.MoveForward;
        }
        else
        {
            if (distanceToTarget >= attackRange)
            {
                return State.MoveForward;
            }
            return State.GetBack;
        }
    }

    public IEnumerator CheckIsOnTheTargetPoint()
    {
        yield return new WaitUntil(() => agent.remainingDistance <= 0.1f);
        tacticExecute = false;
        tacticCounter = tacticWaitTime;
    }
    private void OnDrawGizmos()
    {

    }

}
