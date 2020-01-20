using System;
using UnityEngine;

//Parent controller for gameplay. Passes on inputs to relevant game objects
public class BaseController : MonoBehaviour
{
    public GameState GS_Current; 
    public TimedEventManager TimedEventManager;
    public TransformManager TransformManager;
    public AudioSource MusicPlayer;
    public AudioSource SoundFXPlayer;

    public delegate void OnFixedUpdateDelegate();
    public event OnFixedUpdateDelegate FixedUpdateDelegate;
    public delegate void OnUpdateDelegate();
    public event OnUpdateDelegate UpdateDelegate;

    public virtual TimedAction AddTimedAction(Action action, float activationTime, bool isContinuous = false)
    {
        return TimedEventManager.AddTimedAction(action, activationTime, isContinuous);
    }

    public virtual void RemoveTimedAction(TimedAction timedAction)
    {
        TimedEventManager.RemoveTimedAction(timedAction);
    }

    public virtual void PlaySound(string soundName)
    {
        SoundFXPlayer.PlayOneShot(AudioLibrary.AL.GetAudioClipByName(soundName));
    }

    public virtual void PlayMusic(string musicName)
    {
        MusicPlayer.clip = AudioLibrary.AL.GetAudioClipByName(musicName);
        MusicPlayer.Play();
    }

    protected virtual void FixedUpdate()
    {
        FixedUpdateDelegate();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if(!PlayerPrefs.HasKey("Version"))
        {
            CreateInitialPlayerPrefs();
        }

        MusicPlayer.volume = PlayerPrefs.GetFloat("MusicVolume");
        SoundFXPlayer.volume = PlayerPrefs.GetFloat("SoundVolume");
    }

    private void CreateInitialPlayerPrefs()
    {
        PlayerPrefs.SetString("Version", "0.1a");
        PlayerPrefs.SetFloat("MusicVolume", 0.8f);
        PlayerPrefs.SetFloat("SoundVolume", 1);
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetFloat("MusicVolume", MusicPlayer.volume);
        PlayerPrefs.SetFloat("SoundVolume", SoundFXPlayer.volume);
    }

    protected virtual void Update()
    {
        //UpdateDelegate();
    }
}