using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AirdropController : MonoBehaviour
{
    private AirdropPointer AP;
    private Transform player;
    private Animator anim;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        AP = GetComponent<AirdropPointer>();
        anim = AP.target.GetComponent<Animator>();
        AP.img.gameObject.SetActive(true);
    }

    void Update()
    {
        if (Vector3.Distance(AP.target.position, player.position) < 6f) Collected();
    }
    public void Collected()
    { 
        AP.img.gameObject.SetActive(false);
        anim.SetBool("Ready",true);
    }
    public IEnumerator RandomSpawn()
    { 
      yield return new WaitForSeconds(30);
      AP.target.position = GetRandomPosition() + new Vector3(0,25f,0);
      AP.img.gameObject.SetActive(true);
      anim.SetBool("Ready",false);
    }
    private Vector3 GetRandomPosition()
    {
        var sphere = Random.insideUnitSphere * 50f;
        NavMeshHit hit;
        NavMesh.SamplePosition(sphere, out hit, 50f,1);
        return hit.position;
    }
}
