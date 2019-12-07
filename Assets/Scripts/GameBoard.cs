using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public BlockContainer BlockContainer;
    public SpriteManager BlockSM;
    public Vector2 BoardSize;
    public Vector2 CursorStartPosition;

    // Start is called before the first frame update
    void Start()
    {
        //BlockContainer = new BlockContainer(5, 0.05f);
        CursorStartPosition = new Vector2(2, 2);
    }

    // Update is called once per frame
    void Update()
    {
        // If GameState is Active, move the BlocksUp
        BlockContainer.Move();
    }
}