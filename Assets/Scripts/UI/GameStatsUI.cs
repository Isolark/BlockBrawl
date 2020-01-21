using System;
using TMPro;
using UnityEngine;

public class GameStatsUI : MonoBehaviour
{
    public TMP_Text GameTimeLabel;
    public TMP_Text GameTimeValue;
    public TMP_Text CurrentChainLabel;
    public TMP_Text CurrentChainValue;
    public TMP_Text MaxChainLabel;
    public TMP_Text MaxChainValue;

    // Start is called before the first frame update
    void Start()
    {
        CurrentChainValue.text = MaxChainValue.text = "x1";
    }

    public void SetCurrentChain(int chainValue)
    {
        CurrentChainValue.text = "x" + chainValue.ToString();
    }

    public void SetMaxChain(int chainValue)
    {
        MaxChainValue.text = "x" + chainValue.ToString();
    }

    public void SetGameTime(float gameTime)
    {
        var time = TimeSpan.FromSeconds(gameTime);
        GameTimeValue.text = time.ToString(@"m\:ss");
    }
}