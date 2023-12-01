using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundPayload
    {
        public AudioClip Clip;
        [Range(0, 1.0f)]
        public float Volume = 0.5f;
        //added these two for cooldown stuff:
        public float CooldownTime = 5f;
        public float LastPlayTime;
    }

    public GameObject AudioPlayerPrefab;
    public AudioSource music;
    private AudioPlayer gurtSoundPlayer;

    [Header("Willow Actions")]
    public SoundPayload[] WillowAttackSounds;
    public SoundPayload[] WillowKillSounds;
    public SoundPayload[] WillowStruckSounds;
    public AudioSource WillowWalkSounds;
    public SoundPayload WillowSprintSounds;
    public SoundPayload WillowJumpSounds;

    public void OnWillowAttackSounds(Vector3 position) => PlayRandomSound(WillowAttackSounds, position);
    public void OnWillowKillSounds(Vector3 position) => PlayRandomSound(WillowKillSounds, position);
    public void OnWillowStruckSounds(Vector3 position) => PlayRandomSound(WillowStruckSounds, position);
    public void OnWillowSprintSounds(Vector3 position) => PlaySound(WillowSprintSounds, position);
    public void OnWillowJumpSounds(Vector3 position) => PlaySound(WillowJumpSounds, position);


    [Header("Willow Quips")]
    public SoundPayload WillowGameStart;
    public SoundPayload WillowMusings;
    public SoundPayload[] WillowAttackQuips;
    public SoundPayload[] WillowStruckQuips;

    public void OnWillowGameStart(Vector3 position) => PlaySound(WillowGameStart, position);
    public void OnWillowMusings(Vector3 position) => PlaySound(WillowMusings, position);
    public void OnWillowAttackQuips(Vector3 position) => PlayRandomSound(WillowAttackQuips, position);
    public void OnWillowStruckQuips(Vector3 position) => PlayRandomSound(WillowStruckQuips, position);

    [Header("Projectile Sounds")]
    public SoundPayload[] BulletShootSounds;
    public SoundPayload[] LobShootSounds;
    public SoundPayload[] ExplosionSounds;
    public SoundPayload[] StickShootSounds;
    public SoundPayload[] StickHitSounds;

    public void OnBulletShootSounds(Vector3 position) => PlayRandomSound(BulletShootSounds, position);
    public void OnLobShootSounds(Vector3 position) => PlayRandomSound(LobShootSounds, position);
    public void OnExplosionSounds(Vector3 position) => PlayRandomSound(ExplosionSounds, position);
    public void OnStickShootSounds(Vector3 position) => PlayRandomSound(StickShootSounds, position);
    public void OnStickHitSounds(Vector3 position) => PlayRandomSound(StickHitSounds, position);


    [Header("Bot Actions")]
    public SoundPayload[] BotAttackSounds;
    public SoundPayload[] BotStruckSounds;
    public SoundPayload[] BotStruckQuips;


    public void OnBotAttackSounds(Vector3 position) => PlayRandomSound(BotAttackSounds, position);
    public void OnBotStruckSounds(Vector3 position) => PlayRandomSound(BotStruckSounds, position);
    public void OnBotStruckQuips(Vector3 position) => PlayRandomSound(BotStruckQuips, position);

    [Header("Bot Quips")]
    public SoundPayload[] BotIdleSounds;
    public SoundPayload[] BotPatrollingSounds;
    public SoundPayload[] BotLocatesTargetSounds;
    public SoundPayload[] BotLosesTargetSounds;

    public void OnBotIdleSounds(Vector3 position) => PlayRandomSound(BotIdleSounds, position);
    public void OnBotPatrollingSounds(Vector3 position) => PlayRandomSound(BotPatrollingSounds, position);
    public void OnBotLocatesTargetSounds(Vector3 position) => PlayRandomSound(BotLocatesTargetSounds, position);
    public void OnBotLosesTargetSounds(Vector3 position) => PlayRandomSound(BotLosesTargetSounds, position);

    [Header("Gurt")]
    public SoundPayload GurtSounds;
    public SoundPayload GurtAussieSounds;
    public void OnGurtSounds(Vector3 position) => PlaySound(GurtSounds, position);
    public void OnGurtAussieSounds(Vector3 position) => PlaySound(GurtAussieSounds, position);


    //cooldown code here:

    private bool CanPlaySound(SoundPayload sound)
    {
        return (Time.time - sound.LastPlayTime) >= sound.CooldownTime;
    }

    private void UpdateLastPlayTime(SoundPayload sound)
    {
        sound.LastPlayTime = Time.time;
    }


    // edited these to add cooldown code
    public void PlaySound(SoundPayload sound, Vector3 position)
    {
        if (CanPlaySound(sound))
        {
            GameObject player = Instantiate(AudioPlayerPrefab, position, Quaternion.identity);
            AudioPlayer ap = player.GetComponent<AudioPlayer>();
            if (ap != null)
            {
                ap.Play(sound);
            }
            UpdateLastPlayTime(sound);
        }

    }
    public void PlayRandomSound(SoundPayload[] sounds, Vector3 position)
    {
        if (sounds.Length > 0)
        {
            List<SoundPayload> availableSounds = new List<SoundPayload>();

            foreach (var sound in sounds)
            {
                availableSounds.Add(sound);
            }

            if (availableSounds.Count > 0)
            {
                int idx = Random.Range(0, availableSounds.Count);
                SoundPayload sound = availableSounds[idx];

                PlaySound(sound, position);
            }
        }
    }

    public void PlayWalkSound()
    {
        WillowWalkSounds.mute = false;
    }

    public void PauseWalkSound()
    {
        WillowWalkSounds.mute = true;
    }

    public void PlayMusic()
    {
        music.Play();
    }

    public void PauseMusic()
    {
        music.Pause();
    }

    public void DimMusic(float volume)
    {
        music.volume = volume;
    }

    public void DimGlobalVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void PlayGurtSounds()
    {
        GameObject player = Instantiate(AudioPlayerPrefab);
        AudioPlayer ap = player.GetComponent<AudioPlayer>();
        if (ap != null)
        {
            ap.Play(GurtSounds);
        }
        gurtSoundPlayer = ap;
    }

    public void MuteGurtSounds()
    {
        if (gurtSoundPlayer == null) return;
        gurtSoundPlayer.Pause();
    }
}
