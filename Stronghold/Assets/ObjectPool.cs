using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }

    [UsedImplicitly]
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
            if (!pooledObjects[i].activeInHierarchy)
                return pooledObjects[i];

        amountToPool++;
        var tmp = Instantiate(objectToPool);
        tmp.SetActive(false);
        pooledObjects.Add(tmp);
        return pooledObjects[pooledObjects.Count - 1];
    }

    private void OnDestroy()
    {
        foreach (var pooledObject in pooledObjects) Destroy(pooledObject);
    }
}