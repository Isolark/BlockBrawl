using UnityEngine;

//Parent controller for gameplay (need others for different gameplay modes)
public class GameController : MonoBehaviour
{
    public GameBoard PlayerGameBoard;
    public GameCursor PlayerCursor;

    void Awake() {
        foreach(var block in FindObjectsOfType<GameBlock>())
        {
            block.gameObject.TransBySpriteDimensions(new Vector3(-0.5f, 0.5f, 0));
        }

        PlayerCursor.gameObject.TransBySpriteDimensions(PlayerGameBoard.gameObject, new Vector3((float)-1/3, (float)0.5/12, 0));
        PlayerCursor.LockToBoard(PlayerGameBoard.BoardSize);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}