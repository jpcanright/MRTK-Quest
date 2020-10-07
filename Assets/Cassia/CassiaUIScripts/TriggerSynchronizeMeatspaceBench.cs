using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SynchronizeMeatspaceBench))]
public class TriggerSynchronizeMeatspaceBench : MonoBehaviour
{
    [SerializeField] private SynchronizeMeatspaceBench synchronizer;
    
    // Start is called before the first frame update
    void OnValidate()
    {
        synchronizer = GetComponent<SynchronizeMeatspaceBench>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            synchronizer.Synchronize();
        }
    }
}
