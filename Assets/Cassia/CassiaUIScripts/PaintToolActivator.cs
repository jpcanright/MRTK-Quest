using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using VRTK;

public class PaintToolActivator : MonoBehaviour
{
    //public VRTK_InteractableObject interactableObject;

    public MeshRenderer renderer;
    public Color onColor;
    public Color offColor;

    public bool isToggle = false;
    public bool toggleState = false;
    
    public GameObject offStateImage;
    public GameObject onStateImage;

    public Material paintingMaterial;
    public Color validCollisionColor;
    public Color invalidCollisionColor;
    
    private void OnValidate()
    {
        //interactableObject = GetComponent<VRTK_InteractableObject>();
        renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        //interactableObject.InteractableObjectUsed += ButtonPressed;
        UpdateAppearance();
    }

    private void Update()
    {
        UpdateAppearance();
    }

    void UpdateAppearance()
    {
        if (!renderer)
        {
            Debug.Log("No renderer found, returning.");
            return;
        }
        
        if (isToggle && toggleState)
        {
            //renderer.material.SetColor("_Color", onColor);
//            renderer.material.SetColor("_EmissionColor", onColor);
//            renderer.material.color = onColor;
            if (offStateImage)
            {
                offStateImage.SetActive(false);
            }

            if (onStateImage)
            {
                onStateImage.SetActive(true);
            }
        }
        else
        {
            //renderer.material.SetColor("_Color", offColor);
//            renderer.material.SetColor("_EmissionColor", onColor);
//            renderer.material.color = offColor;
            if (offStateImage)
            {
                offStateImage.SetActive(true);
            }

            if (onStateImage)
            {
                onStateImage.SetActive(false);
            }
        }
    }
    
//    private void ButtonPressed(object sendingObject, InteractableObjectEventArgs eventArgs)
//    {
////        buttonPressEvent.Invoke();
////        if (isToggle)
////        {
////            toggleState = !toggleState;
////        }
//        PaintTool paintTool = eventArgs.interactingObject.GetComponent<PaintTool>();
//        if (paintTool)
//        {
//            paintTool.enabled = true;
//            paintTool.paintingMaterial = paintingMaterial;
//            VariableLengthPointer pointer = eventArgs.interactingObject.GetComponent<VariableLengthPointer>();
//            if (pointer)
//            {
//                pointer.validCollisionColor = validCollisionColor;
//                pointer.invalidCollisionColor = invalidCollisionColor;
//            }
//        }
//    }
}