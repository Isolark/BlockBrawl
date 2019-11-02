using UnityEngine;

public enum GameState
{
    Loading = 0, //[Root State] Loading: Incapable of action until loading completes
    Active = 1, //[Root State] Active: Capable of taking new action
	InGame_Active = 1,
	InGame_Paused = 2
}

//Responsible for Saving/Loading & Handling Major Game Events
public class MainController : MonoBehaviour
{
    private GameState CurrentGameState;
    public static Vector2 UnitResolution;
    public static int PPU = 100;

    void Awake() {
        CurrentGameState = GameState.Loading;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentGameState = GameState.Active;
    }

    // Update is called once per frame
    void Update()
    {
        //NOTE: Realistically, this should be caught somewhere and bubble up here
		// if(InputHub.IH.Pause.IsDown()) {
		// 	if(CurrentGameState == GameState.InGame_Active) {
		// 		CurrentGameState = GameState.InGame_Paused;
		// 	} else {
		// 		CurrentGameState = GameState.InGame_Active;
		// 	}
		// }

        if(UnitResolution.x != Screen.currentResolution.width * PPU || UnitResolution.y != Screen.currentResolution.height * PPU)
        {
            SetUnitResolution();
        }
    }

    // Set UnitResolution based on current screen size
    void SetUnitResolution()
    {
        UnitResolution = new Vector2(Screen.currentResolution.width / PPU, Screen.currentResolution.height / PPU);
    }
}