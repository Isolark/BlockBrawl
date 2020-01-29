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

    // Start is called before the first frame update
    void Start()
    {
        Cursor.LockToBoard(BoardSize, CursorStartPosition);
        BlockContainer.Initialize(BoardSize);
    }
    public void OnUpdate()
    {
        BlockContainer.OnUpdate();
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

        GameController.GameCtrl.ScoreModeMenu.IncreaseScore(totalScore);
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