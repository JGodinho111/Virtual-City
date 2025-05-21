using System.Collections.Generic;
using UnityEngine;

// Scriptable Object to hold all game sounds
[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Scriptable Objects/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public List<Sound> sounds;

    public Sound GetSoundByName(string idName)
    {
        foreach(var sound in sounds)
        {
            if(sound.id == idName)
            {
                return sound;
            }
        }
        return null;
    }
}
