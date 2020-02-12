using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

//Static Controller used for TimedEvent/Transforms/SaveData... Does NOT handle non-generic game logic
public class MainController : MonoBehaviour
{
    public GameState GS_Current; 
    public SceneDataManager ScnDataManager;
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
        ProfanityFilter.Initialize();
        GameDataManager.GM.Initialize();

        BackupTimedEventManager.Pause();
        BackupTransformManager.Pause();

        MusicPlayer.volume = GameDataManager.GM.PlyrConfigData.MusicVolume;
        SoundFXPlayer.volume = GameDataManager.GM.PlyrConfigData.SoundVolume;
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

    public void StopAudio()
    {
        MusicPlayer.clip = null;
        SoundFXPlayer.clip = null;
    }

    public void StopMusic()
    {
        MusicPlayer.clip = null;
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
        if(MusicPlayer.clip != null) { MusicPlayer.Play(); }
        if(SoundFXPlayer.clip != null) { SoundFXPlayer.Play(); }
        TimedEventManager.Unpause();
        TransformManager.Unpause();
        BackupTimedEventManager.Pause();
        BackupTransformManager.Pause();

        GS_Current = GameState.Active;
    }

    public void GoToScene(int sceneIndex, bool stopMusic = true)
    {
        if(stopMusic) { StopMusic(); }
        
        MainController.MC.GS_Current = GameState.Loading;
        var sceneLoader = SceneManager.LoadSceneAsync(sceneIndex);
        StartCoroutine(SceneLoadCoroutine(sceneLoader));
    }

    public void GoToScene(string sceneName, bool stopMusic = true)
    {
        if(stopMusic) { StopMusic(); }
        
        MainController.MC.GS_Current = GameState.Loading;
        var sceneLoader = SceneManager.LoadSceneAsync(sceneName);
        StartCoroutine(SceneLoadCoroutine(sceneLoader));
    }

    public void LoadPrevScene(bool stopMusic = true)
    {
        GoToScene(PrevSceneIndex, stopMusic);
    }

    private IEnumerator SceneLoadCoroutine(AsyncOperation sceneLoader)
    {
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

    // private void CreateInitialPlayerPrefs()
    // {
    //     //TODO: Load from SO Instead
    //     PlayerPrefs.SetString("Version", Version);
    //     PlayerPrefs.SetFloat("MusicVolume", 0.8f);
    //     PlayerPrefs.SetFloat("SoundVolume", 1);
    //     PlayerPrefs.Save();
    // }

    // public void SaveOptions()
    // {
    //     PlayerPrefs.SetFloat("MusicVolume", MusicPlayer.volume);
    //     PlayerPrefs.SetFloat("SoundVolume", SoundFXPlayer.volume);
    //     PlayerPrefs.Save();
    // }

    public void SaveOptions()
    {
        var plyrAVSettings = GameDataManager.GM.PlyrConfigData;
        plyrAVSettings.MusicVolume = MusicPlayer.volume;
        plyrAVSettings.SoundVolume = SoundFXPlayer.volume;

        GameDataManager.GM.SavePlyrCfgData();
    }

    public T GetData<T>(string soName) where T : ScriptableObject
    {
        if(!ScnDataManager.DataList.ContainsKey(soName)) { return null; }
        return (T)ScnDataManager.DataList[soName];
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