using System.Collections;
using UnityEngine;

public static class CanvasGroupExtensions
{
    public static void CrossFadeAlpha(this CanvasGroup canvasGroup, MonoBehaviour monoCtrl, float finalAlpha, float duration)
    {
        monoCtrl.StartCoroutine(canvasGroup.CrossFadeCR(finalAlpha, duration, finalAlpha > canvasGroup.alpha));
    }

    private static IEnumerator CrossFadeCR(this CanvasGroup canvasGroup, float finalAlpha, float duration, bool isFadeIn)
    {
        var startAlpha = canvasGroup.alpha;
        var currentTime = 0f;

        for(;;)
        {
            currentTime += Time.deltaTime;

            if(currentTime >= duration) { 
                canvasGroup.alpha = finalAlpha; 
                break;
            }

            canvasGroup.alpha = Mathf.Lerp(startAlpha, finalAlpha, currentTime / duration);
            yield return null;
        }
    }
}