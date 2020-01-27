using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjs/BlockBattleLevel", order = 1)]
public class BlockBattleOpp : ScriptableObject
{
    public string OpponentName; //Redundant Path to the name used in loading
    public string DisplayName;
    public int HP;
    public List<List<int>> AttackList; //Attack List (Format detailed below)
}