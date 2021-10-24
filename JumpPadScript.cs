using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class JumpPadScript : MonoBehaviour
{
    List<GameObject> collidedObjs;
    List<Sound> sounds;
    float nextTimeToLaunch;
    [SerializeField] float resetSpeed;
    [SerializeField] float launchAngle;
    [SerializeField] float forceAmount;

    [SerializeField] bool velocityChangeInsteadOfForce;
    [SerializeField] bool ragdollsGoLimp;
    [SerializeField] bool soundless;
    private void Start()
    {
        sounds = new List<Sound>();
        sounds.Add(Sound.Launch1);
        sounds.Add(Sound.Launch2);
        collidedObjs = new List<GameObject>();
        nextTimeToLaunch = 0f;
    }
    private void Update()
    {
        //string text = "List: [ ";
        //foreach (GameObject obj in collidedObjs)
        //text += obj.name + ", ";

        //Debug.Log(text + "]");

        if (Time.time > nextTimeToLaunch && collidedObjs.Count >= 1)
        {
            LaunchObjects();
            nextTimeToLaunch = Time.time + resetSpeed;
        }
    }

    private void LaunchObjects()
    {
        if (!soundless)
            AudioManager.PlayRandomSound(sounds, transform.position);

        foreach (GameObject obj in collidedObjs)
        {
            RagdollManager rm = obj.GetComponent<RagdollManager>();
            if (ragdollsGoLimp && rm != null)
                rm.GoLimp();

            foreach (Rigidbody rb in obj.GetComponentsInChildren<Rigidbody>())
            {
                rb.velocity = Vector3.zero;
                Vector3 trajectory = transform.TransformDirection(Quaternion.Euler(0,0, launchAngle) * Vector3.right);
                rb.AddForce
                    (trajectory * forceAmount,             // relative force direction...
                    velocityChangeInsteadOfForce ? ForceMode.VelocityChange : ForceMode.Force);     // Forcemode (add velocity or add force)
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool objIsCollidingAlready = false;
        foreach (GameObject obj in collidedObjs)
        {
            if (obj == other.transform.root.gameObject)
            {
                objIsCollidingAlready = true;
                break;
            }
        }

        if (!objIsCollidingAlready)
            collidedObjs.Add(other.transform.root.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        collidedObjs.Remove(other.transform.root.gameObject);
    }
}
