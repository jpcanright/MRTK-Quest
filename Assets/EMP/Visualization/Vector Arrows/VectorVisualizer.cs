using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VectorVisualizer : MonoBehaviour
{
    /// <summary>
    /// Prefab holding a MeshFilter and MeshRenderer of a vector arrow, to be instantiated on awake.
    /// </summary>
    [Tooltip("Prefab holding a MeshFilter and MeshRenderer of a vector arrow, to be instantiated on awake.")]
    public GameObject vectorPrefab;
    public Vector3 targetQuantity = Vector3.one;
    public Func<Vector3, Vector3> quantityFunc;

    /// <summary>
    /// Set to True to manually set TargetQuantity via the inspector.
    /// </summary>
    public bool overrideQuantityFunc;

    public bool setInactiveOnQuantityZero;

    public float minimumArrowWidth = 0.01f;
    public float maximumArrowWidth = 0.05f;
    public float maxMagnitude = 20;
    
    public Color arrowColor = Color.black;
    
    /// <summary>
    /// GameObject holding the actual MeshRenderer for the vector arrow.
    /// </summary>
    protected GameObject visVector;

    public float scaleFactor = 5e-6F;

    public enum VectorScaling
    {
        Linear,
        Logarithmic,
        Sigmoidal
    }

//    public enum VectorAnchor
//    {
//        Tail
//    }
    
    [SerializeField]
    protected VectorScaling scaling = VectorScaling.Linear;

    //[SerializeField]
    //protected VectorAnchor anchor = VectorAnchor.Tail;
    
    public VectorScaling Scaling
    {
        get { return scaling; }
        set { scaling = value; }
    }

    

    // Use this for initialization
    protected void Awake()
    {
        if (transform.childCount > 0)
        {
            visVector = transform.GetChild(0).gameObject;
        }
        else
        {
            // Instantiate actual vector
            visVector = Instantiate(vectorPrefab, transform);
        }

        SetArrowColor();
    }

    protected void SetArrowColor()
    {
        // Color the vector appropriately
        if (Application.isPlaying)
        {
            visVector.GetComponent<MeshRenderer>().material.color = arrowColor;
        }
    }

    protected void SetArrowDirectionAndScale()
    {
        // Do nothing if the vector has no quantity to represent and is not being set manually.
        if (!overrideQuantityFunc)
        {
            if (quantityFunc == null) return;
            targetQuantity = quantityFunc(transform.position);
        }

        //Vector3 renderedQuantity = targetQuantity * scaleFactor;
        
        visVector.transform.localPosition = new Vector3(0, 0, 3);

        if (targetQuantity.magnitude <= 0.001)
        {
            targetQuantity = Vector3.zero;
        }
        
        //if (renderedQuantity == Vector3.zero)
        if (targetQuantity == Vector3.zero)
        {
            if (setInactiveOnQuantityZero)
            {
                //visVector.SetActive(false);
            }
            
            return;
        }
        
        // Determine how parent transform should be rotated and scaled
        Quaternion vectorDirection = Quaternion.identity;
        float vectorMagnitude = 0;
        
        visVector.SetActive(true);
        
        vectorDirection = Quaternion.LookRotation(targetQuantity, Vector3.up);
        
        if (Scaling == VectorScaling.Linear)
        {
            vectorMagnitude = targetQuantity.magnitude * scaleFactor;
        }
        else if (Scaling == VectorScaling.Sigmoidal)
        {
            vectorMagnitude = LogisticScale(targetQuantity.magnitude * scaleFactor / maxMagnitude) * maxMagnitude;
        }

        if (vectorMagnitude > maxMagnitude)
        {
            vectorMagnitude = maxMagnitude;
        }
        
        
        // Apply scaling to parent object
        transform.rotation = vectorDirection;
        transform.localScale = Vector3.one * vectorMagnitude;
        
        // Even scaling can make for a very large, wide arrow; here we scale the x and y scale of the child transform
        // down if the arrow would get wider than a certain threshold
        
        float childScale =
            Mathf.Lerp(minimumArrowWidth, maximumArrowWidth,
                (vectorMagnitude) /
                maxMagnitude) / transform.localScale.x;
        visVector.transform.localScale = new Vector3(childScale, childScale, visVector.transform.localScale.z);
        
        if (transform.localScale.x * visVector.transform.localScale.x > maximumArrowWidth)
        {
            childScale = maximumArrowWidth / transform.localScale.x;
            visVector.transform.localScale = new Vector3(childScale, childScale, visVector.transform.localScale.z);
        }
        // Likewise, the arrow can become so tiny as to be difficult to see, so we set a minimum bound on its width
        else if (transform.localScale.x * visVector.transform.localScale.x < minimumArrowWidth)
        {
            childScale = minimumArrowWidth / transform.localScale.x;
            visVector.transform.localScale = new Vector3(childScale, childScale, visVector.transform.localScale.z);
        }
        else
        {
            childScale =
                Mathf.Lerp(minimumArrowWidth, maximumArrowWidth,
                    (transform.localScale.x * visVector.transform.localScale.x - minimumArrowWidth) /
                    (maximumArrowWidth - minimumArrowWidth)) / transform.localScale.x;
            visVector.transform.localScale = new Vector3(childScale, childScale, visVector.transform.localScale.z);
        }
    }

    public static float LogisticScale(float x)
    {
        // See Mathematica workbook in docs to show what these do
        float A = 0f;
        float K = 1f;
        float C = 1f;
        float B = 4f;
        float nu = 1;
        float Q = 10;
        return A + (K - A) / Mathf.Pow(C + Q * Mathf.Exp(-B * x), 1 / nu);
    }
    
    public static float Sigmoid(float x)
    {
        return 1f / (1 + Mathf.Exp(-x));
    }
    
    // TODO disable vector renderer if quantityFunc not set
    protected void Update()
    {
        SetArrowDirectionAndScale();
    }
}

