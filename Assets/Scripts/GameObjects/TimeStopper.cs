using System;
using UnityEngine;

public class TimeStopper : MonoBehaviour
{
    public TimeBar Bar;
    private Vector2 InitialBarPos;

    public float MaxTime;
    public float CurrentTime;
    public bool IsTimeStopped;

    private float BaseTimeStop;
    private BlockMultipliers TimeStopMultipliers;
    //private Action TimeUpCallback;

    public void Initialize()//Action callback)
    {
        BaseTimeStop = MainController.MC.GetData<DifficultyLv>("DifficultyLv").BaseTimeStopValue;
        TimeStopMultipliers = MainController.MC.GetData<BlockMultipliers>("TimeStopMultipliers");

        if(Bar != null)
        {
            Bar.Initialize();
        }

        //TimeUpCallback = callback;
        IsTimeStopped = false;
    }

    private void Deinitialize()
    {
        //if(Timer != null) { MainController.MC.RemoveTimedAction(Timer); }
    }

    void Destroy()
    {
        Deinitialize();
    }

    public void ShowBar()
    {
        if(Bar != null && !Bar.IsDisplayed) { 
            Bar.MoveBar(true); 
        }
    }

    public void HideBar()
    {
        if(Bar != null) { 
            Bar.MoveBar(false); 
        }
    }

    public void AddTime(int combo, int chain)
    {
        if(!IsTimeStopped) { IsTimeStopped = true; }
        
        //Add Combo Time
        float comboMultiplier = 0;
        var maxCombo = TimeStopMultipliers.ComboMults.Count;

        if(combo > maxCombo)
        {
            comboMultiplier += TimeStopMultipliers.ComboMaxAdd * (combo - maxCombo);
            combo = maxCombo;
        }
        comboMultiplier += TimeStopMultipliers.ComboMults[combo-1];

        var totalTime = Mathf.Ceil(combo * comboMultiplier * BaseTimeStop);

        //Add Chain Score
        if(chain > 1)
        {
            float chainMultiplier = 0;
            var maxChain = TimeStopMultipliers.ChainMults.Count + 2;

            if(chain > maxChain)
            {
                chainMultiplier += TimeStopMultipliers.ChainMaxAdd * (chain - maxChain);
                chain = maxChain;
            }

            chainMultiplier += TimeStopMultipliers.ChainMults[chain-2];
            totalTime += Mathf.Ceil((chain - 1) * chainMultiplier * BaseTimeStop);
        } 

        totalTime = totalTime / 1000;

        if(totalTime > CurrentTime)
        {
            CurrentTime = totalTime;
            Bar.SetTargetTime(CurrentTime);
        }
    }

    public bool UpdateTime(float deltaTime)
    {
        CurrentTime -= deltaTime;

        if(CurrentTime <= 0)
        {
            CurrentTime = 0;
            IsTimeStopped = false;
            Bar.ResetTime();
            //TimeUpCallback();
        }
        else
        {
            Bar.SetTargetTime(CurrentTime);
        }


        return IsTimeStopped;
    }

    public void ResetTime()
    {
        CurrentTime = 0;
        Bar.ResetTime();
    }

    public void StartFuse()
    {
        // if(!IsFuseOn && (IsTakingDamage || IsRegenActive))
        // {
        //     //TODO: SFX?
        //     FuseAnim.gameObject.SetActive(true);
        //     FuseSpr.color = IsTakingDamage ? Color.red : Color.blue;
        //     FuseAnim.SetTrigger("Play");
        //     IsFuseOn = true;

        //     ToggleFuse(true);
        // }
    }

    public void StopFuse()
    {
        // if(IsFuseOn)
        // {
        //     FuseSpr.transform.localPosition = new Vector2(BarSpr.transform.localPosition.x + BarSpr.bounds.size.y/2, FuseSpr.transform.localPosition.y);
        //     FuseAnim.SetTrigger("Stop");
        //     IsFuseOn = false;

        //     ToggleFuse(false);
        // }
    }

    private void UpdateFuse()
    {
        // var nextPos = BarSpr.bounds.size.y * (HP / MaxHP) * (IsTakingDamage ? -1 : 1);
        // FuseSpr.transform.localPosition = 

    }
}