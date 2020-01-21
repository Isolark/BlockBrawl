using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : GameMenu
{
    public MainMenuState CurrentState;
    public TMP_Text StartLabel;
    public TMP_Text MenuTitle;
    public SpriteRenderer MenuCursor;

    public TMP_Text MusicVolumeLabel;
    public Slider MusicSlider;
    public TMP_Text MusicVolumeValue;
    public TMP_Text SoundVolumeLabel;
    public Slider SoundSlider;
    public TMP_Text SoundVolumeValue;

    private bool OptionChangedFlag;
    private Action CancelAction;


    // Start is called before the first frame update
    void Start()
    {
        CurrentState = MainMenuState.PreMainMenu;
    }

    private void Initialize()
    {
        //TODO: Sound FX
        StartLabel.gameObject.SetActive(false);
        MenuTitle.gameObject.SetActive(true);
        CurrentState = MainMenuState.OnMainMenu;

        MusicSlider.value = MainController.MC.MusicPlayer.volume;
        MusicVolumeValue.text = Mathf.RoundToInt(MusicSlider.value * 100f).ToString();

        SoundSlider.value = MainController.MC.SoundFXPlayer.volume;
        SoundVolumeValue.text = Mathf.RoundToInt(SoundSlider.value * 100f).ToString();

        OptionChangedFlag = false;

        //Darken VolumeSubMenu
        ChangeVolumeSubMenuColor(Color.black, 0.3f);

        SetMenuList(MenuLists[0]);
    }

    override public void SetMenuList(GameMenuList menuList)
    {
        base.SetMenuList(menuList);

        MenuTitle.text = CurrentMenuList.Title;
        CurrentMenuList.Initialize(MenuCursor);
    }

    public void InputStart()
    {
        if(CurrentState == MainMenuState.PreMainMenu && StartLabel.gameObject.activeSelf) 
        {
            Initialize();
        } 
    }

    private void CursorSelect()
    {
    }

    public void InputCancel()
    {
        if(CurrentState == MainMenuState.PreMainMenu) { return; }

        if(CancelAction != null) {
            CancelAction();
            return;
        }

        CurrentMenuList.CancelSelection();
    }

    public void InputConfirm()
    {
        if(CurrentState == MainMenuState.PreMainMenu) { return; }

        CurrentMenuList.ConfirmSelection();
    }

    public void InputTrigger()
    {
    }


    //Menu Item Callbacks
    private void ToMain()
    {
        if(OptionChangedFlag) 
        {
            MainController.MC.SaveOptions();
            OptionChangedFlag = false;
        }
        SetMenuList(MenuLists.First(x => x.name == "MainMenuList"));
    }

    private void ToOptions()
    {
        SetMenuList(MenuLists.First(x => x.name == "OptionsMenuList"));
    }

    private void MusicVolumeSlide()
    {
        MainController.MC.MusicPlayer.volume = MusicSlider.value;
        MusicVolumeValue.text = Mathf.RoundToInt(MusicSlider.value * 100f).ToString();

        OptionChangedFlag = true;
    }

    private void SoundVolumeSlide()
    {
        MainController.MC.SoundFXPlayer.volume = SoundSlider.value;
        SoundVolumeValue.text = Mathf.RoundToInt(SoundSlider.value * 100f).ToString();

        OptionChangedFlag = true;
    }

    private void ToggleVolumeSubMenu()
    {
        var optionsMenu = MenuLists.First(x => x.name == "OptionsMenuList");

        foreach(var menuItem in optionsMenu.MenuItemList)
        {
            menuItem.Value.IsSelectable = !menuItem.Value.IsSelectable;
        }

        if(optionsMenu.CurrentMenuItem.name == "Volume-Label")
        {
            optionsMenu.SetCurrentMenuItem(optionsMenu.MenuItemList.First(x => x.Value.name == "MusicVolume-Label").Value);
            ChangeVolumeSubMenuColor(Color.white, 1);

            CancelAction = ToggleVolumeSubMenu;
        }
        else
        {
            optionsMenu.SetCurrentMenuItem(optionsMenu.MenuItemList.First(x => x.Value.name == "Volume-Label").Value);
            ChangeVolumeSubMenuColor(Color.black, 0.3f);

            CancelAction = null;
        }
    }

    private void ChangeVolumeSubMenuColor(Color lerpColor, float percentage)
    {
        MusicVolumeLabel.color = SoundVolumeLabel.color = Color.Lerp(MusicVolumeLabel.color, lerpColor, percentage);
        MusicVolumeValue.color = SoundVolumeValue.color = Color.Lerp(MusicVolumeValue.color, lerpColor, percentage);

        foreach(var sliderImage in MusicSlider.GetComponentsInChildren<Image>().Concat(SoundSlider.GetComponentsInChildren<Image>()))
        {
            sliderImage.color = Color.Lerp(sliderImage.color, lerpColor, percentage);
        }
    }

    public void OnSPZenModeSelection()
    {
        SceneManager.LoadScene("Main");
    }

    public void OnSPBlockBattleModeSelection()
    {

    }
}
