using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Responsible for displaying the damage/garbage that is coming to the container and passing it after appropriate time
public class DamageBar : MonoBehaviour
{
    public BlockContainer BlockContainer;
    public SpriteRenderer BarSprite;
    public List<DamageBlock> DamageBlockList;
    public const int DamagePerRow = 6;

    // Start is called before the first frame update
    void Start()
    {
        //DamageBlockList = new List<int>();
    }

    void AddDamage(int chainCount, int comboCount)
    {
        comboCount--;
        chainCount += (int)Mathf.Floor(comboCount / DamagePerRow);
        comboCount = comboCount % DamagePerRow;

        if(chainCount > 0)
        {

        }
        if(comboCount > 0)
        {
            if(!DamageBlockList.Exists(x => x.Damage == comboCount)) {
                //DamageBlockList.Add(new DamageBlock())
            }
        }
    }

    //Send Damage to the Container, it handles generating blocks and such
    public void PassDamage()
    {
        //BlockContainer.ReceiveDamage(DamageList);
        //DamageBlockList = new List<int>();
    }
}

public class DamageBlock : MonoBehaviour
{
    public List<SpriteRenderer> BlockSprites;
    public TMP_Text DamageText;
    public int Damage;
    
    public DamageBlock(int damage)
    {
        Damage = damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        BlockSprites = new List<SpriteRenderer>();
    }

    void Initialize(int damage)
    {
        Damage = damage;


    }
}