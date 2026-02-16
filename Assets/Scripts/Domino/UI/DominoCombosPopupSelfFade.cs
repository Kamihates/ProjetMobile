using NaughtyAttributes;
using UnityEngine;
using System.Collections;

public class DominoCombosPopupSelfFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup popup;

    [SerializeField, Foldout("Settings"), HorizontalLine(2f, EColor.Red)] private float popupDelay = 0.3f;
    [SerializeField, Foldout("Settings")] private float popupFadeDuration = 0.2f;
    [SerializeField, Foldout("Settings"), HorizontalLine(2f, EColor.Red)] private float totalDamageDisplayedSecond = 2f;

    public IEnumerator StartPopupFade(DominoCombosPopup dominoCombosPopup)
    {
        yield return StartCoroutine(UIAnimations.Instance.DisplayForXSeconds(popupDelay, popupFadeDuration, popup));
        dominoCombosPopup.ReturnComboPopupToQueue(popup);
    }
}
