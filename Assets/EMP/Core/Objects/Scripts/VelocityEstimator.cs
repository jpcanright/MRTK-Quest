using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityEstimator : MonoBehaviour
{
    
    public Vector3 Velocity { get; private set; }

    private Vector3[] velocities;

    private int currentIndex = 0;
    private Vector3 cachedPosition;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        velocities = new Vector3[5];
        for (int i = 0; i < velocities.Length; i++)
        {
            velocities[i] = Vector3.zero;
        }

        cachedPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 currentVelocity = (transform.position - cachedPosition) / Time.fixedDeltaTime;
        cachedPosition = transform.position;
        velocities[currentIndex] = currentVelocity;
        currentIndex++;
        currentIndex = currentIndex % velocities.Length;
        Vector3 velocitySum = Vector3.zero;
        for (int i = 0; i < velocities.Length; i++)
        {
            velocitySum += velocities[i];
        }

        velocitySum /= velocities.Length;
        Velocity = velocitySum;
    }
}
