using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

public class DominoCombosPopup : MonoBehaviour
{
    [SerializeField, Foldout("Reference"), Required] private DominoCombos combos;

    [SerializeField, Foldout("UI")] private CanvasGroup comboPopupPrefab;
    [SerializeField, Foldout("UI")] private CanvasGroup totalDamageCG;
    [SerializeField, Foldout("UI")] private TMP_Text totalDamageText;

    [SerializeField, Foldout("Settings")] private float popupDelay = 0.3f;
    [SerializeField, Foldout("Settings")] private float popupFadeDuration = 0.5f;
    [SerializeField, Foldout("Settings")] private float totalDamageDisplayedSecond = 2f;

    [SerializeField, Foldout("Debug"), ReadOnly] private List<CanvasGroup> activeCombosPopups = new();

    private Coroutine comboRoutine;
    private Coroutine totalDamageRoutine;

    private void Start()
    {
        combos.OnComboChain += StartComboChain;
        combos.OnComboFinished += FinishCombo;
    }

    private void OnDestroy()
    {
        combos.OnComboChain -= StartComboChain;
        combos.OnComboFinished -= FinishCombo;
    }

    #region Affichage des combos en chaine

    private void StartComboChain(List<RegionPiece> regions)
    {
        // On stop la coroutine du combo précédent si elle est encore en cours
        if (comboRoutine != null)
            StopCoroutine(comboRoutine);

        comboRoutine = StartCoroutine(ComboChainCoroutine(regions));
    }

    private IEnumerator ComboChainCoroutine(List<RegionPiece> regions)
    {
        float delay = 0f;

        foreach (RegionPiece region in regions)
        {
            CanvasGroup popupCG = Instantiate(comboPopupPrefab, transform);
            popupCG.transform.position = region.transform.position;

            TMP_Text tmpText = popupCG.GetComponentInChildren<TMP_Text>();
            tmpText.text = $"+{combos.DamagePerCombo}";

            activeCombosPopups.Add(popupCG);

            StartCoroutine(UIAnimations.Instance.FadeIn(popupFadeDuration, popupCG, true));

            StartCoroutine(FadeAndRemovePopupAfterDelay(popupCG, delay));

            delay += popupDelay;
        }

        yield break;
    }

    private IEnumerator FadeAndRemovePopupAfterDelay(CanvasGroup popupCG, float delayBeforeFade)
    {
        if (popupCG == null) 
            yield break;

        yield return new WaitForSeconds(delayBeforeFade + totalDamageDisplayedSecond);

        if (!popupCG) 
            yield break; 

        yield return UIAnimations.Instance.FadeIn(popupFadeDuration, popupCG, false);

        activeCombosPopups.Remove(popupCG);

        if (popupCG != null)
            Destroy(popupCG.gameObject);
    }

    #endregion

    #region Desaffichage des combos en chaine

    private void FinishCombo(float totalDamage, float T1Multiplier = 0)
    {
        if (totalDamageRoutine != null)
        {
            StopCoroutine(totalDamageRoutine);
            totalDamageRoutine = null;
            totalDamageText.text = "";
        }

        if (activeCombosPopups.Count > 0)
            StartCoroutine(FadeOutAllPopups());

        totalDamageRoutine = StartCoroutine(DisplayForXSecondsTotalDamage(totalDamage, T1Multiplier));
    }

    private IEnumerator FadeOutAllPopups()
    {
        // Copie de la liste pour éviter de modifier pendant le foreach
        var popupsToFade = new List<CanvasGroup>(activeCombosPopups);

        foreach (CanvasGroup popup in popupsToFade)
        {
            if (popup != null)
            {
                StartCoroutine(FadeAndRemovePopupAfterDelay(popup, 0));
            }
        }

        activeCombosPopups.Clear();

        yield break;
    }

    #endregion

    #region Affichage des dégats totaux

    private IEnumerator DisplayForXSecondsTotalDamage(float totalDamage, float T1Multiplier)
    {
        if (T1Multiplier > 1f)
        {
            float comboDamage = totalDamage / (T1Multiplier * combos.T1Multipicator); 
            totalDamageText.text = $"-{comboDamage} (x{T1Multiplier})"; 
            yield return new WaitForSeconds(1f); 
        }

        totalDamageText.text = $"-{totalDamage}"; // On affiche le total de dégâts du combo
        yield return UIAnimations.Instance.DisplayForXSeconds(totalDamageDisplayedSecond, popupFadeDuration, totalDamageCG);
        totalDamageText.text = ""; // On reset le text une fois le total de dégâts affiché
    }

    #endregion
}