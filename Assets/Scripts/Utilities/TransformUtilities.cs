using System.Collections.Generic;
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
        RectTransform rect;

        if(reference.TryGetComponent<SpriteRenderer>(out sprite))
        {
            target.TransBySpriteDimensions(sprite, transPercents);
        }
        else if(reference.TryGetComponent<RectTransform>(out rect))
        {
            target.TransByRectDimensions(rect, transPercents);
        }
    }

    //Actual Transform Logic
    private static void TransBySpriteDimensions(this GameObject target, SpriteRenderer spriteRef, Vector3 spritePercents)
    {
        var size = new Vector3(spriteRef.bounds.size.x, spriteRef.bounds.size.y, 0);
        target.transform.localPosition += Vector3.Scale(size, spritePercents);
    }

    private static void TransByRectDimensions(this GameObject target, RectTransform rectRef, Vector3 rectPercents)
    {
        var size = new Vector3(rectRef.rect.width, rectRef.rect.height, 0);
        target.transform.localPosition += Vector3.Scale(size, rectPercents);
    }

    public static void ResetTransform(this Transform transform, bool resetParent = false)
    {
        if(resetParent) { transform.SetParent(null); }

        transform.localPosition = transform.position = Vector3.zero;
        transform.rotation = transform.localRotation = new Quaternion(0, 0, 0, 0);
        transform.localScale = Vector3.one;
    }
    public static void ResetAllChildrenRecursively(this Transform parent)
    {
        while(parent.childCount > 0)
        {
            parent.GetChild(0).ResetAllChildrenRecursively();
        }

        parent.SetParent(null);
        parent.ResetTransform();
        parent.gameObject.SetActive(false);
    }

    public static void GetAllChildrenRecursively(this Transform parent, ref List<Transform> objList)
    {
        for(var i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).GetAllChildrenRecursively(ref objList);
        }
        objList.Add(parent);
    }
}