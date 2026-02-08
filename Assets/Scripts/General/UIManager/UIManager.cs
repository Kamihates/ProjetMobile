using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private float _menuFadeDuration = 1;

    private CanvasGroup _currentMenu = null;

    [SerializeField] private CanvasGroup _LostPanel;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameLost += DisplayLostScreen;
        }

        DeactivatePanel(_LostPanel);
    }


    private void DeactivatePanel(CanvasGroup canvas)
    {
        canvas.gameObject.SetActive(false);
        canvas.alpha = 0;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameLost -= DisplayLostScreen;
        }
    }


    private void DisplayLostScreen()
    {
        GameManager.Instance.Pause(true);
        OpenMenu(_LostPanel);
    }

    public void OpenMenu(CanvasGroup canvas)
    {
        if (_currentMenu != null)
        {
            CloseMenu(_currentMenu);
        }

        UIAnimations.Instance.Fade(_menuFadeDuration, canvas, true);
        _currentMenu = canvas;
    }

    public void CloseMenu(CanvasGroup canvas)
    {
        UIAnimations.Instance.Fade(_menuFadeDuration, canvas, false);
        _currentMenu = null;
    }
}
