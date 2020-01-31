using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public BlockContainer BlockContainer;
    public GameCursor Cursor;
    public SpriteMask BoardMask;
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

    public void Initialize(int maxSpeedLv, float baseRaiseSpeed, float baseRaiseAccel)
    {
        SpeedLv = 1;
        MaxSpeedLv = maxSpeedLv;
        RaiseAcceleration = baseRaiseAccel;

        Cursor.LockToBoard(BoardSize, CursorStartPosition);
        BlockContainer.Initialize(BoardSize, baseRaiseSpeed);
    }

    //Manual trigger to actually start following whatever intro is involved
    public void BeginGame()
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

        if(PauseSpriteLookup.Any()) { StartCoroutine(FadeSprites(10, false)); }
    }

    public void Unpause()
    {
        ResetAnimations();
        if(PauseSpriteLookup.Any()) { StartCoroutine(FadeSprites(10, true)); }
        BoardMask.gameObject.SetActive(false);
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

        if(!isFadeIn) { BoardMask.gameObject.SetActive(true); }
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