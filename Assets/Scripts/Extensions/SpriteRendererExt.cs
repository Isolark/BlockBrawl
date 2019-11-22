using UnityEngine;

public static class SpriteRendererExt
{
    public static void TransByDimensions(this SpriteRenderer target, Vector3 transPercents)
    {
        var size = new Vector3(target.bounds.size.x, target.bounds.size.y, 0);
        target.transform.localPosition += Vector3.Scale(size, transPercents);
    }
}