using UnityEngine;

public class Block : MonoBehaviour
{
    public BlockType Type;
    public SpriteRenderer BlockSprite;
    public static SpriteLibrary BlockSL;
    public Vector3 BoardLoc;
    public bool IsComboable;
    public bool IsChainable;
    public bool IsMoveable;
}
