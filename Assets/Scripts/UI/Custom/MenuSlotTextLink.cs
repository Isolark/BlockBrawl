using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuSlotTextLink : MenuSlot
{
    public TMP_Text LinkedText;
    public string TextValue;

    override public void SetItem()
    {
        LinkedText.SetText(TextValue);
    }
}