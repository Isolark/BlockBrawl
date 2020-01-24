using System.Linq;
using UnityEngine;

//Parent controller for debugging in game (currently tailored to the GameController's scene)
public class DebugController : MonoBehaviour
{
    public static DebugController DC;

    void Awake()
    {
        //Singleton pattern
        if (DC == null) {
            DC = this;
        }
        else if (DC != this) {
            Destroy(gameObject);
        }     
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public void LocateBlock(Vector2 blockLoc)
    {
        var blockContainer = GameController.GameCtrl.PlayerGameBoard.BlockContainer;

        if(blockContainer.BlockList.ContainsKey(blockLoc))
        {
            var blockObj = blockContainer.BlockList[blockLoc].gameObject;
            blockObj.GetComponent<SpriteRenderer>().sprite = SpriteLibrary.SL.GetSpriteByName("Block");
            blockObj.SetActive(true);
        }
    }

    public void LogMovingBlockLocs()
    {
        var blockList = GameController.GameCtrl.PlayerGameBoard.BlockContainer.BlockList;

        foreach(var block in blockList.Where(x => x.Value.IsFallLocked || x.Value.IsFalling || x.Value.IsMoving))
        {
            block.Value.BlockSprite.color = Color.red;
        }
    }
}