using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : SingletonPersistent<SpawnManager>
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject gunner;
    [SerializeField] private GameObject zombie;
    public int enemySpawnCount = 10;
    public bool onSpawnProcess = false;
    public bool spawnEnemies = true;
    public bool waitForSpawn = true;

    private Camera cam;
    private Plane[] planes;

    private int level;
    private int index;

    public IEnumerator SetSpawner()
    {
        onSpawnProcess = false;
        level = Mathf.Min(PlayerPrefs.GetInt("Level", 1) * 2, 25);
        enemySpawnCount = level;
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
    }

    private IEnumerator SpawnEnemy()
    {
        if (!spawnEnemies)
        {
            yield break;
        }

        yield return new WaitUntil(() => !waitForSpawn);

        onSpawnProcess = true;
        GameObject enemy = GetRandomEnemy();
        GameObject clone = InstantiateEnemy(enemy);

        while (IsCharacterNear(clone) || CharacterOnScreen(clone))
        {
            Destroy(clone);
            clone = InstantiateEnemy(enemy);
            yield return null;
        }

        index++;
        InitializeSpawnedEnemy(clone);

        yield return new WaitUntil(() => GameManager.Instance == null);
        yield return new WaitUntil(() => GameManager.allEnemiesList.Count < 6 || GameManager.Instance.Gamestate == GameManager.GAMESTATE.Finish);

        onSpawnProcess = false;
    }

    private GameObject InstantiateEnemy(GameObject enemy)
    {
        return Instantiate(enemy, GetRandomPosition(), Quaternion.identity);
    }

    private void InitializeSpawnedEnemy(GameObject clone)
    {
        clone.name = index.ToString();
        clone.transform.SetParent(ObjectPool.Instance.transform);
        clone.transform.GetChild(0).gameObject.SetActive(true);

        Enemy enemyComponent = clone.GetComponent<Enemy>();
        Zombie zombieComponent = clone.GetComponent<Zombie>();

        if (enemyComponent != null) enemyComponent.enabled = true;
        if (zombieComponent != null) zombieComponent.enabled = true;
    }

    private bool IsCharacterNear(GameObject sender)
    {
        List<GameObject> lookUp = new List<GameObject>(GameManager.allEnemiesList);
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

        Collider objCollider = sender.GetComponent<Collider>();

        return GeometryUtility.TestPlanesAABB(planes, objCollider.bounds);
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
        return GameManager.Instance.ZombieMode ? zombie : gunner;
    }
}
