using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class MRTKDeletable : MonoBehaviour
{
    [SerializeField] private ObjectManipulator manipulator;
    
    
    // Start is called before the first frame update
    void OnValidate()
    {
        if (!manipulator)
        {
            manipulator = GetComponent<ObjectManipulator>();
        }
    }

    private void OnEnable()
    {
        manipulator.OnManipulationStarted.AddListener(Delete);        
    }

    private void Delete(ManipulationEventData data)
    {
        //if (data.Pointer.GetType() == typeof(DeletorPointer))
        if (data.Pointer is IMixedRealityToolPointer)
        {
            Destroy(gameObject);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
