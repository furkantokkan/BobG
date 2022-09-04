using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class AirdropController : MonoBehaviour
{
    public float timer =-1;
    private AirdropPointer AP;
    private bool open;
    private Transform player;
    private Animator anim;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        open = false;
        AP = GetComponent<AirdropPointer>();
        anim = AP.target.GetComponent<Animator>();
        AP.img.gameObject.SetActive(open);
        AP.target.gameObject.SetActive(open);
        Invoke("Collected",5f);
    }

    void Update()
    {
        if (AP.target.gameObject && Vector3.Distance(AP.target.position, player.position) < 6f) Collected();
    }
    public void Collected()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
            anim.SetTrigger("Collect");
        }
        else
        {
            open = false;
            AP.target.gameObject.SetActive(open);
            AP.img.gameObject.SetActive(open);
            StartCoroutine(RandomSpawn());
            timer = 4;
        }
    }
    IEnumerator RandomSpawn()
    {
        AP.target.position = GetRandomPosition();
      yield return new WaitForSeconds(timer);
      open = true;
      AP.target.gameObject.SetActive(open);
      AP.img.gameObject.SetActive(open);
    }
    private Vector3 GetRandomPosition()
    {
        var sphere = Random.insideUnitSphere * 50f;
        NavMeshHit hit;
        NavMesh.SamplePosition(sphere, out hit, 50f,1);
        return hit.position;
    }

}
