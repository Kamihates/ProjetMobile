using NaughtyAttributes;
using System.Collections;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class DominoCombosPopupSelfFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup popup;

    [SerializeField, Foldout("Settings"), HorizontalLine(2f, EColor.Red)] private float popupDelay = 0.3f;
    [SerializeField, Foldout("Settings")] private float popupFadeDuration = 0.2f;

    private string popupValue;

    public IEnumerator StartPopupFade(DominoCombosPopup dominoCombosPopup, /*DominoCombosPopupSelfFade comboPop,*/ string value)
    {
        popupValue = value;

        TMP_Text tmpText = GetComponentInChildren<TMP_Text>();
        tmpText.text = value;


        yield return StartCoroutine(UIAnimations.Instance.DisplayForXSeconds(popupDelay, popupFadeDuration, popup));
        dominoCombosPopup.ReturnComboPopupToQueue(popup);

        //comboPop.UpdateTotalComboDamage(value);
    }

    public void UpdateTotalComboDamage(float totalDamage)
    {
        popupValue += totalDamage;

        GetComponent<TextMeshProUGUI>().text = popupValue.ToString();
       
    }
}
