using System;
using UnityEngine;

[Serializable] // Sound fields to be added to SoundLibrary ScriptableObject
public class Sound 
{
    public string id;
    public AudioClip audioClip;
    public float volume = 1f;
    public float pitch = 1f;
    public bool loop = false;
}
