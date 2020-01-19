using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

//Parent controller for gameplay. Passes on inputs to relevant game objects
//TODO: OnPause(), trigger an "empty" input for all other bindings (stop player's hold timer, etc...)
public class MenuController : InputController
{
    public static MenuController MC;

    void Awake()
    {
        //Singleton pattern
        if (MC == null) {
            MC = this;
        }
        else if (MC != this) {
            Destroy(gameObject);
        }

        GS_Current = GameState.Loading;
    }

    // Start is called before the first frame update
    override protected void Start()
    {
        GS_Current = GameState.Active;
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();

        if(GS_Current == GameState.Active)
        {
        }
    }

    // public void OnMove(CallbackContext context)
    // {
    // }

    // public void OnConfirm(CallbackContext context)
    // {
    //     if(!context.performed) { return; }
    //     if(GS_Current == GameState.Active)
    //     {
    //         SendMessage("OnConfirm");
    //     }
    // }

    // public void OnCancel(CallbackContext context)
    // {
    //     if(!context.performed) { return; }
    // }

    // public void OnTrigger(CallbackContext context)
    // {
    //     if(!context.performed && !context.canceled) { return; }
    //     if(GS_Current == GameState.Active)
    //     {
    //         SendMessage("OnTrigger", context.performed);
    //     }
    // }
}