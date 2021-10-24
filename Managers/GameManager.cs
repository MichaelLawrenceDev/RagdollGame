using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerRagdoll;
    [SerializeField] GameObject cameraPivot;
    [SerializeField] bool lockCursor = false;

    private void Start()
    {
        if (playerRagdoll != null)
        {
            playerRagdoll.GetComponent<RagdollManager>().controllable = true;
            cameraPivot.GetComponent<CameraScript>().setFollowTarget(playerRagdoll);
        }

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        // Return player to map if he falls off
        if (playerRagdoll.transform.position.y < -10f)
        {
            playerRagdoll.GetComponentInChildren<RigidbodyCalculator>().ResetVelocity();
            playerRagdoll.transform.position = new Vector3(-8f, 13f, 0);
        }
    }
}
