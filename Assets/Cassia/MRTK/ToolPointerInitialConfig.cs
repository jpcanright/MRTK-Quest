using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class ToolPointerInitialConfig : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Turning off deletor pointer at startup.");
        PointerUtils.SetPointerBehavior<DeletorPointer>(PointerBehavior.AlwaysOff, Handedness.Left);
        PointerUtils.SetPointerBehavior<DeletorPointer>(PointerBehavior.AlwaysOff, Handedness.Right);
    }
}
