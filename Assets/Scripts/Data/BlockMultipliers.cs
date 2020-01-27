using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjs/BlockMultipliers", order = 1)]
public class BlockMultipliers : ScriptableObject
{
    public List<float> ComboMults;
    public float ComboMaxAdd;
    public List<float> ChainMults;
    public float ChainMaxAdd;
}