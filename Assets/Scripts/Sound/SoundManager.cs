using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles playing all sounds for the game
/// Called by other classes to play sounds
/// Is a singleton, CheckPlaySound of instance called by classes that use sounds
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField]
    private SoundLibrary soundLibrary;

    [SerializeField]
    private AudioSource audioSourcePrefab;

    private Dictionary<string, Sound> soundMap;

    private Queue<AudioSource> audioSourcePool = new(); // Queue instead of a list
    private int sourcePoolSize = 10; // Could have one per soundMap id, but not needed right now

    private AudioSource backgroundMusicPlayer;

    // Singleton Setup
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        // This is not necessary right now since there is just one game scene,
        // but I'm keeping it here in case I add more.
        DontDestroyOnLoad(this.gameObject);

        // Building dictionary through SoundLibrary scriptable object
        soundMap = new Dictionary<string, Sound>();
        foreach (var sound in soundLibrary.sounds)
        {
            if(!soundMap.ContainsKey(sound.id))
            {
                soundMap.Add(sound.id, sound);
            }
            else
            {
                Debug.LogWarning("Duplicate sound ID.");
            }
        }

        // Setting up the source pool
        for(int i = 0; i< sourcePoolSize; i++)
        {
            AudioSource audioSource = Instantiate(audioSourcePrefab, transform);
            audioSource.gameObject.SetActive(false);
            audioSourcePool.Enqueue(audioSource);
        }
    }

    private void Start()
    {
        PlayMusic("BackgroundMusic");
    }

    // Playing background Music
    private void PlayMusic(string v)
    {
        if (soundMap.TryGetValue(v, out Sound sound))
        {
            // If audio pool is not empty
            if (audioSourcePool.Count != 0)
            {
                backgroundMusicPlayer = audioSourcePool.Dequeue();

                // If audio source is null return
                if (backgroundMusicPlayer == null)
                {
                    Debug.LogWarning("AudioSource is null");
                    return;
                }

                backgroundMusicPlayer.gameObject.SetActive(true);

                backgroundMusicPlayer.clip = sound.audioClip;

                backgroundMusicPlayer.volume = sound.volume;

                backgroundMusicPlayer.loop = sound.loop;

                backgroundMusicPlayer.pitch = sound.pitch;

                Debug.Log("BackgroundMusic Playing");
                backgroundMusicPlayer.Play();
            }
            else
            {
                Debug.LogWarning("No audioSource available at the moment");
                return;
            }
        }
        else
        {
            Debug.LogWarning("Missing Background Music " + v);
        }
    }

    // Just here if needed
    private void StopMusic()
    {
        if(backgroundMusicPlayer != null)
        {
            backgroundMusicPlayer.Stop();
            backgroundMusicPlayer.gameObject.SetActive(false);
            audioSourcePool.Enqueue(backgroundMusicPlayer);
            Debug.LogWarning("Background Music Stopped");
        }   
    }

    // Called by multiple classes to play a sound
    public void CheckPlaySound(string soundToPlay)
    {
        if (soundMap.TryGetValue(soundToPlay, out Sound sound))
        {
            StartCoroutine(PlaySoundOneShot(sound));
        }
        else
        {
            Debug.LogWarning("Missing Sound " + soundToPlay);
        }
    }

    // 9 Currently Existing Sounds (SFX) (edit in Sound Library ScriptableObject):
    // | Sound ID | Called From Class |
    // - SpawnSuccess (DragNDropPlacer)
    // - SpawnFailure (DragNDropPlacer)
    // - PanelEnter (UIPanelPositionChange) - NOT USED
    // - PanelExit (UIPanelPositionChange) - NOT USED
    // - ButtonPress (UIButtonPlacementAction)
    // - ButtonEnter (UIButtonHoverZoom)
    // - ButtonExit (UIButtonHoverZoom) - NOT USED
    // - Trash (ElementalEffectsPlacer)
    // - SnowThunder (ElementalEffectsPlacer)
    private IEnumerator PlaySoundOneShot(Sound currentCliptoPlay)
    {
        if(currentCliptoPlay.audioClip != null)
        {
            // If audio pool is not empty
            if (audioSourcePool.Count != 0)
            {
                AudioSource currentAudioSource = audioSourcePool.Dequeue();

                // If audio source is null return
                if(currentAudioSource == null)
                {
                    Debug.LogWarning("AudioSource is null");
                    yield return null;
                }

                currentAudioSource.gameObject.SetActive(true);

                currentAudioSource.clip = currentCliptoPlay.audioClip;

                currentAudioSource.volume = currentCliptoPlay.volume;

                // Set these because the sound effects I recorded aren't the best ones
                float clipStartTime = currentCliptoPlay.audioClip.length * 0.5f;
                float clipEndTime = currentCliptoPlay.audioClip.length * 0.9f;
                float clipDuration = clipEndTime - clipStartTime;

                // To start playing at half the clip because I usually have dead space before the audio
                currentAudioSource.time = clipStartTime;

                Debug.Log("Audio Playing");
                currentAudioSource.Play();
                yield return new WaitForSeconds(clipDuration);
                currentAudioSource.Stop();
                currentAudioSource.gameObject.SetActive(false);
                audioSourcePool.Enqueue(currentAudioSource);
                yield return null;
            }
            else
            {
                Debug.LogWarning("No audioSource available at the moment");
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning(currentCliptoPlay.id + " is missing its audio clip!");
            yield return null;
        }
        
    }

    /*private AudioSource GetFreeAudioSource()
    {
        foreach (var audioSource in audioSourcePool)
        {
            if (audioSource.clip == null)
                return audioSource;
        }

        return null;
    }*/
}
