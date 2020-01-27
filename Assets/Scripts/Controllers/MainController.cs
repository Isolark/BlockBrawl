using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//Static Controller used for TimedEvent/Transforms/SaveData... Does NOT handle non-generic game logic
public class MainController : MonoBehaviour
{
    public GameState GS_Current; 
    public DataManager DataManager;
    public TimedEventManager TimedEventManager;
    public TransformManager TransformManager;
    public TimedEventManager BackupTimedEventManager;
    public TransformManager BackupTransformManager;
    public AudioSource MusicPlayer;
    public AudioSource SoundFXPlayer;
    public AudioSource BackupSoundFXPlayer; //For use in Menus
    public string Version; //Specifically for data purposes

    public int PrevSceneIndex;
    public bool IsInitialStart;

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

        BackupTimedEventManager.Pause();
        BackupTransformManager.Pause();

        MusicPlayer.volume = PlayerPrefs.GetFloat("MusicVolume");
        SoundFXPlayer.volume = BackupSoundFXPlayer.volume = PlayerPrefs.GetFloat("SoundVolume");
    }

    public TimedAction AddTimedAction(Action action, float activationTime, bool isContinuous = false)
    {
        if(GS_Current == GameState.Active) { return TimedEventManager.AddTimedAction(action, activationTime, isContinuous); }
        else { return BackupTimedEventManager.AddTimedAction(action, activationTime, isContinuous); }
    }

    public void RemoveTimedAction(TimedAction timedAction)
    {
        if(GS_Current == GameState.Active) { TimedEventManager.RemoveTimedAction(timedAction); }
        else { BackupTimedEventManager.RemoveTimedAction(timedAction); }
    }

    public void PlaySound(string soundName)
    {
        if(GS_Current == GameState.Active) { SoundFXPlayer.PlayOneShot(AudioLibrary.AL.GetAudioClipByName(soundName)); }
        else { BackupSoundFXPlayer.PlayOneShot(AudioLibrary.AL.GetAudioClipByName(soundName)); }
    }

    public void PlayMusic(string musicName)
    {
        MusicPlayer.Stop();
        MusicPlayer.clip = AudioLibrary.AL.GetAudioClipByName(musicName);
        MusicPlayer.Play();
    }

    public void Pause()
    {
        MusicPlayer.Pause();
        SoundFXPlayer.Pause();
        TimedEventManager.Pause();
        TransformManager.Pause();
        BackupTimedEventManager.Unpause();
        BackupTransformManager.Unpause();

        GS_Current = GameState.MenuOpen;
    }

    public void Unpause()
    {
        MusicPlayer.Play();
        SoundFXPlayer.Play();
        TimedEventManager.Unpause();
        TransformManager.Unpause();
        BackupTimedEventManager.Pause();
        BackupTransformManager.Pause();

        GS_Current = GameState.Active;
    }

    public void GoToScene(int sceneIndex)
    {
        MainController.MC.GS_Current = GameState.Loading;
        StartCoroutine(SceneLoadCoroutine(sceneIndex));
    }

    public void LoadPrevScene()
    {
        GoToScene(PrevSceneIndex);
    }

    private IEnumerator SceneLoadCoroutine(int sceneIndex)
    {
        var sceneLoader = SceneManager.LoadSceneAsync(sceneIndex);
        sceneLoader.allowSceneActivation = false;

        while(sceneLoader.progress < 0.9f)
        {
            yield return null;
        }

        foreach(var objPooler in FindObjectsOfType<ObjectPooler>())
        {
            objPooler.RepoolAllObjects();
        }
        TransformManager.Reset();
        TimedEventManager.Reset();
        BackupTransformManager.Reset();
        BackupTimedEventManager.Reset();

        sceneLoader.allowSceneActivation = true;

        while(!sceneLoader.isDone)
        {
            yield return null;
        }

        Unpause();
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

    public T GetData<T>(string soName) where T : ScriptableObject
    {
        if(!DataManager.DataList.ContainsKey(soName)) { return null; }
        return (T)DataManager.DataList[soName];
    }

    protected virtual void FixedUpdate()
    {
        if(FixedUpdateDelegate != null) { FixedUpdateDelegate(); }
    }

    protected virtual void Update()
    {
        if(UpdateDelegate != null) { UpdateDelegate(); }
    }
}