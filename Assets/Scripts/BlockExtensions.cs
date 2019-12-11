using UnityEngine;

public static class BlockExtensions
{
    public static SpriteLibrary BlockSL;

    public static void Initialize(this Block block, BlockType type, Vector2 location, bool isOnBoard = false)
    {
        block.Type = type;
        block.BoardLoc = location;
        block.SetStates(isOnBoard);

        block.GetComponent<SpriteRenderer>().sprite = BlockSL.GetSpriteByName("Block-" + type.ToString());

        if(!isOnBoard) {
            block.BlockSprite.color = Color.Lerp(block.BlockSprite.color, Color.black, 0.45f);
        }
    }

    public static void OnEnterBoard(this Block block)
    {
        block.SetStates(true);
        block.BlockSprite.color = Color.white;
    }

    public static void InstantMove(this Block block, Vector3 moveVector)
    {
        block.BoardLoc += moveVector;

        moveVector.Scale(new Vector3(GameController.GC.BlockDist, GameController.GC.BlockDist, 0));
        block.transform.localPosition += moveVector;
    }

    public static void Move(this Block block, Vector3 moveVector)
    {
        block.BoardLoc += moveVector;

        moveVector.Scale(new Vector3(GameController.GC.BlockDist, GameController.GC.BlockDist, 0));
        block.transform.localPosition += moveVector;

        block.SetStates(false);
        //On Finish Move:
        block.SetStates(true);
    }

    public static void SetStates(this Block block, bool state)
    {
        block.IsChainable = block.IsComboable = block.IsMoveable = state;
    }
}
