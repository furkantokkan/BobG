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
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>();
        getnim = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    
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

        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            getnim.SetTrigger("Attack");
            player.healthBar.fillAmount = 0f;
            player.transform.GetChild(0).GetComponent<AnimController>().DeathAnim();
            player.transform.GetChild(0).GetComponent<EffectManager>().Death.Play();
        }
        if (maxHealth <= 0)
        {
            getnim.SetTrigger("Dead");
            if (GameManager.allEnemiesList.Contains(gameObject))
            {
                GameManager.allEnemiesList.Remove(gameObject);
            }
            agent.isStopped = true;
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<CapsuleCollider>().isTrigger = true;
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
