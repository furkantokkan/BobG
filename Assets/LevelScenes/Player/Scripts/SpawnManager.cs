using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private GameObject[] enemys;

    public int enemySpawnCount = 10;

    private bool onSpawnProcess = false;


    GameObject obj;
    Collider objCollider;

    Camera cam;
    Plane[] planes;

    public IEnumerator SetSpawner()
    {
       
        for (int i = 0; i < enemySpawnCount; i++)
        {
            yield return new WaitUntil(() => !onSpawnProcess);
            if (GameManager.Instance.Gamestate == GameManager.GAMESTATE.Finish)
            {
                yield break;
            }
            yield return SpawnEnemy();
        }
    }

    private IEnumerator SpawnEnemy()
    {
        onSpawnProcess = true;
        GameObject enemy = GetRandomEnemy();
        GameObject clone = Instantiate(enemy, GetRandomPosition(), Quaternion.identity);
        while (IsCharacterNear(clone) || CharacterOnScreen(clone))
        {

            Destroy(clone);
            clone = Instantiate(enemy, GetRandomPosition(), Quaternion.identity);
            yield return null;
        }

        clone.transform.SetParent(ObjectPool.Instance.transform);
        clone.transform.GetChild(0).gameObject.SetActive(true);
        clone.GetComponent<Enemy>().enabled = true;
        yield return new WaitUntil(() => GameManager.Instance.allEnemiesList.Count < 6 || GameManager.Instance.Gamestate == GameManager.GAMESTATE.Finish);
        onSpawnProcess = false;
    }
    private bool IsCharacterNear(GameObject sender)
    {
        List<Enemy> lookUp = new List<Enemy>();
        lookUp.AddRange(GameManager.Instance.allEnemiesList);
        lookUp.Remove(sender.GetComponent<Enemy>());

        foreach (Enemy item in lookUp)
        {
            if (Vector3.Distance(sender.transform.position, item.transform.position) < 20f)
            {
                return true;
            }
            else
            {
                continue;
            }
        }

        return false;
    }
    private bool CharacterOnScreen(GameObject sender)
    {
        cam = Camera.main;
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        objCollider = sender.GetComponent<Collider>();
        if (GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
        {
            Debug.Log(" has been detected!");
            return true;
        }
        else
        {
            Debug.Log("Nothing has been detected");
            return false;
        }
    }
    private Vector3 GetRandomPosition()
    {
        var sphere = Random.insideUnitSphere * 50f;
        NavMeshHit hit;
        NavMesh.SamplePosition(sphere, out hit, 50f, 1);
        return hit.position;
    }

    private GameObject GetRandomEnemy()
    {
        int index = Random.Range(0, enemys.Length);
        return enemys[index];
    }
}
