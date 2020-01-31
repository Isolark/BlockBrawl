using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

//Parent controller for gameplay. Passes on inputs to relevant game objects
//TODO: OnPause(), trigger an "empty" input for all other bindings (stop player's hold timer, etc...)
public class GameBlockBattleCtrl : GameController
{
    //public ScoreModeMenu ScoreModeMenu;
    //public BlockBattleMenu BBMenu;
    public static GameBlockBattleCtrl GameBB_Ctrl;
    //public 

    void Awake()
    {
        //Singleton pattern
        if (GameBB_Ctrl == null) {
            GameBB_Ctrl = this;
        }
        else if (GameBB_Ctrl != this) {
            Destroy(gameObject);
        }     
    }

    // Start is called before the first frame update
    // override protected void Start()
    // {
    // }

    // Update is called once per frame
    // override protected void Update()
    // {
    // }
}