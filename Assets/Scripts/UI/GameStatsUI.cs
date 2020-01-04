using TMPro;
using UnityEngine;

public class GameStatsUI : MonoBehaviour
{
    public TMP_Text CurrentChainLabel;
    public TMP_Text CurrentChainValue;
    public TMP_Text MaxChainLabel;
    public TMP_Text MaxChainValue;

    // Start is called before the first frame update
    void Start()
    {
        CurrentChainValue.text = MaxChainValue.text = "x0";
    }

    void SetCurrentChain(int chainValue)
    {
        CurrentChainValue.text = "x" + chainValue.ToString();
    }

    void SetMaxChain(int chainValue)
    {
        MaxChainValue.text = "x" + chainValue.ToString();
    }
}