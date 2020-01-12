using UnityEngine;
using System.Collections.Generic;

public class AudioLibrary : MonoBehaviour
{
    public List<AudioClip> AudioClipList;

    public static AudioLibrary AL;

    void Awake()
    {
        //Singleton pattern
        if (AL == null) {
            AL = this;
        }
        else if (AL != this) {
            Destroy(gameObject);
        }     
    }

    public AudioClip GetAudioClipByName(string name)
    {
        return AudioClipList.Find(x => x.name == name);
    }
}