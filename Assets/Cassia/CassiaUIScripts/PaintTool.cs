using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using VRTK;

public class PaintTool : MonoBehaviour
{
    public Material paintingMaterial;
    
    //public VRTK_ControllerEvents controllerEvents;
    
    
    
    private void OnValidate()
    {
        //controllerEvents = GetComponent<VRTK_ControllerEvents>();
    }

    void OnEnable()
    {
        //controllerEvents.ButtonTwoPressed += OnButtonTwoPressed;
        //controllerEvents.ButtonOnePressed += OnButtonTwoPressed;
    }

    //public void OnButtonTwoPressed(object sender, ControllerInteractionEventArgs args)
    //{
    //    this.enabled = false;
    //}

    private void OnDisable()
    {
        //controllerEvents.ButtonTwoPressed -= OnButtonTwoPressed;
        //controllerEvents.ButtonOnePressed -= OnButtonTwoPressed;
    }
}
