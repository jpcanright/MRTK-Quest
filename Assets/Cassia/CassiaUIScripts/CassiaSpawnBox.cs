using System.Collections;
using System.Collections.Generic;
using EMP.Core;
using UnityEngine;

public class CassiaSpawnBox : MonoBehaviour
{
    [SerializeField] private Transform spawnBox;
    [SerializeField] private GameObject anchorGhost;
    [SerializeField] private GameObject measureGhost;
    [SerializeField] private GameObject forceGhost;
    [SerializeField] private GameObject negativeGhost;
    [SerializeField] private GameObject positiveGhost;
    [SerializeField] private GameObject mintGhost;
    [SerializeField] private GameObject currentGhost;
    
    [SerializeField] private GameObject anchorPrefab;
    [SerializeField] private GameObject measurePrefab;
    [SerializeField] private GameObject forcePrefab;
    [SerializeField] private GameObject negativePrefab;
    [SerializeField] private GameObject positivePrefab;
    [SerializeField] private GameObject mintPrefab;

    [SerializeField] private GameObject[] randomPrefabs;
    [SerializeField] private GameObject[] triforcePrefabs;
    [SerializeField] private GameObject[] tagalongPrefabs;
    [SerializeField] private GameObject[] poolPrefabs;
    [SerializeField] private GameObject[] sinePrefabs;

    [SerializeField] private InteractionField[] triforceFields;
    
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color occupiedColor;

    public enum SpawnBoxState
    {
        Empty,
        Occupied,
        ShowingGhost
    }

    public enum SpawnableObject
    {
        Anchor,
        MeasuringTape,
        ForceMeter,
        NegativeCharge,
        PositiveCharge,
        Mint,
        Random,
        Triforce,
        Tagalong,
        Pools,
        Sine
    }

    private SpawnBoxState _spawnBoxState = SpawnBoxState.Empty;

    

    private GameObject TriforceSpawn()
    {
        if (EMController.Instance.GetFieldGenerators<PointChargeGenerator>(triforceFields[0]).Count == 0)
        {
            return triforcePrefabs[0];
        }
        if (EMController.Instance.GetFieldGenerators<PointChargeGenerator>(triforceFields[1]).Count == 0)
        {
            return triforcePrefabs[1];
        }
        if (EMController.Instance.GetFieldGenerators<PointChargeGenerator>(triforceFields[2]).Count == 0)
        {
            return triforcePrefabs[2];
        }

        return RandomObject(triforcePrefabs);
    }
    
    private GameObject RandomObject(GameObject[] objects)
    {
        return objects[Random.Range(0, objects.Length)];
    }

    public void SpawnObject(GameObject prefab)
    {
        Instantiate(prefab, spawnBox.position, spawnBox.rotation);
        _spawnBoxState = SpawnBoxState.Occupied;
    }
    
    public void SpawnObject(SpawnableObject objectToSpawn)
    {
        if (_spawnBoxState == SpawnBoxState.Occupied)
        {
            // TODO show failure by flashing red or something
            return;
        }
        else if (_spawnBoxState == SpawnBoxState.ShowingGhost)
        {
            ClearCurrentGhost();
        }

        GameObject prefab;
        
        switch (objectToSpawn)
        {
            case SpawnableObject.Anchor: prefab = anchorPrefab;
                break;
            case SpawnableObject.MeasuringTape: prefab = measurePrefab;
                break;
            case SpawnableObject.ForceMeter: prefab = forcePrefab;
                break;
            case SpawnableObject.NegativeCharge: prefab = negativePrefab;
                break;
            case SpawnableObject.PositiveCharge: prefab = positivePrefab;
                break;
            case SpawnableObject.Mint: prefab = mintPrefab;
                break;
            case SpawnableObject.Random:
                prefab = RandomObject(randomPrefabs);
                break;
            case SpawnableObject.Triforce:
                prefab = TriforceSpawn();//RandomObject(triforcePrefabs);
                break;
            case SpawnableObject.Tagalong: prefab = RandomObject(tagalongPrefabs);
                break;
            case SpawnableObject.Pools: prefab = RandomObject(poolPrefabs);
                break;
            case SpawnableObject.Sine: prefab = RandomObject(sinePrefabs);
                break;
            default: prefab = anchorPrefab;
                Debug.LogError("Switch in SpawnObject defaulted; this should never happen.");
                break;
        }
        // TODO Normcore integration
        //Instantiate(prefab, transform.position, transform.rotation);
        
        SpawnObject(prefab);
    }
    
    public void SpawnObject(string objectToSpawn)
    {
        SpawnObject(ParseSpawnableObject(objectToSpawn));
    }

    public void SpawnObject(SpawnableObjectSet objectSet)
    {
        var objectToSpawn = objectSet.GetSpawnablePrefab();
        SpawnObject(objectToSpawn);
    }
    
    public SpawnableObject ParseSpawnableObject(string myString)
    {
        SpawnableObject enumerable = SpawnableObject.Mint;
        try
        {
            enumerable = (SpawnableObject)System.Enum.Parse(typeof(SpawnableObject), myString);
            //Foo(enumerable); //Now you have your enum, do whatever you want.
            
        }
        catch (System.Exception)
        {
            Debug.LogErrorFormat("Parse: Can't convert {0} to enum. Please check the spelling.", myString);
        }
        return enumerable;
    }
    
    public void ShowGhost(SpawnableObject objectToShow)
    {
        if (_spawnBoxState == SpawnBoxState.Occupied)
        {
            // TODO show failure by flashing red or something
            return;
        }
        else if (_spawnBoxState == SpawnBoxState.ShowingGhost)
        {
            ClearCurrentGhost();
            
        }
        GameObject nextGhost;
        switch (objectToShow)
        {
            case SpawnableObject.Anchor: nextGhost = anchorGhost;
                break;
            case SpawnableObject.MeasuringTape: nextGhost = measureGhost;
                break;
            case SpawnableObject.ForceMeter: nextGhost = forceGhost;
                break;
            case SpawnableObject.NegativeCharge: nextGhost = negativeGhost;
                break;
            case SpawnableObject.PositiveCharge: nextGhost = positiveGhost;
                break;
            case SpawnableObject.Mint: nextGhost = mintGhost;
                break;
            default: nextGhost = anchorGhost;
                Debug.LogError("Switch in ShowGhost defaulted; this should never happen.");
                break;
        }
        nextGhost.SetActive(true);
        currentGhost = nextGhost;
        _spawnBoxState = SpawnBoxState.ShowingGhost;
    }

    private void ClearCurrentGhost()
    {
        if (_spawnBoxState == SpawnBoxState.ShowingGhost)
        {
            currentGhost.SetActive(false);
            currentGhost = null;
            _spawnBoxState = SpawnBoxState.Empty;
        }
        
    }

    private void CheckForOccupancy()
    {
        Collider[] collisions = Physics.OverlapBox(spawnBox.position, 
            spawnBox.localScale / 2, 
            spawnBox.rotation, 
            //1 << 10); // "Deletable" layer
            1 << 9); // "Elements" layer
        if (collisions.Length > 0)
        {
            ClearCurrentGhost();
            _spawnBoxState = SpawnBoxState.Occupied;
        }
        else
        {
            _spawnBoxState = SpawnBoxState.Empty;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckForOccupancy();
        if (_spawnBoxState == SpawnBoxState.Occupied)
        {
            spawnBox.gameObject.GetComponent<MeshRenderer>().material.color = occupiedColor;
        }
        else
        {
            spawnBox.gameObject.GetComponent<MeshRenderer>().material.color = defaultColor;
        }
    }
}
