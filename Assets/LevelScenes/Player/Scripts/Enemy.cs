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
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private float fireRate;
    [SerializeField] float attackRange = 8f;
    [SerializeField] float turnSpeed = 15f;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float patrolRadius = 3f;
    [SerializeField] float patrolWaitTime = 2f;
    [SerializeField] float patrolSpeed = 4f;
    [SerializeField] float chaseSpeed = 5f;
    [SerializeField] float tacticSpeed = 5.5f;
    [SerializeField] float tacticWaitTime = 5f;
    [SerializeField] int damage = 2;
    [SerializeField] AnimatorOverrideController AnimatorOverrideController;

    [SerializeField] private AnimController meshAnimator;

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

    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        fireRate =
        fireRateStorage = fireRate;
        tacticCounter = tacticWaitTime;
    }

    private void Update()
    {
        StateSelector();
        StateExecute();
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
            if (other.GetComponent<Bullet>().sender == this.gameObject)
            {
                return;
            }
            Destroy(other.gameObject);
            healthBar.fillAmount -= 0.2f;
            if (healthBar.fillAmount <= 0)
            {
                //LootBoxManager.Instance.LootBoxStage();
                Destroy(gameObject);
            }
        }
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
                break;
            case State.Chase:
                meshAnimator.SetRunAnim(true);
                ChaseTheTarget();
                break;
            case State.GetBack:
                meshAnimator.SetRunAnim(true);
                GetBack();
                break;
            case State.MoveForward:
                meshAnimator.SetRunAnim(true);
                MoveForward();
                break;
            case State.MoveRight:
                meshAnimator.SetRunAnim(true);
                MoveRight();
                break;
            case State.MoveLeft:
                meshAnimator.SetRunAnim(true);
                MoveLeft();
                break;
            case State.Fire:
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
        if (meshAnimator.anim.GetBool("IsFire") == true)
        {
            yield break;
        }
        meshAnimator.SetFireAnimation(true);
        yield return new WaitUntil(() => !meshAnimator.anim.IsInTransition(0) &&
        meshAnimator.anim.GetCurrentAnimatorStateInfo(0).IsTag("AttackAnim") && meshAnimator.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        meshAnimator.SetFireAnimation(false);
        Attack(point, parent);
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

            //agent.enabled = false;
            //transform.position = agentTarget;
            //agent.enabled = true;
            if (agent.velocity == Vector3.zero)
            {
                meshAnimator.SetRunAnim(false);
            }
            Invoke("Search", patrolWaitTime);

            isSearched = true;
        }
    }

    private void Search()
    {
        agent.isStopped = false;
        isSearched = false;
        agent.speed = patrolSpeed;
        meshAnimator.SetRunAnim(true);
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
