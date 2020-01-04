using UnityEngine;

public static class TransformUtilities
{
    public static void TransBySpriteDimensions(this GameObject target, Vector3 transPercents)
    {
        target.TransBySpriteDimensions(target, transPercents);
    }
    public static void TransBySpriteDimensions(this GameObject target, GameObject reference, Vector3 transPercents)
    {
        SpriteRenderer sprite;
        if(reference.TryGetComponent<SpriteRenderer>(out sprite))
        {
            target.TransBySpriteDimensions(sprite, transPercents);
        }
    }

    //Actual Transform Logic
    private static void TransBySpriteDimensions(this GameObject target, SpriteRenderer spriteRef, Vector3 spritePercents)
    {
        var size = new Vector3(spriteRef.bounds.size.x, spriteRef.bounds.size.y, 0);
        target.transform.localPosition += Vector3.Scale(size, spritePercents);
    }

    public static void ResetAllChildrenRecursively(this Transform parent)
    {
        while(parent.childCount > 0)
        {
            parent.GetChild(0).ResetAllChildrenRecursively();
        }

        parent.SetParent(null);
        parent.gameObject.SetActive(false);
    }
}