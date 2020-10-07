using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Steps;

public class SynchronizeMeatspaceBench : MonoBehaviour
{

    [SerializeField] private Transform leftControllerTip;
    [SerializeField] private Transform leftControllerHoop;
    [SerializeField] private Transform leftControllerHandle;
    [SerializeField] private Transform rightControllerTip;
    [SerializeField] private Transform rightControllerHoop;
    // TODO add right controller handle
    
    // Set only X and Z components. +X is to the right along the desk, +Z is farther down the desk.
    [SerializeField] private Vector3 leftTipCornerOffset;
    [SerializeField] private Vector3 rightTipCornerOffset;

    [SerializeField] private Transform benchObject;
    [SerializeField] private float benchHeightInMeters = 0.9906f;

    private Vector3 leftBenchCorner;
    private Vector3 rightBenchCorner;
    
    public bool snapPlayerToExistingBench = false;
    // Update is called once per frame
    void Update()
    {
        //Synchronize();
    }

    public void Synchronize()
    {
        // Check that left controller is upside down; if it is not, don't proceed.
        // TODO also check right controller
        if (leftControllerHandle.position.y < leftControllerTip.position.y)
        {
            return;
        }
        
        //// Determine corners of physical bench in world space from controller positions
        Vector3 leftPoint = leftControllerTip.position;
        Vector3 rightPoint = rightControllerTip.position;

        // Determine meatspace bench depth axis direction vector

        // Vectors pointing from each controller's hoop to its tip
        Vector3 leftDirection = leftPoint - leftControllerHoop.position;
        Vector3 rightDirection = rightPoint - rightControllerHoop.position;

        // Project each onto the XZ plane and normalize
        leftDirection = Vector3.ProjectOnPlane(leftDirection, Vector3.up).normalized;
        rightDirection = Vector3.ProjectOnPlane(rightDirection, Vector3.up).normalized;

        // Check that our angle isn't too big
        if (Vector3.Angle(leftDirection, rightDirection) > 20f)
        {
            Debug.LogError(
                $"Left and right controllers' direction vectors too far apart: {Vector3.Angle(leftDirection, rightDirection).ToString()} degrees.");
            return;
        }
        else
        {
            Debug.Log(
                $"Left and right controllers' direction vectors acceptably close: {Vector3.Angle(leftDirection, rightDirection).ToString()} degrees.");
        }

        // Take average of vectors
        Vector3 benchDepthAxis = Vector3.Slerp(leftDirection, rightDirection, 0.5f).normalized;

        // Create rotation from VR space to meatspace
        Quaternion sceneToMeatSpace = Quaternion.FromToRotation(Vector3.forward, benchDepthAxis);

        // Apply rotation to tip offset vectors
        Vector3 rotatedLeftTipCornerOffset = sceneToMeatSpace * leftTipCornerOffset;
        Vector3 rotatedRightTipCornerOffset = sceneToMeatSpace * rightTipCornerOffset;

        leftBenchCorner = leftPoint + rotatedLeftTipCornerOffset;
        rightBenchCorner = rightPoint + rotatedRightTipCornerOffset;

        // Debugging code
#if UNITY_EDITOR
        Debug.DrawLine(leftPoint, leftControllerHoop.position, Color.red);
        Debug.DrawLine(leftPoint, leftBenchCorner, Color.red);
        Debug.DrawLine(rightPoint, rightControllerHoop.position, Color.blue);
        Debug.DrawLine(rightPoint, rightBenchCorner, Color.blue);
#endif

        // Match bench rotation
        benchObject.rotation = sceneToMeatSpace;

        // Determine where bench position should be
        // Let's leave the y-component alone for this lab
        // X and Z should be at the midpoint of the corners
        Vector3 benchMidpoint = (leftBenchCorner + rightBenchCorner) / 2f;
        //benchMidpoint.y = benchObject.position.y;
        benchMidpoint.y /= 2f;
        benchObject.position = benchMidpoint;

        // Determine bench scale
        // This assumes that the bench is not scaled by a parent

        float benchDepth = (Vector3.Project(rightBenchCorner - leftBenchCorner, benchDepthAxis)).magnitude;
        float benchWidth = Mathf.Sqrt((rightBenchCorner - leftBenchCorner).sqrMagnitude - benchDepth * benchDepth);
        float benchHeight = (leftBenchCorner.y + rightBenchCorner.y) / 2f;
        //Vector3 newBenchScale = new Vector3(benchWidth, benchObject.localScale.y, benchDepth);
        Vector3 newBenchScale = new Vector3(benchWidth, benchHeight, benchDepth);

        benchObject.localScale = newBenchScale;
    }
}
