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
    Tactic
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

    [SerializeField] List<Transform> targetList = new List<Transform>(); 

    private void Awake()
    {
        progressController = GetComponent<ProgressController>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        fireRateStorage = animationFirePosition;
        tacticWaitTime = UnityEngine.Random.Range(5, 15f);
        tacticCounter = tacticWaitTime;
        effectManager = transform.GetChild(0).GetComponent<EffectManager>();
        progressController.SetRandomStartingLevel();
    }
    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameManager.Instance.Gamestate == GameManager.GAMESTATE.Ingame);
        UpdateStats();
        SetStats();
        agent.SetDestination(GetRandomPosition());
    }

    private void Update()
    {
        StateSelector();
        StateExecute();
        //meshAnimator.transform.localRotation = Quaternion.identity;
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
            healthBar.fillAmount -= (float)(other.GetComponent<Bullet>().bulletDamage - currentArmor < 5 ? 5 :
                other.GetComponent<Bullet>().bulletDamage - currentArmor) / maxHealth;
            healthBar.color = Color.Lerp(Color.green, Color.red, 1.2f - healthBar.fillAmount);
            if (healthBar.fillAmount <= 0)
            {
                if (other.GetComponent<Bullet>().sender.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    enemy.target = null;
                }
                healthBar.transform.parent.parent.gameObject.SetActive(false);
                transform.GetChild(0).GetComponent<AnimController>().anim.SetTrigger("Death");
                effectManager.Death.Play();
                effectManager.Circle.SetActive(false);
                UIManager.Instance.Coin += 5 * progressController.incomeLevel;
                CancelInvoke("Search");
                agent.isStopped = true;
                agent.speed = 0f;
                GetComponent<CapsuleCollider>().enabled = false;
                GameManager.Instance.deadEnemyCount += 1;
                enabled = false;
            }
            Destroy(other.gameObject);
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

        if (tacticExecute)
        {
            return;
        }

        else if (distanceToTarget <= chaseRange && distanceToTarget > attackRange && tacticCounter > 0)
        {
            currentState = State.Chase;

        }
        else if (DetectEnemy() && tacticCounter > 0)
        {
            currentState = State.Fire;
        }
        else if (distanceToTarget >= chaseRange || !DetectEnemy())
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

        if (currentState != State.Fire)
        {
            LookAtVector(agent.destination);
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
                meshAnimator.SetFireAnimation(false);
                onAttackProcess = false;
                nextAttackTime = 0f;
                break;
            case State.Chase:
                if (target != null)
                {
                    LookAtEnemy(target.GetComponent<Collider>());
                }
                meshAnimator.SetFireAnimation(false);
                meshAnimator.SetRunAnim(true);
                ChaseTheTarget();
                nextAttackTime = 0f;
                onAttackProcess = false;
                break;
            case State.GetBack:
                meshAnimator.SetFireAnimation(false);
                meshAnimator.SetRunAnim(true);
                nextAttackTime = 0f;
                onAttackProcess = false;
                GetBack();
                break;
            case State.MoveForward:
                meshAnimator.SetFireAnimation(false);
                meshAnimator.SetRunAnim(true);
                nextAttackTime = 0f;
                MoveForward();
                onAttackProcess = false;
                break;
            case State.MoveRight:
                meshAnimator.SetFireAnimation(false);
                meshAnimator.SetRunAnim(true);
                nextAttackTime = 0f;
                onAttackProcess = false;
                MoveRight();
                break;
            case State.MoveLeft:
                meshAnimator.SetFireAnimation(false);
                meshAnimator.SetRunAnim(true);
                nextAttackTime = 0f;
                onAttackProcess = false;
                MoveLeft();
                break;
            case State.Fire:
                if (target != null)
                {
                    LookAtEnemy(target.GetComponent<Collider>());
                }
                meshAnimator.SetFireAnimation(true);
                meshAnimator.SetRunAnim(false);
                AttackState();
                isSearched = false;
                break;
            default:
                break;
        }
    }

    protected override void Attack(Transform point, Transform parent, int newDamage)
    {
        base.Attack(point, parent, newDamage);
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
        Attack(point, parent, currentDamage);
        onAttackProcess = false;
    }

    private bool DetectEnemy()
    {
        if (target == null)
        {
            return false;
        }

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

        targetList = new List<Transform>();
        foreach (Enemy item in GameManager.Instance.allEnemiesList)
        {
            if (item == this)
            {
                continue;
            }
            targetList.Add(item.transform);
        }

        Player player = FindObjectOfType<Player>();

        targetList.Add(player.transform);

        float playerdistance = Vector3.Distance(player.transform.position, this.transform.position);

        foreach (Transform item in targetList)
        {
            float distanceToEnemy = Vector3.Distance(item.transform.position, this.transform.position);

            Debug.Log("Player Distacnce: " + distanceToEnemy + "EnemyDistance: " + distanceToEnemy);
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
                    Debug.LogWarning("Tag:" + hit.collider.gameObject.tag);
                    if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Player"))
                    {
                        Debug.LogWarning("Waiting For Select");
                        if (playerdistance < distanceToEnemy)
                        {
                            target = player.transform;
                        }
                        else
                        {
                            Debug.LogWarning("Selected: " + item.transform.name);
                            target = item.transform;
                        }
                    }
                }
            }
        }
    }
    private void AttackState()
    {
        if (target == null)
        {
            return;
        }
        agent.SetDestination(transform.position);
        agent.isStopped = true;
        agent.velocity = agent.velocity * 0.1f;
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
        if (agent.remainingDistance <= agent.stoppingDistance + 0.1f && !isSearched)
        {
            Vector3 agentTarget = new Vector3(agent.destination.x, transform.position.y, agent.destination.z);

            //agent.enabled = false;
            //transform.position = agentTarget;
            //agent.enabled = true;
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
        StartCoroutine(CheckIsOnTheTargetPoint(transform.position - transform.right * 15));
    }

    private void MoveRight()
    {
        agent.isStopped = false;
        isSearched = false;
        agent.speed = tacticSpeed;
        StartCoroutine(CheckIsOnTheTargetPoint(transform.position + transform.right * 15));
    }

    public void MoveForward()
    {
        agent.isStopped = false;
        isSearched = false;
        agent.speed = tacticSpeed;
        StartCoroutine(CheckIsOnTheTargetPoint(transform.position + transform.forward * 15));
    }

    public void GetBack()
    {
        agent.isStopped = false;
        isSearched = false;
        agent.speed = tacticSpeed;
        StartCoroutine(CheckIsOnTheTargetPoint(transform.position - transform.forward * 15));
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

    public IEnumerator CheckIsOnTheTargetPoint(Vector3 dest)
    {
        currentState = State.Tactic;
        do
        {
            agent.SetDestination(dest);
            yield return null;
        } while (agent.remainingDistance > agent.stoppingDistance + 0.1f);
        tacticExecute = false;
        tacticCounter = tacticWaitTime;
    }
    private void OnDrawGizmos()
    {

    }

}
