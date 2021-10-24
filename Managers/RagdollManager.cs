using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    public bool controllable;
    [Header("Knock Out Variables")]
    [SerializeField] float headRecoveryTime = 3f;
    [SerializeField] float headKnockOutSpeed = 5f;
    [Header("Collision Sound Variables")]
    [SerializeField] float footstepDelay = 0.3f;
    [SerializeField] float minimumSpeed;
    [Range(0, 20)]
    [SerializeField] float speedForMaxVolume;
    [Range(0.01f, 1f)]
    [SerializeField] float maxVolume = 1f;

    bool knockedOut = false;
    int soundsLimit = 3;
    bool leftFootPlayingSound = false;
    bool rightFootPlayingSound = false;
    int currentNumOfSounds = 0;
    int rightGroundTriggerCount = 0;
    int leftGroundTriggerCount = 0;
    Animator animator;

    private void Awake()
    {
        // initilization...
        animator = GetComponent<Animator>();

        // Rigidbody detection scripts (recieve collision info from specific body parts)
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {   
            if (rb.gameObject != gameObject)
            {
                RagdollCollisionDetector rcd = rb.gameObject.AddComponent<RagdollCollisionDetector>();
                rcd.ParentTo(this);
            }
        }
    }
    private void Update()
    {
        if (controllable)
        {
            // Controls
            if (Input.GetKeyDown(KeyCode.Alpha1))
                animator.SetBool("limp", !animator.GetBool("limp"));
            if (Input.GetKeyDown(KeyCode.Alpha2))
                animator.SetTrigger("cheer");
            if (Input.GetKeyDown(KeyCode.Space))
                animator.SetTrigger("jump");

            animator.SetBool("walking", Input.GetKey(KeyCode.W));
        }
    }
    public void PartHasCollided(float speed, PartType type)
    {
        if (type != PartType.head)
            PlayCollisionSound(speed, type);

        switch (type)
        {
            case PartType.head:
                if (speed >= headKnockOutSpeed && !knockedOut)
                    StartCoroutine(KnockOut());
                break;
        }
    }
    public void FootLeftGround(PartType type)
    {
        switch (type)
        {
            case PartType.leftFoot:
                leftGroundTriggerCount--;
                if (leftGroundTriggerCount == 0) animator.SetBool("leftFootOnGround", false);
                break;
            case PartType.rightFoot:
                rightGroundTriggerCount--;
                if (rightGroundTriggerCount == 0) animator.SetBool("rightFootOnGround", false);
                break;
        }
    }
    public void FootOnGround(PartType type)
    {
        switch (type)
        {
            case PartType.leftFoot:
                leftGroundTriggerCount++;
                animator.SetBool("leftFootOnGround", true);
                break;
            case PartType.rightFoot:
                rightGroundTriggerCount++;
                animator.SetBool("rightFootOnGround", true);
                break;
        }
    }
    private void PlayCollisionSound(float speed, PartType type)
    {
        // other sounds...
        if (speed >= minimumSpeed)
        {
            float volume = speed > speedForMaxVolume ? maxVolume : (speed / speedForMaxVolume) * maxVolume;
            float pitch = Random.Range(0.9f, 1.1f);

            // footstep
            if (type == PartType.leftFoot || type == PartType.rightFoot)
                StartCoroutine(PlayFootstep(type, volume, pitch));
            // other parts
            else if (volume < maxVolume && currentNumOfSounds != soundsLimit)
                StartCoroutine(PlayQuietSound(type, volume, pitch)); // limit small sounds to 3
            else if (volume == maxVolume)
                AudioManager.PlaySound(GetPartSound(type), transform.position, volume, pitch); // no limit on massive impact sounds
        }
    }
    private Sound GetPartSound(PartType part)
    {
        int ran;
        Sound sound;
        switch (part)
        {
            case PartType.leftFoot:
            case PartType.rightFoot:
                ran = Random.Range(1, 4);
                if (ran == 1)       
                    sound = Sound.Footstep1;
                else if (ran == 2)  
                    sound = Sound.Footstep2;
                else                
                    sound = Sound.Footstep3;
                break;

            default:
                ran = Random.Range(1, 3);
                sound = ran == 1 ? Sound.BluntThud1 : Sound.BluntThud2;
                break;
        }
        return sound;
    }
    public void GoLimp() { animator.SetBool("limp", true); }
    IEnumerator PlayQuietSound(PartType part, float volume, float pitch)
    {
        currentNumOfSounds++;

        Sound sound = GetPartSound(part);
        AudioManager.PlaySound(sound, transform.position, volume, pitch);
        yield return new WaitForSeconds(AudioManager.getSoundLength(sound));

        currentNumOfSounds--;
    }
    IEnumerator PlayFootstep(PartType foot, float volume, float pitch)
    {
        Sound sound = GetPartSound(foot); 

        // left foot
        if (foot == PartType.leftFoot && !leftFootPlayingSound)
        {
            leftFootPlayingSound = true;

            AudioManager.PlaySound(sound, transform.position, volume, 0.7f);
            yield return new WaitForSeconds(footstepDelay);

            leftFootPlayingSound = false;
        }
        // right foot
        else if (!rightFootPlayingSound)
        {
            rightFootPlayingSound = true;

            AudioManager.PlaySound(sound, transform.position, volume, 1f);
            yield return new WaitForSeconds(footstepDelay);

            rightFootPlayingSound = false;
        }
    }
    IEnumerator KnockOut()
    {
        AudioManager.PlaySound(Sound.WoodenThunk1, transform.position, 1f);
        animator.SetBool("limp", true);
        knockedOut = true;

        yield return new WaitForSeconds(headRecoveryTime);

        animator.SetBool("limp", false);
        knockedOut = false;
    }
}
