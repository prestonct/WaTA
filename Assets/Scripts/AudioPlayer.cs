using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    AudioSource source;
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    public void Play(AudioManager.SoundPayload sound)
    {
        source.volume = sound.Volume;
        source.PlayOneShot(sound.Clip);
        Destroy(gameObject, sound.Clip.length);
    }

    public void Pause()
    {
        source.Pause();
    }
}