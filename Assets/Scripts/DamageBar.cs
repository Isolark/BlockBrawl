using UnityEngine;

//Responsible for displaying the damage/garbage that is coming to the container and passing it after appropriate time
public class DamageBar : MonoBehaviour
{
    public BlockContainer BlockContainer;
    public SpriteRenderer BarSprite;
    public int Score;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnUpdate()
    {
    }

    //Send Damage to the Container
    public void PassDamage()
    {
    }
}