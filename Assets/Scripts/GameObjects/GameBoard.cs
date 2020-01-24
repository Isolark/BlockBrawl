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

    public void InputConfirm()
    {
        BlockContainer.OnCursorConfirm(Cursor.BoardLoc);
    }

    public void InputTrigger(bool performed)
    {
        BlockContainer.OnInputTrigger(performed);
    }

    public void InputMove(Vector2 value)
    {
        Cursor.OnMove(value);
    }
}