using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioChannel { Master, Music, Ambient, Vfx };

public class AudioComponent : MonoBehaviour
{
    public static AudioComponent Instance { get; set; }

    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float musicVolume = 1;
    [Range(0, 1)]
    public float ambientVolume = 1;
    [Range(0, 1)]
    public float fxVolume = 1;

    public bool musicIsLooping = true;
    public bool ambientIsLooping = true;

    AudioSource musicSource;
    AudioSource ambientSource;
    AudioSource fxSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        //==============================================================
        // Create audio sources
        //==============================================================
        GameObject newfxSource = new GameObject("2D Vfx Source");
        fxSource = newfxSource.AddComponent<AudioSource>();
        newfxSource.transform.parent = transform;
        fxSource.playOnAwake = false;

        GameObject newMusicSource = new GameObject("Music Source");
        musicSource = newMusicSource.AddComponent<AudioSource>();
        newMusicSource.transform.parent = transform;
        musicSource.loop = musicIsLooping; // Music is looping
        musicSource.playOnAwake = false;

        GameObject newAmbientsource = new GameObject("Ambient Source");
        ambientSource = newAmbientsource.AddComponent<AudioSource>();
        newAmbientsource.transform.parent = transform;
        ambientSource.loop = ambientIsLooping; // Ambient sound is looping
        ambientSource.playOnAwake = false;

        //==============================================================
        // Set volume on all the channels
        //==============================================================
        SetVolume(masterVolume, AudioChannel.Master);
        SetVolume(fxVolume, AudioChannel.Vfx);
        SetVolume(musicVolume, AudioChannel.Music);
        SetVolume(ambientVolume, AudioChannel.Ambient);
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolume = volumePercent;
                break;
            case AudioChannel.Vfx:
                fxVolume = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolume = volumePercent;
                break;
            case AudioChannel.Ambient:
                ambientVolume = volumePercent;
                break;
        }

        // Set the audiosource volume
        fxSource.volume = fxVolume * masterVolume;
        musicSource.volume = musicVolume * masterVolume;
        ambientSource.volume = ambientVolume * masterVolume;
    }

    public void PlayMusic(AudioClip audioClip, float delay)
    {
        musicSource.clip = audioClip;
        musicSource.PlayDelayed(delay);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public IEnumerator PlayMusicFade(AudioClip audioClip, float duration)
    {
        float startVolume = 0;
        float targetVolume = musicSource.volume;
        float currentTime = 0;

        musicSource.clip = audioClip;
        musicSource.Play();

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }
    }

    public IEnumerator StopMusicFade(float duration)
    {
        float currentVolume = musicSource.volume;
        float startVolume = musicSource.volume;
        float targetVolume = 0;
        float currentTime = 0;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }
        musicSource.Stop();
        musicSource.volume = currentVolume;

        yield break;
    }

    public void PlayAmbient(AudioClip audioClip, float delay)
    {
        ambientSource.clip = audioClip;
        ambientSource.PlayDelayed(delay);
    }

    public void StopAmbient()
    {
        ambientSource.Stop();
    }

    public void PlaySound2D(AudioClip audioClip)
    {
        fxSource.PlayOneShot(audioClip, fxVolume * masterVolume);
    }

    public void PlaySound3D(AudioClip audioClip, Vector3 soundPosition)
    {
        AudioSource.PlayClipAtPoint(audioClip, soundPosition, fxVolume * masterVolume);
    }
}
