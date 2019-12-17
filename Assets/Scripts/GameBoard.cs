using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public BlockContainer BlockContainer;
    public SpriteLibrary BlockSL;
    public GameCursor Cursor;
    public Vector2 BoardSize;
    public Vector2 CursorStartPosition;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.LockToBoard(BoardSize, CursorStartPosition);
        BlockContainer.Initialize(BoardSize);
    }
    public void ManUpdate()
    {
        BlockContainer.Move();
    }

    public void OnConfirm()
    {
        BlockContainer.OnCursorConfirm(Cursor.BoardLoc);
    }

    public void OnTrigger(bool performed)
    {
        BlockContainer.OnTrigger(performed);
    }

    public void OnMove(Vector2 value)
    {
        Cursor.OnMove(value);
    }
}