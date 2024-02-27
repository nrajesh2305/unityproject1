using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioService : MonoBehaviour
{
    public static AudioService Instance;
    public List<Sound> soundsList;
    private AudioMixerGroup _audioMixerGroup;
    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();

    public AudioMixerGroup AudioMixerGroup { get; set; }
    private float currentPitch = 1.0f;

    public void Awake()
    {
        InitializeSingleton();
        InitializeSounds();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSounds()
    {
        foreach (Sound sound in soundsList)
        {
            sound.Initialize(gameObject, _audioMixerGroup);
            if (!soundDictionary.ContainsKey(sound.name))
                soundDictionary.Add(sound.name, sound);
        }
    }

    public void SetPitchForAllSounds(float pitch)
    {
        const float minPitch = 0.2f;
        currentPitch = Mathf.Max(pitch, minPitch);

        foreach (var sound in soundDictionary.Values)
        {
            if (sound.source.isPlaying)
            {
                sound.source.pitch = currentPitch;
            }
        }
    }

    public void Play(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            sound.pitch = currentPitch;
            sound.source.pitch = currentPitch;
            sound.Play();
        }
        else
        {
            Debug.LogError($"Sound {soundName} not found");
        }
    }

    public Sound TryGetSound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            return sound;
        }
        else
        {
            Debug.LogError($"Sound {soundName} not found");
            return null;
        }
    }

    public void FadeOut(string soundName, float duration)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            sound.FadeOut(duration);
        }
        else
        {
            Debug.LogError($"Sound {soundName} not found");
        }
    }
}
