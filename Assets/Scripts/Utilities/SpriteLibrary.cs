using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteLibrary : MonoBehaviour
{
    public List<Sprite> SpriteList;

    public Sprite GetSpriteByName(string name)
    {
        return SpriteList.Find(x => x.name == name);
    }
}