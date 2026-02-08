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

    private IEnumerator FadeIn(float duration, CanvasGroup grp, bool fade = true)
    {
        if (fade)
            grp.gameObject.SetActive(true);

        float time = 0;

        while (time < duration)
        {

            float ratio = time / duration;

            grp.alpha = Mathf.Lerp(fade ? 0 : 1, fade ? 1 : 0, ratio);

            time += Time.unscaledDeltaTime;

            yield return null;
        }

        grp.alpha = fade ? 1 : 0;
        grp.interactable = fade;
        grp.blocksRaycasts = fade;


        if (!fade)
            grp.gameObject.SetActive(true);

        _currentCoroutine = null;
    } 

}
