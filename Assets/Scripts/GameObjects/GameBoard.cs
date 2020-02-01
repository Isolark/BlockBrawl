using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public BlockContainer BlockContainer;
    public GameCursor Cursor;
    public Vector2 BoardSize;
    public Vector2 CursorStartPosition;

    public ILookup<float, Animator> PauseAnimLookup;
    public ILookup<float, SpriteRenderer> PauseSpriteLookup;

    public int Score;
    public int SpeedLv;
    private TimedAction RaiseSpeedLvTimer;
    private int MaxSpeedLv;
    private float RaiseAcceleration;

    private int MaxHP;
    private int HP;
    private int RegenAmount;
    private TimedAction RegenTimer;
    private float RegenDelay;

    private int DamageAmount;
    private TimedAction DamageTimer;
    private float DamageDelay; //Dependent on SpeedLv

    private TMP_Text CountdownTimeText;
    private TMP_Text CountdownReadyText;
    private int CountdownTime;
    public float CountdownDropSpeed;
    private readonly float COUNTDOWN_OFFSET_Y = 8;
    private readonly float COUNTDOWN_TIME_FINAL_Y = 9.8f;
    private readonly float COUNTDOWN_READY_FINAL_Y = 8.25f;

    public void Initialize(int maxSpeedLv, float baseRaiseSpeed, float baseRaiseAccel)
    {
        SpeedLv = 1;
        MaxSpeedLv = maxSpeedLv;
        RaiseAcceleration = baseRaiseAccel;

        Cursor.gameObject.SetActive(true);
        Cursor.AtTop = true;
        Cursor.LockToBoard(BoardSize);
        Cursor.gameObject.SetActive(false);
        BlockContainer.Initialize(BoardSize, baseRaiseSpeed);

        CountdownTime = 4;
        StartCountdown();
    }

    private void StartCountdown()
    {
        GetFadeChildren();
        FadeSprites(float.MaxValue, false);

        var textRectSize = new Vector2(6, 2);
        var timeText = TextMeshPooler.TMP.GetPooledObject("CountdownBig-TxtConfig", textRectSize, "Foreground", 0, "CountdownText", transform);
        var readyText = TextMeshPooler.TMP.GetPooledObject("CountdownSmall-TxtConfig", textRectSize, "Foreground", 0, "ReadyText", transform);

        CountdownTimeText = timeText.GetComponent<TMP_Text>();
        CountdownReadyText = readyText.GetComponent<TMP_Text>();

        CountdownTimeText.alignment = CountdownReadyText.alignment = TextAlignmentOptions.Center;
        CountdownTimeText.text = string.Empty;
        CountdownReadyText.text = "Ready";

        timeText.transform.localPosition = new Vector3(transform.position.x, COUNTDOWN_TIME_FINAL_Y, 0);
        readyText.transform.localPosition = new Vector3(transform.position.x, COUNTDOWN_OFFSET_Y + COUNTDOWN_READY_FINAL_Y, 0);

        MainController.MC.TransformManager.Add_LinearTimePos_Transform(readyText, new Vector2(0, COUNTDOWN_READY_FINAL_Y), CountdownDropSpeed, () => {
            Unpause();
            Cursor.gameObject.SetActive(true);
            var boardMiddleLoc = new Vector2(Mathf.Floor(BoardSize.x/2) - 1, Mathf.Floor(BoardSize.y/2));
            Cursor.StartAutoMoveLoc(boardMiddleLoc);
        });

        MainController.MC.AddTimedAction(() => { 
            RunCountdown(); 
            GameController.GameCtrl.CanMoveCursor = true; 
        }, 1);
    }

    private void RunCountdown()
    {
        if(CountdownTime == 1)
        {
            TextMeshPooler.TMP.RepoolTextMeshText(CountdownReadyText);
            TextMeshPooler.TMP.RepoolTextMeshText(CountdownTimeText);
            CountdownReadyText = CountdownTimeText = null;

            Cursor.AtTop = false;
            GameController.GameCtrl.StartGame();
            return;
        }

        CountdownTime--;

        //TODO: SoundFX each time
        CountdownTimeText.text = CountdownTime.ToString();
        MainController.MC.AddTimedAction(RunCountdown, 1);
    }

    //Manual trigger to actually start following whatever intro is involved
    public void StartGame()
    {
        if(RaiseSpeedLvTimer != null) 
        {
            MainController.MC.RemoveTimedAction(RaiseSpeedLvTimer);
        }
        RaiseSpeedLvTimer = MainController.MC.AddTimedAction(IncreaseSpeedLv, GameController.GameCtrl.RaiseSpeedLevelDelay, true);

        MainController.MC.PlayMusic("ScoreAttack");
    }

    public void OnUpdate()
    {
        BlockContainer.OnUpdate();
    }

    private void IncreaseSpeedLv()
    {
        if(SpeedLv >= MaxSpeedLv) { return; }

        SpeedLv++;

        var currentSpeed = BlockContainer.BaseRaiseSpeed;
        currentSpeed += RaiseAcceleration;

        if(SpeedLv % 50 == 0)
        {
            currentSpeed += RaiseAcceleration * 5;
            RaiseAcceleration += GameController.GameCtrl.RaiseDeltaAcceleration;

            if(SpeedLv == 100)
            {
                //Other things that happen at Speed 100
            }
        }
        
        BlockContainer.IncreaseSpeed(currentSpeed);
        GameScoreAtkCtrl.GameSA_Ctrl.ScoreModeMenu.SetSpeedLv(SpeedLv);
    }

    public void IncreaseScore(int blockCount, int chainCount)
    {
        var difficultyScoreVal = MainController.MC.GetData<DifficultyLv>("DifficultyLv").BaseScoreValue;
        var scoreMultipliers = MainController.MC.GetData<BlockMultipliers>("ScoreMultipliers");
        
        var comboMultiplier = scoreMultipliers.ComboMults[blockCount-1];
        var totalScore = Mathf.CeilToInt(blockCount * comboMultiplier * difficultyScoreVal);

        if(chainCount > 1)
        {
            var chainMultiplier = scoreMultipliers.ChainMults[chainCount-2];
            totalScore += Mathf.CeilToInt((chainCount - 1) * chainMultiplier * difficultyScoreVal);
        } 

        GameScoreAtkCtrl.GameSA_Ctrl.ScoreModeMenu.IncreaseScore(totalScore);
    }

    public void StartDamage()
    {

    }

    public void StopDamage()
    {

    }

    public void Pause()
    {
        GetFadeChildren();
        if(PauseSpriteLookup.Any()) { StartCoroutine(FadeSprites(10, false)); }
        if(CountdownReadyText != null) {
            CountdownReadyText.gameObject.SetActive(false);
            CountdownTimeText.gameObject.SetActive(false);
        }
    }

    private void GetFadeChildren()
    {
        var childObjList = new List<Transform>();
        BlockContainer.transform.GetAllChildrenRecursively(ref childObjList);

        var pauseSpriteList = new List<SpriteRenderer>();
        var pauseAnimList = new List<Animator>();

        foreach(var childObj in childObjList)
        {
            if(!childObj.gameObject.activeSelf) { continue; }

            SpriteRenderer sprRenderer;
            if(childObj.TryGetComponent<SpriteRenderer>(out sprRenderer))
            {
                pauseSpriteList.Add(sprRenderer);
            }
            Animator anim;
            if(childObj.TryGetComponent<Animator>(out anim))
            {
                pauseAnimList.Add(anim);
            }
        }

        PauseAnimLookup = pauseAnimList.ToLookup(x => x.speed);
        PauseSpriteLookup = pauseSpriteList.ToLookup(x => x.color.a);

        foreach(var anim in pauseAnimList) 
        {
            anim.speed = 0;
        }
    }

    public void Unpause()
    {
        ResetAnimations();
        if(PauseSpriteLookup.Any()) { StartCoroutine(FadeSprites(10, true)); }
        //BoardMask.gameObject.SetActive(false);
        if(CountdownReadyText != null && !CountdownReadyText.isActiveAndEnabled) {
            CountdownReadyText.gameObject.SetActive(true);
            CountdownTimeText.gameObject.SetActive(true);
        }
    }

    public void ResetAnimations()
    {
        foreach(var animSpeedGroup in PauseAnimLookup) 
        {
            var prevSpeed = animSpeedGroup.Key;

            foreach(var anim in animSpeedGroup)
            {
                anim.speed = prevSpeed;
            }
        }
    }

    private IEnumerator FadeSprites(float fadeMultiplier, bool isFadeIn)
    {
        var isComplete = false;

        while(!isComplete)
        {
            foreach(var spriteAlphaGroup in PauseSpriteLookup)
            {
                var finalAlpha = isFadeIn ? spriteAlphaGroup.Key : 0;

                foreach(var sprite in spriteAlphaGroup)
                {
                    if(!isComplete)
                    {
                        var nextAlpha = sprite.color.a + Time.deltaTime * fadeMultiplier * (isFadeIn ? 1 : -1);
                        if((isFadeIn && nextAlpha >= finalAlpha) || (finalAlpha == 0 && nextAlpha <= 0)) 
                        { 
                            isComplete = true; 
                            nextAlpha = finalAlpha;
                        }
                        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.g, nextAlpha * spriteAlphaGroup.Key);
                    }
                    else 
                    { 
                        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.g, finalAlpha * spriteAlphaGroup.Key);
                    } 
                }                
            }
            
            yield return null;
        }
    }

    public void InputConfirm()
    {
        BlockContainer.OnCursorConfirm(Cursor.BoardLoc);
    }

    public void InputTrigger(bool performed)
    {
        BlockContainer.OnInputTrigger(performed);
    }

    public void InputMove(Vector2 value)
    {
        Cursor.OnMove(value);
    }
}