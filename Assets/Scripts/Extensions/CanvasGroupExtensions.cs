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

    public static void SlideCrossFadeAlpha(this CanvasGroup canvasGroup, MonoBehaviour monoCtrl, float finalAlpha, Vector2 finalPos, float duration)
    {
        monoCtrl.StartCoroutine(canvasGroup.SlideCrossFadeCR(finalAlpha, finalPos, duration, finalAlpha > canvasGroup.alpha));
    }

    private static IEnumerator SlideCrossFadeCR(this CanvasGroup canvasGroup, float finalAlpha, Vector2 finalPos, float duration, bool isFadeIn)
    {
        var startAlpha = canvasGroup.alpha;
        var startPos = canvasGroup.transform.localPosition;
        var currentTime = 0f;

        for(;;)
        {
            currentTime += Time.deltaTime;

            if(currentTime >= duration) { 
                canvasGroup.alpha = finalAlpha; 
                canvasGroup.transform.localPosition = finalPos;
                break;
            }

            var t = currentTime / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, finalAlpha, t);
            canvasGroup.transform.localPosition = Vector2.Lerp(startPos, finalPos, t);
            yield return null;
        }
    }
}