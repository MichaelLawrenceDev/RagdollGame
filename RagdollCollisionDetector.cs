using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PartType
{
    other,
    rightHand,
    leftHand,
    rightFoot,
    leftFoot,
    head
}

public class RagdollCollisionDetector : MonoBehaviour
{
    RagdollManager rm;
    Rigidbody rb;
    PartType part;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Determine body type
        if (gameObject.CompareTag("Untagged"))
        {
            part = PartType.other;
        }
        else
        {
            // determine partType of object                // initial calculations for easy comparisons later.
            if (gameObject.CompareTag("Left Hand"))        { part = PartType.leftHand; }
            else if (gameObject.CompareTag("Right Hand"))  { part = PartType.rightHand; }
            else if (gameObject.CompareTag("Left Foot"))   { part = PartType.leftFoot; }
            else if (gameObject.CompareTag("Right Foot"))  { part = PartType.rightFoot; }
            else                                           { part = PartType.head; }
        }
    }
    public void ParentTo(RagdollManager ragdollManager) 
    {
        rm = ragdollManager;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (transform.root != collision.transform.root)
            rm.PartHasCollided(collision.relativeVelocity.magnitude, part);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (part == PartType.leftFoot || part == PartType.rightFoot)
            rm.FootOnGround(part);
    }
    private void OnTriggerExit(Collider other)
    {
        // used by animator
        if (part == PartType.leftFoot || part == PartType.rightFoot)
            rm.FootLeftGround(part);
    }
}
