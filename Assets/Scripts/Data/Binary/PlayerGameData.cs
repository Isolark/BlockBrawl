using System;
using System.Collections.Generic;

[Serializable]
public class PlayerGameData
{
    public Guid PlayerID;
    public string PlayerName;
    public string PlayerEmail;
    
    public int BlockBrawlMaxLv;
    public List<HighScoreRecord> HighScores;

    public DateTime CreateDate;
    public DateTime LastUpdateDate;
}