using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawnable Object Set", menuName = "ScriptableObjects/SpawnableObjectSet", order = 1)]
public class SpawnableObjectSet : ScriptableObject
{
    [SerializeField] protected GameObject[] spawnableObjects;

    public GameObject GetSpawnablePrefab()
    {
        return RandomObject(spawnableObjects);
    }
    
    private GameObject RandomObject(GameObject[] objects)
    {
        return objects[Random.Range(0, objects.Length)];
    }
}
