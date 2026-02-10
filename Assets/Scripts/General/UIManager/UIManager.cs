using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField, Foldout("Settings")] private float _menuFadeDuration = 1;
    [SerializeField, Foldout("Settings")] private float displayedSeconds = 2;

    [SerializeField, Foldout("Panel"), Required] private CanvasGroup splashScreenPanel;
    [SerializeField, Foldout("Panel"), Required] private CanvasGroup titlePanel;
    [SerializeField, Foldout("Panel"), Required] private CanvasGroup menuPanel;
    [SerializeField, Foldout("Panel"), Required] private CanvasGroup pausePanel;
    [SerializeField, Foldout("Panel"), Required] private CanvasGroup settingsPanel;
    [SerializeField, Foldout("Panel"), Required] private CanvasGroup tutoPanel;
    [SerializeField, Foldout("Panel"), Required] private CanvasGroup lostPanel;
    [SerializeField, Foldout("Panel"), Required] private CanvasGroup winPanel;

    [SerializeField, Foldout("Debug"), ReadOnly] private CanvasGroup _currentPanel = null;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.OnStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState gameState)
    {
        //CloseMenu();

        switch(gameState)
        {
            case GameState.SplashScreenState:
                if(splashScreenPanel == null)
                {
                    Debug.LogWarning("SplashScreenPanel is not assigned in the inspector"); return;
                }
                StartCoroutine(SplashScreen());
                break;
            case GameState.TitleScreenState:
                if (titlePanel == null)
                {
                    Debug.LogWarning("Title Panel is not assigned in the inspector"); return;
                }
                OpenMenu(titlePanel);
                break;
            case GameState.MenuState:
                if (menuPanel == null)
                {
                    Debug.LogWarning("Menu Panel is not assigned in the inspector"); return;
                }
                OpenMenu(menuPanel);
                break;
            case GameState.InGameState:
                CloseMenu();
                break;
            case GameState.PauseState:
                if (pausePanel == null)
                {
                    Debug.LogWarning("Pause Panel is not assigned in the inspector"); return;
                }
                OpenMenu(pausePanel);
                break;
            case GameState.SettingsState:
                if(settingsPanel == null)
                {
                    Debug.LogWarning("Settings Panel is not assigned in the inspector"); return;
                }
                OpenMenu(settingsPanel);
                break;
            case GameState.TutoState:
                if (tutoPanel == null)
                {
                    Debug.LogWarning("Tuto Panel is not assigned in the inspector"); return;
                }
                OpenMenu(tutoPanel);
                break;
            case GameState.LoseState:
                if (lostPanel == null)
                {
                    Debug.LogWarning("Lost Panel is not assigned in the inspector"); return;
                }
                OpenMenu(lostPanel);
                break;
            case GameState.WinState:
                if(winPanel == null)
                {
                    Debug.LogWarning("Win Panel is not assigned in the inspector"); return;
                }
                OpenMenu(winPanel);
                break;
        }
        
    }

    private IEnumerator SplashScreen()
    {
        yield return UIAnimations.Instance.DisplayForXSeconds(displayedSeconds, _menuFadeDuration, splashScreenPanel);
        GameManager.Instance.ChangeState(GameState.TitleScreenState);
    }

    //private void DeactivatePanel(CanvasGroup canvas)
    //{
    //    canvas.gameObject.SetActive(false);
    //    canvas.alpha = 0;
    //    canvas.interactable = false;
    //    canvas.blocksRaycasts = false;
    //}

    public void OpenMenu(CanvasGroup canvas)
    {
        StartCoroutine(OpenPanel(canvas));
    }

    public IEnumerator OpenPanel(CanvasGroup canvas)
    {
        if (_currentPanel != null)
        {
            CloseMenu();
            yield return new WaitForSecondsRealtime(_menuFadeDuration);
        }

        _currentPanel = canvas;
        UIAnimations.Instance.Fade(_menuFadeDuration, canvas, true);
    }

    public void CloseMenu()
    {
        if(_currentPanel != null)
            UIAnimations.Instance.Fade(_menuFadeDuration, _currentPanel, false);

        _currentPanel = null;
    }
}