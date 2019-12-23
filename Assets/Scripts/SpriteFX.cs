using UnityEngine;

//Sprite that plays through an animation and then deactivates itself (by default)
public class SpriteFX : MonoBehaviour
{
    public bool DestroySelf;
    public SpriteRenderer FXSprite;
    public Animator FXAnimCtrl;

    public void OnFinishAnimation(string clipName)
    {
        if(DestroySelf && clipName == "None") {
            this.OnDestroy();
        }
    }
}