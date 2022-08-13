using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    [Serializable]
    public struct Pool
    {
        public Queue<GameObject> PooledObject;
        public GameObject objectPrefab;
        public int poolsize;
    }
    [SerializeField] public Pool[] pools = null;
    private void Awake()
    {
        for (int i = 0; i < pools.Length; i++)
        {
            pools[i].PooledObject = new Queue<GameObject>();
            for (int j = 0; j < pools[i].poolsize; j++)
            {
                GameObject obj = Instantiate(pools[i].objectPrefab);
                obj.SetActive(false);
                pools[i].PooledObject.Enqueue(obj);
            }
        }
    }
    public GameObject GetPooledObject(int objectType)
    {
        if (objectType >= pools.Length) return null;
        if(pools[objectType].PooledObject.Count == 0)
            AddSizePool(5,objectType);
        GameObject obj = pools[objectType].PooledObject.Dequeue();
        obj.SetActive(true);
        return obj;
    }
    public void SetPooledObject(GameObject pooledObject, int objectType)
    {
        if (objectType >= pools.Length) return;
        pools[objectType].PooledObject.Enqueue(pooledObject);
        pooledObject.SetActive(false);
    }
    public void AddSizePool(float amount, int objectType)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(pools[objectType].objectPrefab);
            obj.SetActive(false);
            pools[objectType].PooledObject.Enqueue(obj);
        }
    }
}