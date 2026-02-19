using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class StatsVisual : MonoBehaviour
{
    [SerializeField] private DominoFusion _fusionManager;
    [SerializeField] private DominoCombos _comboManager;


    [BoxGroup("Win Panel"),SerializeField] private TextMeshProUGUI _maxComboDamageTxt;
    [BoxGroup("Win Panel"), SerializeField] private TextMeshProUGUI _scoreTxt;
    [BoxGroup("Win Panel"), SerializeField] private TextMeshProUGUI _highScoreTxt;
    [BoxGroup("Win Panel"), SerializeField] private TextMeshProUGUI _fusionCountTxt;

    [BoxGroup("Death Panel"), SerializeField] private TextMeshProUGUI _maxComboDamageTxtOver;
    [BoxGroup("Death Panel"), SerializeField] private TextMeshProUGUI _scoreTxtOver;
    [BoxGroup("Death Panel"), SerializeField] private TextMeshProUGUI _highScoreTxtOver;
    [BoxGroup("Death Panel"), SerializeField] private TextMeshProUGUI _fusionCountTxtOver;


    public static StatsVisual Instance { get; private set; }
    private void Awake() { Instance = this; }


    public void DisplayStatsOnWin()
    {
        _maxComboDamageTxt.gameObject.SetActive(false);
        _scoreTxt.gameObject.SetActive(false);
        _highScoreTxt.gameObject.SetActive(false);
        _fusionCountTxt.gameObject.SetActive(false);

        // update des scores

        _maxComboDamageTxt.text = "Max Combo Damage : " + _comboManager.MaxComboDamage.ToString();
        _scoreTxt.text = "Score : " + _comboManager.CurrentTotalDamage.ToString();

        _highScoreTxt.text = "High Score : " + PlayerPrefs.GetFloat("MaxDamage").ToString();
        _fusionCountTxt.text = "Fusion : " + _fusionManager.FusionCount.ToString();

        // affichage dans tt les modes

        _maxComboDamageTxt.gameObject.SetActive(true);
        _fusionCountTxt.gameObject.SetActive(true);


        // on affiche les stats differentes selon les modes
        if (GameManager.Instance.IsInfiniteState)
        {
            _highScoreTxt.gameObject.SetActive(true);
            _scoreTxt.gameObject.SetActive(true);
        }
    }

    public void DisplayStatsOnDeath()
    {
        _maxComboDamageTxtOver.gameObject.SetActive(false);
        _scoreTxtOver.gameObject.SetActive(false);
        _highScoreTxtOver.gameObject.SetActive(false);
        _fusionCountTxtOver.gameObject.SetActive(false);

        // update des scores

        _maxComboDamageTxtOver.text = "Max Combo Damage : " + _comboManager.MaxComboDamage.ToString();
        _scoreTxtOver.text = "Score : " + _comboManager.CurrentTotalDamage.ToString();

        _highScoreTxtOver.text = "High Score : " + PlayerPrefs.GetFloat("MaxDamage").ToString();
        _fusionCountTxtOver.text = "Fusion : "  + _fusionManager.FusionCount.ToString();

        // affichage dans tt les modes

        _maxComboDamageTxtOver.gameObject.SetActive(true);
        _fusionCountTxtOver.gameObject.SetActive(true);


        // on affiche les stats differentes selon les modes
        if (GameManager.Instance.IsInfiniteState)
        {
            _highScoreTxtOver.gameObject.SetActive(true);
            _scoreTxtOver.gameObject.SetActive(true);
        }
    }

}
