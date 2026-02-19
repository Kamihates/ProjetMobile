using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;


public class DominoMovementController : MonoBehaviour
{
    public DominoPiece CurrentDomino
    {
        get => _currentDomino; set
        {
            _currentDomino = value;
            if (value != null)
            {
                _currentDomino.FallController.Init(_fallingSpeed, _stepSpeed);
            }
        }
    }
    private DominoPiece _currentDomino;
    [SerializeField] private DominoPlacementController dominoPlacement;

    [HorizontalLine(color: EColor.Blue)]
    [BoxGroup("Gestion du drag"), SerializeField, Label("temps du hold en seconde avant de detecter un drag")]  private float _holdTime = 0.2f;
    [BoxGroup("Gestion du drag"), SerializeField, Label("distance minimal avant de detecter un drag")] private float _dragDistance = 0.2f;


    //private float _draggingChrono = 0; // chrono du temps qu'on reste appuyer sur le domino pour detecter un drag
    //private bool _isDragged = false; // est ce que je suis en train de deplacer mon domino ?
    //private bool _startDrag = false; // est ce que je suis en train de detecter un drag ?
    //private Vector2 _pressStartPos; // position de la souris quand je clique sur le domino pour verifier si j'ai bougé pour detecter un drag plus vite que le timer

    [HorizontalLine(color: EColor.Blue)]
    [BoxGroup("Vitesse du domino"), SerializeField, Label("Vitesse de fall d'un domino")] private float _fallingSpeed;
    [BoxGroup("Vitesse du domino"), SerializeField, Label("attente en seconde avant de descendre a la case suivante")] private float _stepSpeed;


    private bool _startLongTap = false; // est ce que je suis en train de detecter un drag ?
    private float _LongTapChrono = 0;
    //private bool _isMoving = false;
    private bool _isAccelerating = false;
    private bool _hasDragged = false;
    
    [HorizontalLine(color: EColor.Blue)]
    [BoxGroup("Chute rapide"),SerializeField, Label("temps du hold en seconde pour activer la descente rapide")] private float _holdTapTime = 0.2f;

    [BoxGroup("-- deplacement d'un domino --"), EnableIf("FallPerCase"), Header("temps entre chaque step en secondes")]
    public float FallingStepStoppingTime = 1f;

    private Camera _cam;
    private void Awake()
    {
        _cam = Camera.main;
    }

    Vector2 _mousePos = Vector2.zero;
    Vector2 _stoppingMousePos = Vector2.zero;
    private void Update()
    {
        if (_currentDomino == null) return;
        if (GameManager.Instance.CurrentState != GameState.InGameState) return;

        // detection d'un appuie

        if (Input.GetMouseButtonDown(0))
        {
            _mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
            _stoppingMousePos = _mousePos;
            if (GridManager.Instance != null)
            {
                if (GridManager.Instance.IsInGrid(new List<Vector2> { _mousePos }, false, true))
                {
                    // est ce que je maintient ? 
                    _startLongTap = true;
                }
            }
        }

        if (_startLongTap)
        {
            // si ma souris bouge a droite ou gauche, on deplace le domino
            _LongTapChrono += Time.deltaTime;
            Vector2 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


            // si ma souris bouge, de deplace
            if (Vector2.Distance(currentMousePos, _stoppingMousePos) > 0.03f && !_isAccelerating)
            {
                _hasDragged = true;
                if (_currentDomino.Visual.MoveOnX(_stoppingMousePos))
                {
                    _stoppingMousePos = currentMousePos;
                    _LongTapChrono = 0;      
                }
            }

            // si le timer depasse, on applique le fall x2
            if (_LongTapChrono >= _holdTapTime && !_hasDragged && !_isAccelerating)
            {
                if (GameManager.Instance.NoGravityMode)
                {
                    _currentDomino.FallController.IsTapToFall = true;
                }
                _currentDomino.FallController.Init(_fallingSpeed * 4, _stepSpeed / 2);
                _isAccelerating = true;
                _LongTapChrono = 0;
                AudioManager.Instance.PlaySFX(AudioManager.Instance.DataAudio.DominoDash);
            }

            if (Input.GetMouseButtonUp(0))
            {
                // on rotate et desatcive le long tap
                if (!_isAccelerating && !_hasDragged)
                    _currentDomino.Visual.Rotate();

                _isAccelerating = false;
                _startLongTap = false;
                _LongTapChrono = 0;
                _hasDragged = false;

                _currentDomino.FallController.Init(_fallingSpeed, _stepSpeed);
                CurrentDomino.FallController.IsTapToFall = false;
            }

        }

        

    }

