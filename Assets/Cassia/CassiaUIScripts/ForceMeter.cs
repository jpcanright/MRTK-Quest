using System.Collections;
using System.Collections.Generic;
using EMP.Core;
using UnityEngine;

public class ForceMeter : MonoBehaviour
{
    [SerializeField] private QuantityDisplayBoard quantityDisplay;
//    [SerializeField] private VRTK_SnapDropZone dropZone;
    [SerializeField] private FieldReactor[] reactors = null;
    //[SerializeField] private MonopoleVelocityStateController monopoleStateController;
    [SerializeField] private Transform pivotTransform;
    [SerializeField] private Transform grabberTransform;
    [SerializeField] private ConfigurableJoint joint;
    
    // Start is called before the first frame update
    void OnEnable()
    {
//        dropZone.ObjectSnappedToDropZone += ChargeSnappedToDropZone;
//        dropZone.ObjectUnsnappedFromDropZone += ChargeUnsnappedFromDropZone;
        quantityDisplay.SetQuantityFunction(GetForce);
    }

    private void OnDisable()
    {
//        dropZone.ObjectSnappedToDropZone -= ChargeSnappedToDropZone;
//        dropZone.ObjectUnsnappedFromDropZone -= ChargeUnsnappedFromDropZone;
    }

//    private void ChargeSnappedToDropZone(object o, SnapDropZoneEventArgs e)
//    {
//        reactors = e.snappedObject.GetComponents<FieldReactor>();
//        monopoleStateController = e.snappedObject.GetComponent<MonopoleVelocityStateController>();
//
//    }

//    private void ChargeUnsnappedFromDropZone(object o, SnapDropZoneEventArgs e)
//    {
//        reactors = null;
//        monopoleStateController = null;
//    }

    private Vector3 SumReactorForces()
    {
        Vector3 sum = Vector3.zero;
        if (reactors == null)
        {
            return sum;
        }
        if (reactors.Length == 0)
        {
            return sum;
        }
        foreach (FieldReactor reactor in reactors)
        {
            sum += reactor.ForceUponReactor();
        }

        return sum;
    }
    
    public float GetForce()
    {
        // Return zero if any of the following are true:
        // - There are no reactors.
        // - There is no attached monopoleStateController.
        // - The attached monopole is anchored.
        // - Physics is paused.
//        if (reactors == null || !monopoleStateController)
//        {
//            return 0;
//        }
//        if (monopoleStateController.IsAnchored 
//            || MonopoleVelocityStateManager.Instance.PhysicsIsFrozen)
//        {
//            return 0;
//        }
//        else
//        {
            // Compute force as the dot product of the direction vector from the pivot to the grabber and the force upon the reactor.
            //return reactor.ForceUponReactor().magnitude;
            
            
            //joint.targetRotation = Quaternion.LookRotation(reactor.ForceUponReactor().normalized, Vector3.up);
            
            ////joint.targetRotation = Quaternion.FromToRotation((grabberTransform.position - pivotTransform.position).normalized, reactor.ForceUponReactor().normalized);
            //Debug.DrawRay(pivotTransform.position,reactor.ForceUponReactor().normalized);
            return Vector3.Dot(SumReactorForces(), (grabberTransform.position - pivotTransform.position).normalized);
        //}
    }
}
