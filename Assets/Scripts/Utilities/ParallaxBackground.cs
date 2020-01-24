using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Vector3 Velocity;
    public GameObject PrimaryLayer;
    public GameObject SecondaryLayer;
    public int BaseLayerIndex;

    public Vector3 InitialPos;
    public Vector3 FinalPos;

    void Start()
    {
        float minX = 0, maxX = 0, minY = 0, maxY = 0;

        var spriteList = PrimaryLayer.GetComponentsInChildren<SpriteRenderer>();

        foreach(var sprite in spriteList)
        {
            var spriteWidth = sprite.bounds.size.x / 2;
            var spriteHeight = sprite.bounds.size.y / 2;

            var leftX = sprite.transform.localPosition.x - spriteWidth;
            var rightX = sprite.transform.localPosition.x + spriteWidth;
            var topY = sprite.transform.localPosition.y + spriteHeight;
            var botY = sprite.transform.localPosition.y - spriteHeight;

            if(leftX < minX) { minX = leftX; }
            if(rightX > maxX) { maxX = rightX; }
            if(topY > maxY) { maxY = topY; }
            if(botY < minY) { minY = botY; }

            sprite.sortingOrder += BaseLayerIndex;
        }

        foreach(var secondarySprite in SecondaryLayer.GetComponentsInChildren<SpriteRenderer>())
        {
            secondarySprite.sortingOrder += BaseLayerIndex;
        }

        InitialPos = transform.localPosition;

        var signX = Velocity.x == 0 ? 0 : Mathf.Sign(Velocity.x);
        var signY = Velocity.y == 0 ? 0 : Mathf.Sign(Velocity.y);

        FinalPos = InitialPos + new Vector3(signX * (maxX - minX), signY * (maxY - minY), 0);
        SecondaryLayer.transform.localPosition = PrimaryLayer.transform.localPosition - (FinalPos - InitialPos);
    }

    void Update()
    {
        var nextPosition = transform.localPosition + Velocity;

        if((Mathf.Sign(Velocity.x) > 0 && nextPosition.x > FinalPos.x) || (Mathf.Sign(Velocity.x) < 0 && nextPosition.x < FinalPos.x)) {
            nextPosition = new Vector3(InitialPos.x, nextPosition.y, 0);
        }
        if((Mathf.Sign(Velocity.y) > 0 && nextPosition.y > FinalPos.y) || (Mathf.Sign(Velocity.y) < 0 && nextPosition.y < FinalPos.y)) {
            nextPosition = new Vector3(nextPosition.x, InitialPos.y, 0);
        }

        transform.localPosition = nextPosition;
    }
}