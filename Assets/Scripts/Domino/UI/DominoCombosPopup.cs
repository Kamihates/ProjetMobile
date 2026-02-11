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

    [SerializeField, Foldout("Settings")] private float popupDelay = 1f;
    [SerializeField, Foldout("Settings")] private float popupFadeDuration = 0.5f;
    [SerializeField, Foldout("Settings")] private float totalDamageDisplayedSecond = 5f;

    [SerializeField, Foldout("Debug"), ReadOnly] private float currentTotalDamage = 0;
    [SerializeField, Foldout("Debug"), ReadOnly] private List<CanvasGroup> activeCombosPopups = new();

    private Coroutine comboRoutine;
    private Coroutine totalDamageRoutine;

    private void Start()
    {
        combos.OnComboChain += StartComboChain;
        combos.OnT1Multiplier += DisplayT1Multiplier;
        combos.OnComboFinished += FinishCombo;
    }

    // Appelé à chaque fois qu'on a un t1, pour afficher le multiplicateur du t1 à coté des dégats totaux
    private void DisplayT1Multiplier(int multiplier)
    {
        totalDamageText.text += $" x{multiplier}"; 
    }

    #region Affichage des combos en chaine

    private void StartComboChain(List<RegionPiece> regions)
    {
        if (comboRoutine != null)
            StopCoroutine(comboRoutine);

        StartCoroutine(ComboChainCoroutine(regions));
    }

    private IEnumerator ComboChainCoroutine(List<RegionPiece> regions)
    {
        foreach(RegionPiece region in regions)
        {
            CanvasGroup popupCG = Instantiate(comboPopupPrefab, transform);
            popupCG.transform.position = region.transform.position;

            TMP_Text tmpText = popupCG.GetComponentInChildren<TMP_Text>();
            tmpText.text = combos.DamagePerCombo.ToString(); // On affiche les dégâts de base du combo

            activeCombosPopups.Add(popupCG); // On ajoute le popup de dmg à la liste des popups actifs

            UIAnimations.Instance.Fade(popupFadeDuration, popupCG, true); // On appelle la fonction pour fade le popup

            currentTotalDamage += combos.DamagePerCombo; // Les dégats totaux sont augmentés progressivement par rapports au dégats de base du combo
            UpdateTotalDamage(); 

            yield return new WaitForSeconds(popupDelay);
        }
    }

    #endregion

    #region Desaffichage des combos en chaine

    private void FinishCombo(float totalDamage)
    {
        // On stop l'ancien coroutine de désaffichage du combo pour en lancer un nouveau à chaque fois qu'on finit un combo
        if (totalDamageRoutine != null)
            StopCoroutine(totalDamageRoutine);

        totalDamageRoutine = StartCoroutine(FinishComboCoroutine());
    }

    private IEnumerator FinishComboCoroutine()
    {
        foreach (CanvasGroup popupCG in activeCombosPopups)
        {
            UIAnimations.Instance.Fade(popupFadeDuration, popupCG, false);
        }
        activeCombosPopups.Clear(); // On vide la liste des popups actifs

        yield return StartCoroutine(FadeOutTotalDamage()); // On lance la coroutine pour faire disparaitre le total des dégats après un certain temps
    }

    #endregion

    #region Affichage des dégats totaux

    private void UpdateTotalDamage()
    {
        totalDamageCG.alpha = 1; 
        //UIAnimations.Instance.Fade(popupFadeDuration, totalDamageCG, true);

        totalDamageText.text = $"-{currentTotalDamage.ToString()}";
    }

    private IEnumerator FadeOutTotalDamage()
    {
        yield return new WaitForSeconds(totalDamageDisplayedSecond);
        UIAnimations.Instance.Fade(popupFadeDuration, totalDamageCG, false);
        currentTotalDamage = 0; // On reset les dégats totaux pour le prochain combo
    }

    #endregion
}