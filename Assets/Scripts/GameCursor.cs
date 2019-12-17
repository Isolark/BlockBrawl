using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameCursor : MonoBehaviour
{
    public Vector2 ZeroPosition;
    public Vector2 Bounds;
    public Vector2 BoardLoc;
    public float Padding;
    public float BlockDist;
    public bool AtTop; //While true, allows moving to top row

    // Start is called before the first frame update
    void Start()
    {
        BlockDist = GameController.GC.BlockDist;
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Set zero position (assumed set by gameCtrl) & bounds
    public void LockToBoard(Vector2 boardSize, Vector2 startingPosition)
    {
        transform.localPosition = ZeroPosition = Vector2.zero;
        transform.localPosition += new Vector3(-BlockDist * 2, 0.5f);
        Bounds = boardSize - new Vector2(1, 0);
        BoardLoc = Vector2.up;

        OnMove(startingPosition);
    }

    public void OnConfirm(InputValue value)
    {
    }

    public void OnCancel(InputValue value)
    { 
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

    public void OnMove(Vector2 value)
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