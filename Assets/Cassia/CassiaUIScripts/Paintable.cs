using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;

public class Paintable : MonoBehaviour
{
    public ObjectManipulator interactableObject;
    public MeshRenderer renderer;
    public string particleIdentity = "Unspecified";

    public bool permanentPaint = false;
    
//    
//    public enum Particle
//    {
//        Power,
//        Wisdom,
//        Courage,
//        Tagalong,
//        Negalong,
//        Pool,
//        SineNeg,
//        SinePos
//    }
//
//    public Particle particleType;
    
    private void OnValidate()
    {
        interactableObject = GetComponent<ObjectManipulator>();
        renderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        interactableObject.OnManipulationStarted.AddListener(PaintObject);   //; += PaintObject;
    }

    private void OnEnable()
    {
        PaintManager.Instance.RegisterPaintable(this);
    }

    private void OnDisable()
    {
        PaintManager.Instance.UnregisterPaintable(this);
    }

    public void PaintPermanent(Material material)
    {
        renderer.material = material;
        permanentPaint = true;
    }
    
    private void PaintObject(ManipulationEventData eventArgs)
    {
        if (permanentPaint)
        {
            return;
        }
        PaintTool paintTool = eventArgs.Pointer.BaseCursor.GameObjectReference.GetComponent<PaintTool>();
        if (paintTool)
        {
            if (paintTool.enabled)
            {
                renderer.material = paintTool.paintingMaterial;
            }
        }
    }
}