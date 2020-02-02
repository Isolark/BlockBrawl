using UnityEngine;

public class GameDataManager : MonoBehaviour{

    public PlayerConfigData PlyrConfigData;

    //private readonly string MAIN_DATAPATH = "/TetraNights.data.json";
    private readonly string PLAYERSETTING_DATAPATH = "/TetraNights.playerConfig.json";

    public static GameDataManager GM;
    public static bool IsPlayerDataLoaded;

    void Awake() {
        //Singleton pattern
        if (GM == null) {
            GM = this;
        }
        else if (GM != this) {
            GM = this;
        }
    }

    public void Initialize()
    {
        if(IsPlayerDataLoaded) { return; }
        
        if(!DataUtility.CheckExistsJSON(PLAYERSETTING_DATAPATH))
        {
            CreateInitialPlyrCfgData();
            SavePlyrCfgData();
        }
        else
        {
            PlyrConfigData = DataUtility.LoadFromJSON<PlayerConfigData>(PLAYERSETTING_DATAPATH);
        }
        IsPlayerDataLoaded = true;
    }

    public void SavePlyrCfgData()
    {
        DataUtility.SaveToJSON(PLAYERSETTING_DATAPATH, PlyrConfigData);
    }

    private void CreateInitialPlyrCfgData()
    {
        PlyrConfigData = new PlayerConfigData()
        {
            MusicVolume = 0.60f,
            SoundVolume = 0.80f
        };
    }
}