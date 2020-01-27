using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjs/BlockBattleLevel", order = 1)]
public class BlockBattleLv : ScriptableObject
{
    public int BBLevel; //Unique Identifier for the level
    public string OpponentName; //Used to load from "BBOpponent" SO
    public int DifficultyLv; //Used to load from "DifficultyLv" SO
    public string BackgroundSpritePath; //Used to load from Assets
}