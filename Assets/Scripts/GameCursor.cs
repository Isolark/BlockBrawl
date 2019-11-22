using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameCursor : MonoBehaviour
{
    public float padding;
    public float moveDist;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnConfirm(InputValue value)
    {
        Debug.Log("Bleh");
    }

    public void OnMove(InputValue value)
    {
        var v = value.Get<Vector2>();
        Debug.Log(new Vector2(v.x, v.y));
        this.gameObject.transform.localPosition += Vector3.Scale(new Vector3(moveDist, moveDist, 0), v);
    }
}
