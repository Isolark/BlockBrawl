using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameCursor : DirectionInputReceiver
{
    public BlockContainer BlockContainer;
    public Vector2 ZeroPosition;
    public Vector2 Bounds;
    public Vector2 BoardLoc;
    public float Padding;
    public float BlockDist;
    public float AutoMoveDelay;

    // Set zero position (assumed set by gameCtrl) & bounds
    public void LockToBoard(Vector2 boardSize)
    {
        BlockDist = GameController.GameCtrl.BlockDist;
        transform.localPosition = ZeroPosition = Vector2.zero;
        transform.localPosition += new Vector3(-BlockDist * 2, 0.5f);
        Bounds = boardSize - Vector2.right;
        BoardLoc = Vector2.up;

        OnMove(Bounds - Vector2.one, false);
    }

    public void StartAutoMoveLoc(Vector2 finalBoardLoc)
    {
        if(finalBoardLoc == BoardLoc) { return; }

        Vector2 nextMove;

        if(BoardLoc.y > finalBoardLoc.y) { nextMove = Vector2.down; }
        else if(BoardLoc.y < finalBoardLoc.y) { nextMove = Vector2.up; }
        else if(BoardLoc.x > finalBoardLoc.x) { nextMove = Vector2.left; }
        else if(BoardLoc.x < finalBoardLoc.x) { nextMove = Vector2.right; }
        else { return; }

        MainController.MC.AddTimedAction(() => { 
            MoveCursor(nextMove); 
            StartAutoMoveLoc(finalBoardLoc);
        }, AutoMoveDelay);
    }

    public void StepAutoMoveLoc(Vector2 nextBoardLoc)
    {

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
        var bound_Y = !BlockContainer.AtTop ? Bounds.y : Bounds.y + 1;

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