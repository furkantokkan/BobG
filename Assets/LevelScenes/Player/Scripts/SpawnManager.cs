using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private GameObject gunner;
    [SerializeField] private GameObject zombie;

    public int enemySpawnCount = 10;

    public bool onSpawnProcess = false;

    public bool spawnEnemies = true;

    GameObject obj;
    Collider objCollider;

    Camera cam;
    Plane[] planes;

    private int level;

    public IEnumerator SetSpawner()
    {
        onSpawnProcess = false;
        level = PlayerPrefs.GetInt("Level", 1);
        enemySpawnCount = level * 2 > 25 ? 25 : level * 2;
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < enemySpawnCount; i++)
        {
            yield return new WaitUntil(() => !onSpawnProcess);
            if (GameManager.Instance.Gamestate == GameManager.GAMESTATE.Finish)
            {
                yield break;
            }
            yield return SpawnEnemy();
        }

        yield return null;
    }
    int index;
    private IEnumerator SpawnEnemy()
    {
        if (!spawnEnemies)
        {
            yield break;
        }
        onSpawnProcess = true;
        GameObject enemy = GetRandomEnemy();
        GameObject clone = Instantiate(enemy, GetRandomPosition(), Quaternion.identity);
        while (IsCharacterNear(clone) || CharacterOnScreen(clone))
        {
            Destroy(clone);
            clone = Instantiate(enemy, GetRandomPosition(), Quaternion.identity);
            yield return null;
        }
        index++;
        clone.gameObject.name = index.ToString();
        clone.transform.SetParent(ObjectPool.Instance.transform);
        clone.transform.GetChild(0).gameObject.SetActive(true);
        if (clone.GetComponent<Enemy>() != null)
        {
            clone.GetComponent<Enemy>().enabled = true;
        }
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(() => GameManager.allEnemiesList.Count < 6 || GameManager.Instance.Gamestate == GameManager.GAMESTATE.Finish);
        onSpawnProcess = false;
    }
    private bool IsCharacterNear(GameObject sender)
    {
        List<GameObject> lookUp = new List<GameObject>();
        lookUp.AddRange(GameManager.allEnemiesList);
        lookUp.Remove(sender.gameObject);

        foreach (GameObject item in lookUp)
        {
            if (Vector3.Distance(sender.transform.position, item.transform.position) < 20f)
            {
                return true;
            }
        }

        return false;
    }
    private bool CharacterOnScreen(GameObject sender)
    {
        cam = Camera.main;
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (planes == null)
        {
            return false;
        }
        objCollider = sender.GetComponent<Collider>();
        if (GeometryUtility.TestPlanesAABB(planes, objCollider.bounds))
        {
            return true;
        }
        else
        {
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
        if (GameManager.Instance.ZombieMode) return zombie;

        return gunner;
    }
}
