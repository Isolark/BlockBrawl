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
        CurrentPosition = new Vector2();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Set zero position (assumed set by gameCtrl) & bounds
    public void LockToBoard(Vector2 boardSize)
    {
        ZeroLocation = gameObject.transform.localPosition;
        Bounds = boardSize - new Vector2(1, 0);

        CurrentPosition.Set(0, 0);
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
        Debug.Log(new Vector2(v.x, v.y));

        var nextPosition = CurrentPosition + v;

        if(nextPosition.x >= 0 && nextPosition.x < Bounds.x 
        && nextPosition.y >= 0 && nextPosition.y < Bounds.y)
        {
            CurrentPosition = nextPosition;
            this.gameObject.transform.localPosition += Vector3.Scale(new Vector3(MoveDist, MoveDist, 0), v);
        }
    }
}
