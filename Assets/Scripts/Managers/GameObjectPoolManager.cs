using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    potionStirLR,
    potionStirFB
}

public class GameObjectPoolManager : MonoBehaviour
{
    public static GameObjectPoolManager instance;

    Dictionary<PoolType, Queue<GameObject>> poolParents = new Dictionary<PoolType, Queue<GameObject>>();
    Dictionary<PoolType, GameObject> originalPreFab = new Dictionary<PoolType, GameObject>();

    [Header("Pools")]
    [SerializeField] List<GameObject> potionStirLRPool = new List<GameObject>();
    [SerializeField] List<GameObject> potionStirFBPool = new List<GameObject>();

    [Header("Original PreFabs")]
    [SerializeField] GameObject potionStirLRPreFab;
    [SerializeField] GameObject potionStirFBPreFab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;        

        poolParents.Add(PoolType.potionStirLR, new Queue<GameObject>(potionStirLRPool));
        poolParents.Add(PoolType.potionStirFB, new Queue<GameObject>(potionStirFBPool));

        originalPreFab.Add(PoolType.potionStirLR, potionStirLRPreFab);
        originalPreFab.Add(PoolType.potionStirFB, potionStirFBPreFab);
    }

    public GameObject GetPoolObject(PoolType poolName)
    {
        if (poolParents.TryGetValue(poolName, out Queue<GameObject> tempPool))
        {
            if (tempPool != null)
            {
                if (tempPool.Count > 0)
                {
                    GameObject tempObject = tempPool.Dequeue();
                    tempObject.SetActive(true);                    
                    return tempObject;
                }else
                {
                    if (originalPreFab.TryGetValue(poolName, out GameObject prefab) && prefab != null)
                    {
                        GameObject tempObject = Instantiate(prefab);
                        tempObject.SetActive(true);
                        return tempObject;
                    }                    
                }
            }
        }
        return null;
    }

    public void ReturnPoolObject(GameObject returnObject, PoolType poolName, float waitTime = 0)
    {
        StartCoroutine(ReturnPoolObjectCoroutine(waitTime, returnObject, poolName));
    }

    IEnumerator ReturnPoolObjectCoroutine(float waitTime, GameObject returnObject, PoolType poolName)
    {
        yield return new WaitForSeconds(waitTime);
        if (poolParents.TryGetValue(poolName, out Queue<GameObject> tempPool)) 
        {
            returnObject.SetActive(false);
            tempPool.Enqueue(returnObject);
        }        
    }
}