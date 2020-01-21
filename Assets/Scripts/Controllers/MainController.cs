using System;
using UnityEngine;

//Static Controller used for TimedEvent/Transforms/SaveData... Does NOT handle non-generic game logic
public class MainController : MonoBehaviour
{
    public GameState GS_Current; 
    public TimedEventManager TimedEventManager;
    public TransformManager TransformManager;
    public AudioSource MusicPlayer;
    public AudioSource SoundFXPlayer;
    public string Version; //Specifically for data purposes

    public delegate void OnFixedUpdateDelegate();
    public event OnFixedUpdateDelegate FixedUpdateDelegate;
    public delegate void OnUpdateDelegate();
    public event OnUpdateDelegate UpdateDelegate;

    public static MainController MC;

    void Awake()
    {
        //Singleton pattern
        if (MC == null) { MC = this; }
        else if (MC != this) { Destroy(gameObject); }
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        if(!PlayerPrefs.HasKey("Version"))
        {
            CreateInitialPlayerPrefs();
        }

        MusicPlayer.volume = PlayerPrefs.GetFloat("MusicVolume");
        SoundFXPlayer.volume = PlayerPrefs.GetFloat("SoundVolume");
    }

    public TimedAction AddTimedAction(Action action, float activationTime, bool isContinuous = false)
    {
        return TimedEventManager.AddTimedAction(action, activationTime, isContinuous);
    }

    public void RemoveTimedAction(TimedAction timedAction)
    {
        TimedEventManager.RemoveTimedAction(timedAction);
    }

    public void PlaySound(string soundName)
    {
        SoundFXPlayer.PlayOneShot(AudioLibrary.AL.GetAudioClipByName(soundName));
    }

    public void PlayMusic(string musicName)
    {
        MusicPlayer.clip = AudioLibrary.AL.GetAudioClipByName(musicName);
        MusicPlayer.Play();
    }

    protected virtual void FixedUpdate()
    {
        if(FixedUpdateDelegate != null) { FixedUpdateDelegate(); }
    }

    protected virtual void Update()
    {
        if(UpdateDelegate != null) { UpdateDelegate(); }
    }

    private void CreateInitialPlayerPrefs()
    {
        //TODO: Load from SO Instead
        PlayerPrefs.SetString("Version", Version);
        PlayerPrefs.SetFloat("MusicVolume", 0.8f);
        PlayerPrefs.SetFloat("SoundVolume", 1);
        PlayerPrefs.Save();
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetFloat("MusicVolume", MusicPlayer.volume);
        PlayerPrefs.SetFloat("SoundVolume", SoundFXPlayer.volume);
        PlayerPrefs.Save();
    }
}