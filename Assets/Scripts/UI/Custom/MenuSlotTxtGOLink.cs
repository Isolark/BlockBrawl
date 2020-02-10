using System.Collections.Generic;
using UnityEngine;

public class MenuSlotTxtGOLink : MenuSlotTextLink
{
    public List<GameObject> LinkedGOList;

    override public void SetItem()
    {
        foreach(var linkedGO in LinkedGOList)
        {
            linkedGO.SetActive(true);
        }
        base.SetItem();
    }

    override public void UnsetItem()
    {
        foreach(var linkedGO in LinkedGOList)
        {
            linkedGO.SetActive(false);
        } 
    }
}