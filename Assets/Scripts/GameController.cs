using UnityEngine;

//Parent controller for gameplay (need others for different gameplay modes)
public class GameController : MonoBehaviour
{
    public GameBoard PlayerGameBoard;
    public GameCursor PlayerCursor;

    void Awake() {
        foreach(var block in FindObjectsOfType<GameBlock>())
        {
            block.GetComponent<SpriteRenderer>().TransByDimensions(new Vector3(-0.5f, 0.5f, 0));
        }

        PlayerCursor.GetComponent<SpriteRenderer>().TransByDimensions(new Vector3(-0.5f, 0.5f, 0));
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