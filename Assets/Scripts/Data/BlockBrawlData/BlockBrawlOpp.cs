using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjs/BlockBrawlOpp", order = 1)]
public class BlockBrawlOpp : ScriptableObject
{
    public string OpponentName; //Redundant Path to the name used in loading
    public string DisplayName; //Name that is shown in UI
    public int HP; //HP of opponent
    public List<List<int>> AttackList; //Attack List (Format detailed below)
}

/******************************************Attack List Format****************************************************
    -Attack List Format: [[AtkPatt1], [AtkPatt2], [AtkPatt3]]

    -Attack Pattern Format:

        -Int First | Value: 1-100 | Purpose: Indicates the upper range of the "probability roll" for this attack
            -When selecting an attack, random # between 1-100 is rolled. Attack selected is the attack that is
             closet above this value
            -E.g. [10], [80], [100] | 3 attack w/ 10%, 70%, & 20% chances to be selected

        Int X1 | Value: # in milliseconds | Purpose: Attack Delay; The time before the attack is triggered

        Int X2 | Value: 0 or 1 | Purpose: Chain Flag; 0 = Not a chain, 1 = Is a chain

        Int X3 | Value: 3 - 66 | Purpose: Combo Size; Size of the combo/# of blocks

        Int X4 | Value: # in milliseconds | Purpose: Ending Delay; Time before next part of attack is run.
            -When at the final X4, is the delay before the next attack is selected

        Provide First & as many [X1 - X4]s as needed for parts of attack
*****************************************************************************************************************/