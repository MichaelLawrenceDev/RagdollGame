using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Targeting : MonoBehaviour
{
    [Header("Initilization")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform turretBarrel;
    [SerializeField] Transform projectileSpawnPoint;
    Transform turretStand;
    [SerializeField] Vector3 offset;

    [Header("Turret Properties")]
    [SerializeField] bool turretActive = true;
    [SerializeField] Transform target;
    [SerializeField] float hostileDistance;
    [SerializeField] float lookingDistance;
    [SerializeField] float rotationSpeed;

    [Header("Bullet Properties")]
    [SerializeField] bool bulletDrop = true;
    [SerializeField] float velocity = 20f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] float destroyIn = 5f;

    Vector3 targetDeactiveEuler = new Vector3(115f, -90f, 0f);
    Vector3 idleBarrelEuler = new Vector3(90f, -90f, 0f);
    Vector3 targetIdleEuler;
    float timeToFire = 0f;
    float timeTillNextIdle = 0f;
    bool doneRotating = false;
    bool deactiveMoving = true;
    private void Start()
    {
        turretStand = turretBarrel.parent;
    }
    private void Update()
    {
        // determine state logic
        if (turretActive)
        {
            deactiveMoving = true;
            Vector3 relativePos = target.position - turretBarrel.position;
            float distance = relativePos.magnitude;

            if (distance <= lookingDistance)
            {
                if (RotateToTarget(relativePos) && (distance <= hostileDistance))
                    FireBullet();
            }
            else
                IdleRotation();
        }
        else if (deactiveMoving)
            RotateToDeactiveState();
    }
    private bool RotateToTarget(Vector3 relativePos)
    {
        // rotate turret to target, returns true if lined up with target
        Vector3 lookAt = Quaternion.LookRotation(relativePos, Vector3.up).eulerAngles;
        lookAt += offset;
        float step = rotationSpeed * Time.deltaTime;

        float gravOffset = bulletDrop ? -5f : 0f;
        Quaternion targetBarrelRot = Quaternion.Euler(new Vector3(lookAt.x + gravOffset, -offset.y, lookAt.z));
        Quaternion targetStandRot = Quaternion.Euler(new Vector3(0f, lookAt.y, 0f));

        turretBarrel.localRotation = Quaternion.RotateTowards(turretBarrel.localRotation, targetBarrelRot, step);
        turretStand.rotation = Quaternion.RotateTowards(turretStand.rotation, targetStandRot, step);

        return turretBarrel.localRotation == targetBarrelRot && turretStand.rotation == targetStandRot;
    }

    private void RotateToDeactiveState()
    {
        // Rotates to deactive stance (looks down)
        float step = rotationSpeed * Time.deltaTime;
        turretBarrel.localRotation = Quaternion.RotateTowards(turretBarrel.localRotation, Quaternion.Euler(targetDeactiveEuler), step);

        deactiveMoving = turretBarrel.localRotation != Quaternion.Euler(targetDeactiveEuler);
    }

    private void IdleRotation()
    {
        float step = rotationSpeed * Time.deltaTime;

        if (timeTillNextIdle <= Time.time)
        {
            // set target rotation, and start rotating turret
            doneRotating = false;
            float degreeIncrement = Random.Range(30f, 180f);
            targetIdleEuler = turretStand.eulerAngles;
            targetIdleEuler.y += Random.Range((int)0, (int)2) == 0 ? degreeIncrement : -degreeIncrement;

            timeTillNextIdle = Time.time + 4f;
        }
        else if (!doneRotating)
        {
            turretBarrel.localRotation = Quaternion.RotateTowards(turretBarrel.localRotation, Quaternion.Euler(idleBarrelEuler), step);
            turretStand.rotation = Quaternion.RotateTowards(turretStand.rotation, Quaternion.Euler(targetIdleEuler), step);

            doneRotating = turretStand.rotation == Quaternion.Euler(targetIdleEuler) && turretBarrel.localRotation == Quaternion.Euler(idleBarrelEuler);
        }
    }

    private void FireBullet()
    {
        if (timeToFire <= Time.time)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = projectileSpawnPoint.position;
            bullet.transform.rotation = projectileSpawnPoint.rotation;

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.useGravity = bulletDrop;
            rb.AddForce(bullet.transform.up * velocity, ForceMode.VelocityChange);

            Sound sound = Random.Range((int)0, (int)2) == 0 ? Sound.CannonShot1 : Sound.CannonShot2;
            AudioManager.PlaySound(sound, projectileSpawnPoint.position);

            Destroy(bullet, destroyIn);
            timeToFire = Time.time + fireRate;
        }
    }
}
