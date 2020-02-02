using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjs/MainGameData", order = 1)]
public class MainGameData : ScriptableObject
{
    public string Version; //Version of the game, when different from what was saved previously, catch up/build update list
    public DateTime LastUpdatedDateTime; //When was the version last updated?

    public bool ShowBanner; //Whether or not to show the banner string
    public string BannerString; //String to display to user when landing on Title Screen
}