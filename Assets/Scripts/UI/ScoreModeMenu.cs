using System;
using TMPro;
using UnityEngine;

public class ScoreModeMenu : MonoBehaviour
{
    public bool IsTimeDoubleDigits;
    public TMP_Text GameTimeValue;
    public TMP_Text GameScoreValue;
    public TMP_Text GameSpeedLvValue;

    // Start is called before the first frame update
    void Start()
    {
        IsTimeDoubleDigits = false;
    }
    public void SetGameTime(float gameTime)
    {
        var time = TimeSpan.FromSeconds(gameTime);
        var timeText = time.ToString(@"m\:ss");
        

        if(!IsTimeDoubleDigits && timeText.Length > 4) { 
            GameTimeValue.rectTransform.localPosition += new Vector3(-24, 0, 0);
            IsTimeDoubleDigits = true; 
        }

        GameTimeValue.text = timeText;
    }

    public void IncreaseScore(int score)
    {
        GameScoreValue.text = (int.Parse(GameScoreValue.text) + score).ToString();;
    }

    public void SetSpeedLv(int speedLv)
    {
        GameSpeedLvValue.text = speedLv.ToString();
    }
}
