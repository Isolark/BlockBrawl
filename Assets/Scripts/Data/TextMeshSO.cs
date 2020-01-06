using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjs/TextMeshSO", order = 1)]
public class TextMeshSO : ScriptableObject
{
    public float FontSize;
    public Color VertexColor;
    public bool IsBold;
}