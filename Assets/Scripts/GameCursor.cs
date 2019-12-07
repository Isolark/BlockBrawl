using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameCursor : MonoBehaviour
{
    public Vector2 ZeroLocation;
    public Vector2 Bounds;
    public Vector2 CurrentPosition;
    public float Padding;
    public float MoveDist;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Set zero position (assumed set by gameCtrl) & bounds
    public void LockToBoard(Vector2 boardSize, Vector2 startingPosition)
    {
        ZeroLocation = gameObject.transform.localPosition;
        Bounds = boardSize - new Vector2(1, 0);
        CurrentPosition = Vector2.zero;

        OnMove(startingPosition);

        Debug.Log(this.GetComponent<SpriteRenderer>().sprite.name);
        this.GetComponent<SpriteRenderer>().sprite.name = "Block-Blue";
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

    public void OnMove(Vector2 value)
    {
        var nextPosition = CurrentPosition + value;

            Debug.Log(nextPosition);
            Debug.Log(CurrentPosition);

        if(nextPosition.x >= 0 && nextPosition.x < Bounds.x 
        && nextPosition.y >= 0 && nextPosition.y < Bounds.y)
        {
            CurrentPosition = nextPosition;
            this.gameObject.transform.localPosition += Vector3.Scale(new Vector3(MoveDist, MoveDist, 0), value);
        }
    }

    public void SetPosition(Vector2 position)
    {
        this.gameObject.transform.localPosition = ZeroLocation;
        CurrentPosition = Vector2.zero;

        OnMove(position);
    }
}