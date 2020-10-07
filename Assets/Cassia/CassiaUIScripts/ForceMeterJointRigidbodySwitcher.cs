using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If a Joint's connectedBody field is empty, it binds the object to a fixed point in space rather than doing nothing. Unfortunately, we need it to do nothing in that case in order to work with a VRTK_SnapDropZone which uses a joint. This script works around that limitation by setting connectedBody to the rigidbody of an otherwise empty child GameObject (configured separately).
/// </summary>
public class ForceMeterJointRigidbodySwitcher : MonoBehaviour
{
    [SerializeField] private Joint joint;
    [SerializeField] private Rigidbody defaultRigidbody;

    private void OnValidate()
    {
        joint = GetComponent<Joint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!joint.connectedBody)
        {
            joint.connectedBody = defaultRigidbody;
        }
    }
}
