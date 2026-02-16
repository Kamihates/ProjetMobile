using NaughtyAttributes;
using UnityEngine;
using System.Collections;
using TMPro;

public class DominoCombosPopupSelfFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup popup;

    [SerializeField, Foldout("Settings"), HorizontalLine(2f, EColor.Red)] private float popupDelay = 0.3f;
    [SerializeField, Foldout("Settings")] private float popupFadeDuration = 0.2f;

    private float popupValue;

    public IEnumerator StartPopupFade(DominoCombosPopup dominoCombosPopup, DominoCombosPopupSelfFade comboPop, float value)
    {

        popupValue = value;

        yield return StartCoroutine(UIAnimations.Instance.DisplayForXSeconds(popupDelay, popupFadeDuration, popup));
        dominoCombosPopup.ReturnComboPopupToQueue(popup);

        comboPop.UpdateTotalComboDamage(value);
    }

    public void UpdateTotalComboDamage(float totalDamage)
    {
        popupValue += totalDamage;

        GetComponent<TextMeshProUGUI>().text = popupValue.ToString();
    }
}
