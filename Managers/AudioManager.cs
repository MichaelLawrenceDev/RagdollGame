using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

// Reference: https://www.youtube.com/watch?v=QL29aTa7J5Q

public enum Sound
{
    // Add key words here, then adjust in Game Assets (in resources folder)
    BluntThud1, BluntThud2,     
    WoodenThunk1, WoodenThunk2,
    HollowThud1, HollowThud2,
    MetalThud,
    CinematicThud,
    Footstep1, Footstep2, Footstep3,
    CannonShot1, CannonShot2,
    Launch1, Launch2
}
public enum Music
{
}

public static class AudioManager
{
    private static Dictionary<Sound, float> soundTimerDict;
    private static GameObject oneShotObj;
    private static GameObject musicObj;
    private static AudioSource oneShotAudio;
    

    public static void PlaySound(Sound type)
    {
        if (oneShotObj == null)
        {
            oneShotObj = new GameObject("Sound");
            oneShotObj.tag = "sound";
            oneShotAudio = oneShotObj.AddComponent<AudioSource>();
        }
        GameAssets.SoundAudioClip SAC = getAudioClip(type);
        oneShotAudio.PlayOneShot(SAC.audioClip, SAC.volume);
    }
    public static float PlayMusic(Music type)
    {
        AudioSource audioSource;
        if (musicObj == null) // if no music playing...
        {
            musicObj = new GameObject("Music");
            musicObj.tag = "music";
            audioSource = musicObj.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = musicObj.GetComponent<AudioSource>();
        }
        GameAssets.MusicAudioClip MAC = getAudioClip(type);

        // Settings
        audioSource.clip = MAC.audioClip;
        audioSource.volume = MAC.volume;
        audioSource.pitch = MAC.pitch;
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        audioSource.Play();

        // return time when song ends
        return audioSource.clip.length;
    }
    public static void PlaySound(Sound type, Vector3 pos, float volume = 0f, float pitch = 0f, bool loop = false)
    {
        GameObject soundObj = new GameObject("Sound");
        soundObj.tag = "sound";
        soundObj.transform.position = pos;
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        GameAssets.SoundAudioClip SAC = getAudioClip(type);

        // Settings
        if (volume == 0)
            audioSource.volume = SAC.volume;
        else
            audioSource.volume = volume;
        if (pitch == 0)
            audioSource.pitch = SAC.pitch;
        else
            audioSource.pitch = pitch;

        audioSource.clip = SAC.audioClip;
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        audioSource.Play();
        audioSource.loop = loop;

        // destroy one time sound
        if (!loop) Object.Destroy(soundObj, audioSource.clip.length);
    }
    public static void PlayRandomSound(List<Sound> soundList, Vector3 pos, float volume = 0f, float pitch = 0f, bool loop = false)
    {
        if (soundList.Count == 0)
            Debug.LogError("Cannot play sound from empty list!");
        else
        {
            int ran = Random.Range(0, soundList.Count);
            PlaySound(soundList.ElementAt(ran), pos, volume, pitch, loop);
        } 
    }
    public static float getSoundLength(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip obj in GameAssets.i.audioClipArray)
            if (obj.sound == sound)
                return obj.audioClip.length;

        Debug.LogError("Sound " + sound + " does not exist.");
        return 0;
    }
    private static GameAssets.SoundAudioClip getAudioClip(Sound type)
    {
        foreach (GameAssets.SoundAudioClip soundObj in GameAssets.i.audioClipArray)
            if (soundObj.sound == type)
                return soundObj;

        Debug.LogError("Sound " + type + " does not exist.");
        return null;
    }
    private static GameAssets.MusicAudioClip getAudioClip(Music type)
    {
        foreach (GameAssets.MusicAudioClip soundObj in GameAssets.i.musicClipArray)
            if (soundObj.sound == type)
                return soundObj;

        Debug.LogError("Music " + type + " does not exist.");
        return null;
    }
}

