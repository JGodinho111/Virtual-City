using System.Collections.Generic;
using UnityEngine;

// Scriptable Object to hold all game sounds
[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Scriptable Objects/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public List<Sound> sounds;
}
