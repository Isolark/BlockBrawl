using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjs/BlockBrawlLevel", order = 1)]
public class BlockBrawlLv : ScriptableObject
{
    public int BBLevel; //Unique Identifier for the level
    public string OpponentName; //Used to load from "BBOpponent" SO
    [TextAreaAttribute(10,20)]
    public string OpponentDescription;
    public int DifficultyLv; //Used to load from "DifficultyLv" SO
    public string BackgroundSpritePath; //Used to load from Assets
}