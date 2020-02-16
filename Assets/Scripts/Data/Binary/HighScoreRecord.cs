using System;

[Serializable]
public class HighScoreRecord
{
    public DateTime CreatedDate;
    public int Score;
    public int MaxChain;
    public int MaxSpeedLv;
    public DateTime GameTime;
    public int DifficultyLv;
}