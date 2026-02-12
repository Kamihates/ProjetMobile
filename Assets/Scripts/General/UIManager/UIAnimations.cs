using System.Collections;
using UnityEngine;

public class UIAnimations : MonoBehaviour
{
    private Coroutine _currentCoroutine = null;

    public static UIAnimations Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Fade(float duration, CanvasGroup grp, bool fade = true)
    {
        if (_currentCoroutine == null)
        {
            _currentCoroutine = StartCoroutine(FadeIn(duration, grp, fade));
        }
    }

    public IEnumerator DisplayForXSeconds(float seconds, float fadeDuration, CanvasGroup grp)
    {
        yield return StartCoroutine(FadeIn(fadeDuration, grp));
        yield return new WaitForSeconds(seconds);
        yield return StartCoroutine(FadeIn(fadeDuration, grp, false));
    }

    public IEnumerator FadeIn(float duration, CanvasGroup grp, bool fade = true)
    {
        if(grp == null) yield break;

        float time = 0;

        while (time < duration)
        {

            if (grp == null) yield break;

            float ratio = time / duration;

            grp.alpha = Mathf.Lerp(fade ? 0 : 1, fade ? 1 : 0, ratio);

            time += Time.unscaledDeltaTime;

            yield return null;
        }

        if(grp != null)
        {
            grp.alpha = fade ? 1 : 0;
            grp.interactable = fade;
            grp.blocksRaycasts = fade;
        }

        _currentCoroutine = null;
    } 

}
