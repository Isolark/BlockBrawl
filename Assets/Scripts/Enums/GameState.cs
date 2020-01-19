public enum GameState
{
    Loading = 0, //[Root State] Loading: Incapable of action until loading completes
    Active = 1, //[Root State] Active: Capable of taking new action
	MenuOpen = 2 //Menu Open: "Freeze" GameElements & Do not pass inputs
}