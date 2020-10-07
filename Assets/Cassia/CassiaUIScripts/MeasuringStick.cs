using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class MeasuringStick : MonoBehaviour, IResettable
{
    private Vector3 startPoint;
    [SerializeField] private Transform startMarker;
    [SerializeField] private Transform endMarker;
    [SerializeField] private LineRenderer renderer;
    [SerializeField] private QuantityDisplayBoard quantityDisplay;


    public float MeasuredLength()
    {
        return (startMarker.position - endMarker.position).magnitude;
    }
    
    // Start is called before the first frame update
    void OnEnable()
    {
        quantityDisplay.SetQuantityFunction(MeasuredLength);
    }

    // Update is called once per frame
    void Update()
    {
        var startPosition = startMarker.position;
        var endPosition = endMarker.position;
        renderer.SetPositions(new Vector3[] {startPosition, endPosition});
        quantityDisplay.transform.position = (startPosition + endPosition) / 2;
    }

    public void OnSceneReset()
    {
        Destroy(gameObject);
    }
}
