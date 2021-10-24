using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyCalculator : MonoBehaviour
{
    [SerializeField] Transform copyRotation;
    bool copyRotationIsNull = false;
    List<Rigidbody> rigidbodies;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = new List<Rigidbody>();
        foreach (Rigidbody rb in transform.parent.GetComponentsInChildren<Rigidbody>())
            rigidbodies.Add(rb);

        copyRotationIsNull = copyRotation == null;
    }

    // Update is called once per frame
    void Update()
    {
        // update position to center of mass
        Vector3 centerOfMass = Vector3.zero;
        float totalMass = 0f;
        foreach (Rigidbody rb in rigidbodies)
        {
            centerOfMass += rb.position * rb.mass;
            totalMass += rb.mass;
        }
        centerOfMass /= totalMass;
        transform.position = centerOfMass;

        // rotate based on camera
        if (!copyRotationIsNull)
        {
            Vector3 rot = copyRotation.eulerAngles;
            rot.x = 0;
            transform.eulerAngles = rot;
        }
    }
    public void ResetVelocity()
    {
        foreach (Rigidbody rb in rigidbodies)
            rb.velocity = Vector3.zero;
    }
}
