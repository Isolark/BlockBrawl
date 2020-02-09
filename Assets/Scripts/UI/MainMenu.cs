using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//TODO: Link this up to the actual MenuController for logic and proper resetting?
public class MainMenu : GameMenu
{
    public MainMenuState CurrentState;
    public FlashingText StartLabel;
    public TMP_Text MenuTitle;


    public TMP_Text MusicVolumeLabel;
    public TMP_Text SoundVolumeLabel;


    public TMP_Text DifficultyValue;

    public CanvasRenderer RightMenuPanel;
    public CanvasRenderer BackPanel;
    public CanvasRenderer BottomPanel;

    private TMP_Text DescriptionText;

    private Slider MusicSlider;
    private TMP_Text MusicVolumeValue;
    private Slider SoundSlider;
    private TMP_Text SoundVolumeValue;

    private bool OptionChangedFlag;
    private Action CancelAction;

    // private int CurrentDifficulty;
    // private readonly int MIN_DIFFICULTY = 1;
    // private readonly int MAX_DIFFICULTY = 10;
    //private int MaxDifficultyLock;

    private IDictionary<string, Tuple<Vector2, Vector2>> RightPanelSizeList;

    void Awake()
    {
        // RightPanelSizeList = new Dictionary<string, Tuple<Vector2, Vector2>>();

        // var rPanelDscrptPos = new Vector2(240, -120);
        // var rPanelDscrptSize = new Vector2(420, 220);
        // var rPanelMenuPos = new Vector2(240, -140);
        // var rPanelMenuSize = new Vector2(480, 280);

        // RightPanelSizeList.Add("Description", new Tuple<Vector2, Vector2>(rPanelDscrptPos, rPanelDscrptSize));
        // RightPanelSizeList.Add("Menu", new Tuple<Vector2, Vector2>(rPanelMenuPos, rPanelMenuSize));
    }

    void Start()
    {
        MusicSlider = MusicVolumeLabel.transform.Find("MusicVolume-Slider").GetComponent<Slider>();
        MusicVolumeValue = MusicVolumeLabel.transform.Find("MusicVolume-Value").GetComponent<TMP_Text>();
        SoundSlider = SoundVolumeLabel.transform.Find("SoundVolume-Slider").GetComponent<Slider>();
        SoundVolumeValue = SoundVolumeLabel.transform.Find("SoundVolume-Value").GetComponent<TMP_Text>();

        DescriptionText = BottomPanel.transform.Find("DescriptionText").GetComponent<TMP_Text>();
    }
    public void Preinitialize()
    {
        RightMenuPanel.gameObject.SetActive(false);
        BottomPanel.gameObject.SetActive(false);

        StartLabel.gameObject.SetActive(true);
        StartLabel.Initialize();

        CurrentState = MainMenuState.PreMainMenu;
    }

    override public void Initialize()
    {
        base.Initialize(); 

        //TODO: Sound FX
        StartLabel.Deinitialize();

        //MenuTitle.gameObject.SetActive(true);
        CurrentState = MainMenuState.OnMainMenu;

        MusicSlider.value = MainController.MC.MusicPlayer.volume;
        MusicVolumeValue.text = Mathf.RoundToInt(MusicSlider.value * 100f).ToString();

        SoundSlider.value = MainController.MC.SoundFXPlayer.volume;
        SoundVolumeValue.text = Mathf.RoundToInt(SoundSlider.value * 100f).ToString();

        OptionChangedFlag = false;
        // CurrentDifficulty = 1;
        // MaxDifficultyLock = 10;

        //Darken VolumeSubMenu
        ChangeVolumeSubMenuColor(Color.black, 0.3f);

        //SetRightPanelConfig("Description");
        // RightMenuPanel.gameObject.SetActive(true);
        // DescriptionText = RightMenuPanel.GetComponentsInChildren<TMP_Text>().First(x => x.name == "DescriptionText");

        SetMenuList(MenuLists[0]);
    }

    // private void SetRightPanelConfig(string configName)
    // {
    //     var nextPos = RightPanelSizeList[configName].Item1;
    //     var nextSize = RightPanelSizeList[configName].Item2;

    //     var rPanelRect = RightMenuPanel.GetComponent<RectTransform>();
    //     rPanelRect.localPosition = nextPos;
    //     rPanelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nextSize.x);
    //     rPanelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, nextSize.y);
    // }

    override public void SetMenuList(GameMenuList menuList)
    {
        base.SetMenuList(menuList);
        DescriptionText.text = CurrentMenuList.CurrentMenuItem.ItemDescription;
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

    override public void InputMove(Vector2 value)
    {
        if(CurrentMenuList == null) { return; }
        CurrentMenuList.MoveCursor(value);
        
        DescriptionText.SetText(CurrentMenuList.CurrentMenuItem.ItemDescription);
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

        BottomPanel.gameObject.SetActive(false);

        BackPanel.GetComponent<CanvasGroup>().CrossFadeAlpha(this, 0, 0.1f);
        MainController.MC.TransformManager.Add_LinearTimePos_Transform(BackPanel.gameObject, BackPanel.transform.localPosition + new Vector3(0, -15, 0), 0.1f);
        SetMenuList(MenuLists.First(x => x.name == "MainMenuList"));
    }

    private void ToSinglePlayerMode()
    {
        DescriptionText.text = string.Empty;
        MainController.MC.AddTimedAction(() => { BottomPanel.gameObject.SetActive(true); }, 0.3f);

        BackPanel.GetComponent<CanvasGroup>().CrossFadeAlpha(this, 1, 0.1f);
        MainController.MC.TransformManager.Add_LinearTimePos_Transform(BackPanel.gameObject, BackPanel.transform.localPosition + new Vector3(0, 15, 0), 0.1f);
        SetMenuList(MenuLists.First(x => x.name == "1PlayerMenuList"));
    }

    private void ToOptions()
    {
        SetMenuList(MenuLists.First(x => x.name == "OptionsMenuList"));
    }

    private void ChangeDifficulty(int value)
    {
        var difficultyStr = DifficultyValue.text.Replace("< ", string.Empty).Replace(" >", string.Empty);
        var currentDifficulty = int.Parse(difficultyStr);

        //if(currentDifficulty )
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

    public void OnSPScoreModeSelection()
    {
        MainController.MC.PrevSceneIndex = SceneManager.GetActiveScene().buildIndex;
        MainController.MC.GoToScene("SP_ScoreMode");
    }

    public void OnSPBlockBattleModeSelection()
    {

    }
}