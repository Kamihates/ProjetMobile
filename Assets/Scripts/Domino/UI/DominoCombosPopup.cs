using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;

public class DominoCombosPopup : MonoBehaviour
{
    [SerializeField, Foldout("Reference"), Required] private DominoCombos combos;

    [SerializeField, Foldout("UI")] private CanvasGroup comboPopupPrefab;
    [SerializeField, Foldout("UI")] private CanvasGroup weaknessTxt;
    [SerializeField, Foldout("UI")] private CanvasGroup resistanceTxt;
    [SerializeField, Foldout("UI")] private CanvasGroup totalDamageCG;
    [SerializeField, Foldout("UI")] private TMP_Text totalDamageText;

    [SerializeField, Foldout("Settings")] private int initialComboPopupQueueSize = 10;

    [SerializeField, Foldout("Settings"), HorizontalLine(2f, EColor.Red)] private float popupDelay = 0.3f;
    [SerializeField, Foldout("Settings")] private float popupFadeDuration = 0.5f;
    [SerializeField, Foldout("Settings"), HorizontalLine(2f, EColor.Red)] private float totalDamageDisplayedSecond = 2f;

     private Queue <CanvasGroup> comboPopupQueue = new();

    private void Start()
    {
        combos.OnComboChain += StartComboChain;
        combos.OnComboFinished += FinishCombo;

        InitializeComboPopupQueue();
    }

    private void OnDestroy()
    {
        combos.OnComboChain -= StartComboChain;
        combos.OnComboFinished -= FinishCombo;
    }

    #region PopupToQueue

    private void InitializeComboPopupQueue()
    {
        for (int i = 0; i < initialComboPopupQueueSize; i++)
            CreateNewComboPopupInQueue();
    }

    private void CreateNewComboPopupInQueue()
    {
        CanvasGroup popup = Instantiate(comboPopupPrefab, transform);
        popup.alpha = 0f;
        popup.gameObject.SetActive(false);
        comboPopupQueue.Enqueue(popup); // On ajoute la popup dans la queue
    }

    private CanvasGroup GetPopupFromQueue()
    {
        if (comboPopupQueue.Count == 0)
            CreateNewComboPopupInQueue();

        CanvasGroup popup = comboPopupQueue.Dequeue();
        popup.gameObject.SetActive(true);
        return popup;
    }

    public void ReturnComboPopupToQueue(CanvasGroup popup)
    {
        popup.alpha = 0f;
        popup.gameObject.SetActive(false);
        comboPopupQueue.Enqueue(popup);
    }

    #endregion

    #region Affichage des combos en chaine

    private void StartComboChain(List<Vector2Int> regions, float value, float t1Multiplicator, bool weakness, bool resistance)
    {
        StartCoroutine(ComboChainCoroutine(regions, value, t1Multiplicator, weakness, resistance));
    }

    private IEnumerator ComboChainCoroutine(List<Vector2Int> regions, float value, float t1Multiplicator, bool weakness, bool resistance)
    {
        
        foreach (Vector2Int Index in regions)
        {
            CanvasGroup popupCG = GetPopupFromQueue();
            popupCG.transform.position = GridManager.Instance.GetCellPositionAtIndex(Index);


            if (weakness)
            {
                StartCoroutine(popupCG.gameObject.GetComponent<DominoCombosPopupSelfFade>().StartPopupFade(this, /*selfFadePopUp,*/"- " + value.ToString()));
            }
            else if (resistance)
            {
                StartCoroutine(popupCG.gameObject.GetComponent<DominoCombosPopupSelfFade>().StartPopupFade(this, /*selfFadePopUp,*/"- " + value.ToString()));
            }
            else
            {
                StartCoroutine(popupCG.gameObject.GetComponent<DominoCombosPopupSelfFade>().StartPopupFade(this, /*selfFadePopUp,*/"- " + value.ToString()));
            }

           yield return new WaitForSeconds(popupDelay);
        }

        if (t1Multiplicator > 0)
        {
            yield return new WaitForSeconds(0.3f);

            Vector2 targetPos = GeneralVisualController.Instance.GetCenterPosition(regions);
            CanvasGroup popupTotal = GetPopupFromQueue();

            popupTotal.transform.position = targetPos;

            DominoCombosPopupSelfFade selfFadePopUp = popupTotal.gameObject.GetComponent<DominoCombosPopupSelfFade>();
            StartCoroutine(selfFadePopUp.StartPopupFade(this, "x " + t1Multiplicator));
        }
        

        yield break;
    }

    #endregion

    #region Desaffichage des combos en chaine

    private void FinishCombo(float totalDamage, float T1Multiplier, bool weakness, bool resistance)
    {
        StartCoroutine(DisplayForXSecondsTotalDamage(totalDamage, T1Multiplier));

        //if (weakness)
        //{
        //    ShowWeaknessAndResistance(true);
        //}
        //else if (resistance)
        //{
        //    ShowWeaknessAndResistance(false);
        //}
    }

    #endregion

    #region Affichage des dégats totaux

    private IEnumerator DisplayForXSecondsTotalDamage(float totalDamage, float T1Multiplier)
    {
        if (T1Multiplier > 1f)
        {
            float comboDamage = totalDamage / (T1Multiplier * combos.T1Multipicator); 
            totalDamageText.text = $"-{(int)comboDamage} (x{T1Multiplier})"; 
            yield return new WaitForSeconds(1f); 
        }

        totalDamageText.text = $"-{(int)totalDamage}"; // On affiche le total de dégâts du combo
        yield return UIAnimations.Instance.DisplayForXSeconds(totalDamageDisplayedSecond, popupFadeDuration, totalDamageCG);
        totalDamageText.text = ""; // On reset le text une fois le total de dégâts affiché
    }

    #endregion

    #region Affichage Resistance ou faiblesse

    //private void ShowWeaknessAndResistance(bool isweakness)
    //{
    //    StartCoroutine(UIAnimations.Instance.DisplayForXSeconds(2, 0.1f, isweakness ? weaknessTxt : resistanceTxt));
    //}



    #endregion
}