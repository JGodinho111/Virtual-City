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

    private Dictionary<string, Sound> soundMap;

    private List<AudioSource> audioSourcePool = new();
    private int sourcePoolSize = 5; // Could have one per soundMap id, but not needed right now

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
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSourcePool.Add(audioSource);
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

    // 9 Currently Existing Sounds (edit in Sound Library ScriptableObject):
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
            AudioSource currentAudioSource = GetFreeAudioSource();

            if (currentAudioSource == null)
            {
                Debug.LogWarning("No audioSource available at the moment");
                yield return null;
            }

            currentAudioSource.clip = currentCliptoPlay.audioClip;
            currentAudioSource.volume = currentCliptoPlay.volume;

            // To start playing at half the clip because I usually have dead space before the audio
            currentAudioSource.time = currentCliptoPlay.audioClip.length / 2f;

            currentAudioSource.PlayOneShot(currentCliptoPlay.audioClip);

            currentAudioSource.clip = null;            
            yield return null;
        }
        else
        {
            Debug.LogWarning(currentCliptoPlay.id + " is missing its audio clip!");
            yield return null;
        }
        
    }

    private AudioSource GetFreeAudioSource()
    {
        foreach (var audioSource in audioSourcePool)
        {
            if (audioSource.clip == null)
                return audioSource;
        }

        return null;
    }
}
