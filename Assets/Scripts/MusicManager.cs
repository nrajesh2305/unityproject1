using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioMixerGroup audioMixerGroup;
    public Queue<Sound> musicQueue = new Queue<Sound>();
    public Sound currentSong;

    public List<Sound> fullSongList;
    public event Action<Sound> UpdatedCurrentSong;

    private Coroutine playSongCoroutine;

    private void Awake()
    {
        InitializeSingleton();
        playSongCoroutine = StartCoroutine(PlayMusicQueue());
    }

    public void ResetQueue()
    {
        StopCurrentCoroutine();
        TryStopAndClearQueue();
        playSongCoroutine = StartCoroutine(PlayMusicQueue());
    }

    public bool TryEnqueue(Sound songToAdd)
    {
        if (IsValidSong(songToAdd) && !IsSongInQueue(songToAdd.name))
        {
            SetupSong(songToAdd);
            musicQueue.Enqueue(songToAdd);
            return true;
        }
        return false;
    }

    public Sound TryGetCurrentSong()
    {
        return musicQueue.Count > 0 ? musicQueue.Peek() : null;
    }

    public void TryStopAndClearQueue()
    {
        if (currentSong != null && currentSong.source != null)
        {
            currentSong.source.Stop();
        }
        musicQueue.Clear();
    }

    public void ForcePlay(string songName)
    {
        Sound songToPlay = fullSongList.FirstOrDefault(song => song.name == songName);

        if (songToPlay == null)
        {
            Debug.LogWarning($"Song with name {songName} not found.");
            return;
        }

        ResetQueue();

        if (TryEnqueue(songToPlay))
        {
            currentSong = songToPlay;
            currentSong.source.Play();
            UpdatedCurrentSong?.Invoke(currentSong);
        }
        else
        {
            Debug.LogWarning("Invalid song provided to ForcePlay.");
        }
    }

    public bool TryDequeue(Sound songToRemove)
    {
        if (musicQueue.Count == 0 || songToRemove.name != musicQueue.Peek().name) return false;

        musicQueue.Dequeue();
        return true;
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator PlayMusicQueue()
    {
        const float waitAmount = 0.25f;

        while (true)
        {
            if (musicQueue.Count > 0)
            {
                currentSong = musicQueue.Peek();
                if (!currentSong.source.isPlaying)
                {
                    currentSong.source.Play();
                    UpdatedCurrentSong?.Invoke(currentSong);

                    yield return new WaitForSecondsRealtime(currentSong.source.clip.length + waitAmount);
                    if (!musicQueue.Peek().source.isPlaying)
                        TryDequeue(currentSong);
                    else Debug.LogError("Trying to normally Dequeue a song that is playing...");
                }
            }
            yield return new WaitForSecondsRealtime(waitAmount);
        }
    }

    private bool IsValidSong(Sound song)
    {
        return song != null && song.name != null;
    }

    private bool IsSongInQueue(string soundName)
    {
        return musicQueue.Any(song => song.name == soundName);
    }

    private void SetupSong(Sound songToAdd)
    {
        songToAdd.source = gameObject.AddComponent<AudioSource>();
        songToAdd.source.clip = songToAdd.clip;
        songToAdd.source.loop = songToAdd.loop;
        songToAdd.source.outputAudioMixerGroup = audioMixerGroup;
        SetVolumeAndPitchWithVariance(songToAdd);
    }

    private void SetVolumeAndPitchWithVariance(Sound song)
    {
        float volumeVariance = UnityEngine.Random.Range(-song.volumeDeviation / 2f, song.volumeDeviation / 2f) + 1f;
        float pitchVariance = UnityEngine.Random.Range(-song.pitchDeviation / 2f, song.pitchDeviation / 2f) + 1f;

        song.source.volume = Mathf.Clamp(song.volume * volumeVariance, 0, 1);
        song.source.pitch = song.pitch * pitchVariance;
    }

    private Sound GetSongFromQueue(string soundName)
    {
        Sound sound = musicQueue.FirstOrDefault(song => song.name == soundName);
        if (sound == null)
        {
            Debug.LogWarning($"Couldn't find song: {soundName}, in MusicSystem");
        }
        return sound;
    }

    private void FadeOut(Sound song, float duration)
    {
        StartCoroutine(FadeOutSound(song, duration));
    }

    private IEnumerator FadeOutSound(Sound sound, float duration)
    {
        if (sound?.source == null) yield break;

        float startVolume = sound.source.volume;
        while (sound.source.volume > 0)
        {
            sound.source.volume = Mathf.Max(sound.source.volume - (startVolume * Time.deltaTime / duration), 0);
            yield return null;
        }
        sound.source.Stop();
        sound.source.volume = startVolume;
    }

    private void StopCurrentCoroutine()
    {
        if (playSongCoroutine != null)
        {
            StopCoroutine(playSongCoroutine);
        }
    }
}
