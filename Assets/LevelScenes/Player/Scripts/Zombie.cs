using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Zombie : MonoBehaviour
{
    [SerializeField] int maxHealth;
    [SerializeField] float attackRange = 8f;
    private NavMeshAgent agent;
    Animator getnim;
    private Player player;

    bool isDead = false;
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>();
        getnim = transform.GetChild(0).GetComponent<Animator>();
        GameManager.Instance.allEnemiesList.Add(gameObject);
    }
    
    void Timer()
    {
        GameManager.Instance.Gamestate = GameManager.GAMESTATE.Finish;
    }
    
    void Update()
    {
        if (GameManager.Instance.Gamestate == GameManager.GAMESTATE.Ingame)
            Move();
    }
    void Move()
    {
        if (player.healthBar.fillAmount <= 0)
            return;

        agent.SetDestination(player.transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) < attackRange && agent.isStopped == false)
        {
            getnim.SetTrigger("Attack");
            player.healthBar.fillAmount = 0f;
            player.transform.GetChild(0).GetComponent<AnimController>().DeathAnim();
            player.transform.GetChild(0).GetComponent<EffectManager>().Death.Play();
        }
        if (maxHealth <= 0 && isDead == false)
        {
            getnim.SetTrigger("Dead");
            if (GameManager.Instance.allEnemiesList.Contains(gameObject))
            {
                GameManager.Instance.allEnemiesList.Remove(gameObject);
            }
            GameManager.Instance.deadEnemyCount += 1;
            agent.isStopped = true;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<CapsuleCollider>().isTrigger = true;
            isDead = true;
            if (SpawnManager.Instance.enemySpawnCount - GameManager.Instance.deadEnemyCount <= 0)
            {
                Invoke("Timer",2f);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            maxHealth--;
            Destroy(other.gameObject);
        }
    }
}
