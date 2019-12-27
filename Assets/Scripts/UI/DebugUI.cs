using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    public Button LocateBlockBtn;
    public TMP_InputField LocateBlockX;
    public TMP_InputField LocateBlockY;

    // Start is called before the first frame update
    void Start()
    {
        LocateBlockBtn.onClick.AddListener(LocateBlock);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LocateBlock()
    {
        if(string.IsNullOrWhiteSpace(LocateBlockX.text) || string.IsNullOrWhiteSpace(LocateBlockY.text)) { return; }

        var x = int.Parse(LocateBlockX.text);
        var y = int.Parse(LocateBlockY.text);

        var blockLoc = new Vector2(x, y);
        DebugController.DC.LocateBlock(blockLoc);
    }
}
