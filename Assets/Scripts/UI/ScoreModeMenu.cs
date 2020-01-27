using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreModeMenu : MonoBehaviour
{
    public bool IsTimeDoubleDigits;
    public TMP_Text GameTimeValue;

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
}