    //private void Update()
    //{
    //    if (_currentDomino == null) return;
    //    if (GameManager.Instance.CurrentState != GameState.InGameState) return;

    //    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    float clickRadius = 0.3f;
    //    //Collider2D hit = Physics2D.OverlapCircle(pos, clickRadius);
    //    Collider2D[] hits = Physics2D.OverlapCircleAll(pos, clickRadius);


    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        // est ce que je clique sur mon domino ? 
    //        foreach (Collider2D hit in hits)
    //        {
    //            DominoPiece domino = hit.gameObject.GetComponentInParent<DominoPiece>();
    //            if (domino != null)
    //            {
    //                // mon domino est celui qui est en train de tomber ? 
    //                if (domino.PieceUniqueId == _currentDomino.PieceUniqueId)
    //                {
    //                    // je me prepare a drag
    //                    _startDrag = true;
    //                    _startLongTap = false;
    //                    _pressStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    //                    return;
    //                }
    //            }
    //        }

    //        // je suis pas sur mon domino, est ce que je suis dans la grille ? 
    //        if (!_startDrag)
    //        {
    //            if (GridManager.Instance != null)
    //            {
    //                if (GridManager.Instance.IsInGrid(new List<Vector2> { pos }, false))
    //                {
    //                    // est ce que je maintient ? 
    //                    _startLongTap = true;
    //                }
    //            }
    //        }
    //    }

    //    if (_startLongTap && !_isDragged)
    //    {
    //        _LongTapChrono += Time.deltaTime;

    //        if (Input.GetMouseButtonUp(0))
    //        {
    //            _currentDomino.Visual.Rotate();
    //            _startLongTap = false;
    //            _LongTapChrono = 0f;

    //        }


    //        if (_LongTapChrono >= _holdTapTime)
    //        {
    //            if (GameManager.Instance.NoGravityMode)
    //            {
    //                _currentDomino.FallController.IsTapToFall = true;
    //            }
    //           _currentDomino.FallController.Init(_fallingSpeed * 4, _stepSpeed / 2);


    //            _startLongTap = false;
    //            _LongTapChrono = 0f;
    //        }
    //    }


    //    // Si je suis en phase de detection de drag
    //    if (_startDrag)
    //    {
    //        _draggingChrono += Time.deltaTime;

    //        Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        float distance = Vector2.Distance(currentPos, _pressStartPos);

    //        // si j'ai bougé ma souris, c'est que j'ai commencé à drag, on peut deplacer
    //        if (distance > _dragDistance)
    //        {
    //            _startDrag = false;
    //            _isDragged = true;
    //            _draggingChrono = 0;

    //            _startLongTap = false; 
    //            _LongTapChrono = 0f;
    //            return;
    //        }

    //        // si j'ai relaché ma souris, c'etait un slique simple, je tourne le domino
    //        if (Input.GetMouseButtonUp(0))
    //        {
    //            _currentDomino.Visual.Rotate();
    //            _startDrag = false;
    //        }

    //        // si le temps de drag est atteint, je commence à drag
    //        if (_draggingChrono >= _holdTime)
    //        {
    //            _draggingChrono = 0f;
    //            if (_startDrag)
    //            {
    //                _startDrag = false;
    //                _isDragged = true;

    //                _startLongTap = false; 
    //                _LongTapChrono = 0f;
    //                return;
    //            }
    //        }
    //    }


    //    // si je relache la souris, j'arrete de drag mon domino si je le draggait
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        _isDragged = false;
    //        _currentDomino.FallController.Init(_fallingSpeed, _stepSpeed);
    //        CurrentDomino.FallController.IsTapToFall = false;
    //    }

    //    // si je drag mon domino, je le bouge
    //    if (_isDragged)
    //    {
    //        _currentDomino.Visual.MoveOnX();
    //    }
    //}

}
