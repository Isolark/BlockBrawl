using UnityEngine;
using System.Collections.Generic;

public class SpriteLibrary : MonoBehaviour
{
    public List<Sprite> SpriteList;

    public static SpriteLibrary SL;

    void Awake()
    {
        //Singleton pattern
        if (SL == null) {
            SL = this;
        }
        else if (SL != this) {
            Destroy(gameObject);
        }     
    }

    public Sprite GetSpriteByName(string name)
    {
        return SpriteList.Find(x => x.name == name);
    }
}