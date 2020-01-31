using UnityEngine;
using UnityEngine.InputSystem;

public class GameCursor : DirectionInputReceiver
{
    public Vector2 ZeroPosition;
    public Vector2 Bounds;
    public Vector2 BoardLoc;
    public float Padding;
    public float BlockDist;
    public bool AtTop; //While true, allows moving to top row

    // Set zero position (assumed set by gameCtrl) & bounds
    public void LockToBoard(Vector2 boardSize, Vector2 startingPosition)
    {
        BlockDist = GameController.GameCtrl.BlockDist;
        transform.localPosition = ZeroPosition = Vector2.zero;
        transform.localPosition += new Vector3(-BlockDist * 2, 0.5f);
        Bounds = boardSize - new Vector2(1, 0);
        BoardLoc = Vector2.up;

        OnMove(startingPosition, false);
    }

    public void OnMove(InputValue value)
    {
        var v = value.Get<Vector2>();
        OnMove(v);
    }

    public void MoveBoardLoc(Vector2 value)
    {
        BoardLoc = BoardLoc + value;
    }

    public void OnMove(Vector2 value, bool inputBased = true)
    {
        if(IsHoldMoving) { IsHoldMoving = false; }

        if(inputBased)
        {
            OnMove(value, MoveCursor);
        }
        else { MoveCursor(value); } 
    }

    private void MoveCursor(Vector2 value)
    {
        var nextPosition = BoardLoc + value;
        var bound_Y = !AtTop ? Bounds.y : Bounds.y + 1;

        if(nextPosition.x >= 0 && nextPosition.x < Bounds.x 
        && nextPosition.y > 0 && nextPosition.y < bound_Y)
        {
            BoardLoc = nextPosition;
            this.gameObject.transform.localPosition += Vector3.Scale(new Vector3(BlockDist, BlockDist, 0), value);
        }
    }

    public void SetPosition(Vector2 position)
    {
        this.gameObject.transform.localPosition = ZeroPosition;
        BoardLoc = Vector2.up;

        OnMove(position);
    }
}