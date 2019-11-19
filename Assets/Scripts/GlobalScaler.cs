public class GlobalScaler
{
    private GameState CurrentGameState;

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
    }
}