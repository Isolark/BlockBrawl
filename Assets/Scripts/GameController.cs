using UnityEngine;

//Parent controller for gameplay (need others for different gameplay modes)
public class GameController : MonoBehaviour
{
    public GameBoard PlayerGameBoard;
    public GameCursor PlayerCursor;

    void Awake() {
        bool movedCursor = false;
        foreach(var block in FindObjectsOfType<GameBlock>())
        {
            block.gameObject.TransBySpriteDimensions(new Vector3(-0.5f, 0.5f, 0));
            if(!movedCursor)
            {
                Debug.Log(block.gameObject.GetComponent<SpriteRenderer>().bounds.size);

                PlayerCursor.gameObject.TransBySpriteDimensions(block.gameObject, new Vector3(-1f, 0.5f, 0));
                movedCursor = true;
            }
        }
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