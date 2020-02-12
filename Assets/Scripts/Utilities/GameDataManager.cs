using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour{

    public PlayerConfigData PlyrConfigData;
    public PlayerGameData PlyrGameData;

    private readonly string PLAYERGAME_DATAPATH = "/TetraNights.sav";
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
        
        if(!DataUtility.CheckFileExists(PLAYERSETTING_DATAPATH))
        {
            CreateInitialPlyrCfgData();
            SavePlyrCfgData();
        }
        else { 
            PlyrConfigData = DataUtility.LoadFromJSON<PlayerConfigData>(PLAYERSETTING_DATAPATH); 
        }

        if(!DataUtility.CheckFileExists(PLAYERGAME_DATAPATH))
        {
            CreateInitialPlyrGameData();
            SavePlyrGameData();
        }
        else { 
            PlyrGameData = DataUtility.LoadFromBinary<PlayerGameData>(PLAYERGAME_DATAPATH);
        }

        IsPlayerDataLoaded = true;
    }

    public void SavePlyrCfgData()
    {
        DataUtility.SaveToJSON(PLAYERSETTING_DATAPATH, PlyrConfigData);
    }

    public void SavePlyrGameData()
    {
        DataUtility.SaveToBinary(PLAYERGAME_DATAPATH, PlyrGameData);
    }

    private void CreateInitialPlyrCfgData()
    {
        PlyrConfigData = new PlayerConfigData()
        {
            MusicVolume = 0.60f,
            SoundVolume = 0.80f
        };
    }

    private void CreateInitialPlyrGameData()
    {
        PlyrGameData = new PlayerGameData()
        {
            PlayerID = Guid.NewGuid(),
            PlayerName = "Dreamer",
            PlayerEmail = "",
            BlockBrawlMaxLv = 1,
            HighScores = new List<HighScoreRecord>(),
            CreateDate = DateTime.UtcNow,
            LastUpdateDate = DateTime.UtcNow
        };
    }
}