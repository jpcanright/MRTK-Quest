using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class DeletorToggle : MonoBehaviour
{
    
    public void ToggleDeletorPointer()
    {
        // Get which hand the hand menu is tracking
        Handedness hand = transform.GetComponentInParent<SolverHandler>().CurrentTrackedHandedness;
        if (hand == Handedness.None)
        {
            Debug.LogWarning("DeletorToggle's parent isn't associated with a hand.");
        }
        
        // Get the first tool pointer on that hand
        IMixedRealityToolPointer toolPointer = PointerUtils.GetPointers<IMixedRealityToolPointer>(hand).First();
        
        InputSourceType inputSourceType = toolPointer.InputSourceParent.SourceType;
        
        // Get associated PointerBehavior
        PointerBehavior behavior = PointerUtils.GetPointerBehavior<DeletorPointer>(hand, inputSourceType);
        
        // IsInteractionEnabled should be a proxy for PointerBehavior, but I am not 100% sure and this could be a future source of errors
        //if (toolPointer.IsInteractionEnabled)
        if (behavior == PointerBehavior.AlwaysOn)
        {
            Debug.Log("Turning off deletor pointer.");
            PointerUtils.SetPointerBehavior<DeletorPointer>(PointerBehavior.AlwaysOff, hand);
            PointerUtils.SetPointerBehavior<ShellHandRayPointer>(PointerBehavior.Default, hand);
        }
        else
        {
            Debug.Log("Turning on deletor pointer.");
            PointerUtils.SetPointerBehavior<DeletorPointer>(PointerBehavior.AlwaysOn, hand);
            PointerUtils.SetPointerBehavior<ShellHandRayPointer>(PointerBehavior.AlwaysOff, hand);
        }
    }
}
