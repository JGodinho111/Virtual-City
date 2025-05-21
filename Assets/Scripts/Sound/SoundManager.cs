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

    // 9 Sounds (edit in Sound Library ScriptableObject):
    // - SpawnSuccess (DragNDropPlacer)
    // - SpawnFailure (DragNDropPlacer)
    // - PanelEnter (UIPanelPositionChange)
    // - PanelExit (UIPanelPositionChange)
    // - ButtonPress (UIButtonPlacementAction)
    // - ButtonEnter (UIButtonHoverZoom)
    // - ButtonExit (UIButtonHoverZoom)
    // - Trash (ElementalEffectsPlacer)
    // - SnowThunder (ElementalEffectsPlacer)
    private IEnumerator PlaySoundOneShot(Sound currentCliptoPlay)
    {
        if(currentCliptoPlay.audioClip != null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.clip = currentCliptoPlay.audioClip;
            audioSource.volume = currentCliptoPlay.volume;
            audioSource.PlayOneShot(currentCliptoPlay.audioClip);
            if (!audioSource.isPlaying)
            {
                Destroy(audioSource);
            }
            yield return null;
        }
        else
        {
            Debug.LogWarning(currentCliptoPlay.id + " is missing its audio clip!");
        }
        
    }
}
