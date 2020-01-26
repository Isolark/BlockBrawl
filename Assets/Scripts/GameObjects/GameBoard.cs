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

    //public IEnumerable<Animator> PauseAnimList;
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

    public void Pause()
    {
        //PauseAnimList = BlockContainer.gameObject.GetComponentsInChildren<Animator>().Where(x => x.isActiveAndEnabled == true);
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

        // var pauseAnimList = childObjList.Where(x => x.comp).GetComponentsInChildren<Animator>().Where(x => x.isActiveAndEnabled == true);
        PauseAnimLookup = pauseAnimList.ToLookup(x => x.speed);
        PauseSpriteLookup = pauseSpriteList.ToLookup(x => x.color.a);

        foreach(var anim in pauseAnimList) 
        {
            anim.speed = 0;
        }

        //if(PauseSpriteLookup.Any()) { StartCoroutine(FadeSprites(0, -10)); }
    }

    public void Unpause()
    {
        Reset();
        if(PauseSpriteLookup.Any()) { StartCoroutine(FadeSprites(1, 10)); }
        BoardMask.gameObject.SetActive(false);
    }

    public void Reset()
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

    private IEnumerator FadeSprites(float finalAlpha, float fadeMultiplier)
    {
        var isComplete = false;

        while((finalAlpha == 1 && PauseSpriteLookup.Any(x => x.First().color.a < 1) || (finalAlpha == 0 && PauseSpriteLookup.Any(x => x.First().color.a > 0))))
        {
            foreach(var spriteAlphaGroup in PauseSpriteLookup)
            {
                foreach(var sprite in spriteAlphaGroup)
                {
                    if(!isComplete)
                    {
                        var nextAlpha = sprite.color.a + Time.deltaTime * fadeMultiplier;
                        if((finalAlpha == 1 && nextAlpha >= 1) || (finalAlpha == 0 && nextAlpha <= 0)) 
                        { 
                            isComplete = true; 
                            nextAlpha = finalAlpha;
                        }
                        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.g, nextAlpha);
                    }
                    else 
                    { 
                        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.g, finalAlpha);
                    } 
                }

            }
            yield return null;
        }

        BoardMask.gameObject.SetActive(finalAlpha == 0);
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