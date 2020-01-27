using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjs/DifficultyLv", order = 1)]
public class DifficultyLv : ScriptableObject
{
    public int Level;
    public int NumberOfBlocks;
    public int StartingSpeedLv;
    public float DestroyTimeMultiplier;
    public float BaseScoreValue;
    public float BaseTimeStopValue;
}