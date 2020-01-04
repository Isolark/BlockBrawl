using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public BlockContainer BlockContainer;
    public GameCursor Cursor;
    public Vector2 BoardSize;
    public Vector2 CursorStartPosition;

    public int Score;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.LockToBoard(BoardSize, CursorStartPosition);
        BlockContainer.Initialize(BoardSize);
    }
    public void OnUpdate()
    {
        BlockContainer.OnUpdate();
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