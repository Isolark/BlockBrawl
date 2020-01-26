using TMPro;
using UnityEngine;

public class FlashingText : MonoBehaviour
{
    public TMP_Text TextObj;
    public bool IsFadingIn;
    public float FadeTime;
    public float FadeInDelay;
    public float FadeOutDelay;
    private TimedAction Fade_TA;

    public void Initialize()
    {
        IsFadingIn = false;
        if(Fade_TA == null) { Fade_TA = MainController.MC.AddTimedAction(OnFade, FadeOutDelay, true); }
    }

    public void OnFade()
    {
        if(IsFadingIn)
        {
            TextObj.CrossFadeAlpha(1, FadeTime, false);
            Fade_TA.SetTime(FadeOutDelay);
        }
        else
        {
            TextObj.CrossFadeAlpha(0, FadeTime, false);
            Fade_TA.SetTime(FadeInDelay);
        }
        IsFadingIn = !IsFadingIn;
        Fade_TA.Reset();
    }

    public void Deinitialize()
    {
        MainController.MC.RemoveTimedAction(Fade_TA);
        Fade_TA = null;
        gameObject.SetActive(false);
    }
}