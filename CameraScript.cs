using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject followTarget;
    [SerializeField] GameObject camera;
    [SerializeField] Vector3 offset;
    [SerializeField] float sensitivity;
    [SerializeField] float smoothSpeed;
    [SerializeField] LayerMask layerMask;

    float clampAdjust;
    float distanceFromCenter;
    Vector3 currentRotation;
    Vector3 currentPosition;
    Vector3 targetPosition;
    float numFrames = 45f;
    float elapsedFrames = 0;
    bool hasOffset = false;

    void Start()
    {
        currentRotation = transform.eulerAngles;
        hasOffset = offset != Vector3.zero;
        if (hasOffset)
        {
            camera.transform.localPosition = offset;
            // adjust clamp amount based on offset angle
            clampAdjust = Mathf.Atan2(offset.y, offset.z) * Mathf.Rad2Deg;
            distanceFromCenter = Vector3.Distance(Vector3.zero, offset) * 1.05f;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (followTarget != null)
            SmoothPosition(); // Smoothly move pivot to ragdoll

        UpdateRotate();  // Update camera position based on input
        if (hasOffset)
            MoveCameraOnCollision();
    }
    void UpdateRotate()
    {
        // Update currentRotation with player input
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        if (x != 0 || y != 0)
        {
            Vector3 inputVector = new Vector3(y * sensitivity, x * sensitivity, 0);
            currentRotation = currentRotation + inputVector;
        }
        currentRotation.x = Mathf.Clamp(currentRotation.x, -80 + clampAdjust, 50 + clampAdjust);
        transform.eulerAngles = currentRotation;
    }

    void SmoothPosition()
    {
        Vector3 smoothPos = Vector3.Lerp(transform.position, followTarget.transform.position, smoothSpeed);
        transform.position = smoothPos;
    }

    void MoveCameraOnCollision()
    {
        RaycastHit hit;

        Vector3 lookAt = (transform.position - camera.transform.position);
        //UnityEngine.Debug.DrawRay(transform.localPosition, -lookAt, Color.red);
        if (Physics.Raycast(transform.localPosition, -lookAt, out hit, distanceFromCenter, layerMask))
            camera.transform.localPosition = transform.InverseTransformPoint(hit.point) * 0.95f;

        else
            camera.transform.localPosition = offset;

    }
    public void setFollowTarget(GameObject obj) { followTarget = obj; }
}
