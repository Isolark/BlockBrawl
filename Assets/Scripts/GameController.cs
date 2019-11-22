using UnityEngine;

//Parent controller for gameplay (need others for different gameplay modes)
public class GameController : MonoBehaviour
{
    public GameBoard PlayerGameBoard;
    public GameCursor PlayerCursor;

    void Awake() {
        var gameBoardSize = PlayerGameBoard.GetComponent<SpriteRenderer>().bounds.size;
        var gameCursorSize = PlayerCursor.GetComponent<SpriteRenderer>().bounds.size;
        
        PlayerCursor.transform.localPosition = new Vector2(gameCursorSize.x / 2 - gameBoardSize.x / 2 - PlayerCursor.padding, -PlayerCursor.padding);
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