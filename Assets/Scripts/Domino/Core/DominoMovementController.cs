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
    
    [BoxGroup("Gestion du drag"), SerializeField]  private float _holdTime = 0.2f;
    [BoxGroup("Gestion du drag"), SerializeField] private float _dragDistance = 0.2f;


    private float _draggingChrono = 0; // chrono du temps qu'on reste appuyer sur le domino pour detecter un drag
    private bool _isDragged = false; // est ce que je suis en train de deplacer mon domino ?
    private bool _startDrag = false; // est ce que je suis en train de detecter un drag ?
    private Vector2 _pressStartPos; // position de la souris quand je clique sur le domino pour verifier si j'ai bougé pour detecter un drag plus vite que le timer


    [BoxGroup("Vitesse du domino"), SerializeField] private float _fallingSpeed;
    [BoxGroup("Vitesse du domino"), SerializeField] private float _stepSpeed;

    [SerializeField] private DominoPlacementController dominoPlacement;


    private bool _startLongTap = false; // est ce que je suis en train de detecter un drag ?
    private float _LongTapChrono = 0;
    [SerializeField] private float _holdTapTime = 0.2f;




    // A RESTRUCTURER pour lisibilité
    private void Update()
    {
        if (_currentDomino == null) return;



        if (Input.GetMouseButtonDown(0))
        {
            // est ce que je clique sur mon domino ? 

            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit.collider != null)
            {
                DominoPiece domino = hit.collider.gameObject.GetComponentInParent<DominoPiece>();
                if (domino != null)
                {
                    // mon domino est celui qui est en train de tomber ? 
                    if (domino.PieceUniqueId == _currentDomino.PieceUniqueId)
                    {
                        // je me prepare a drag
                        _startDrag = true;
                        _pressStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
            }

            // je suis pas sur mon domino, est ce que je suis dans la grille ? 
            if (!_startDrag)
            {
                if (GridManager.Instance != null)
                {
                    if (GridManager.Instance.IsInGrid(new List<Vector2> { pos }))
                    {
                        // est ce que je maintient ? 
                        _startLongTap = true;

                    }

                }
            }
        }

        if (_startLongTap)
        {
            _LongTapChrono += Time.deltaTime;

            if (Input.GetMouseButtonUp(0))
            {
                _currentDomino.Visual.Rotate();
                _startLongTap = false;
                _LongTapChrono = 0f;

            }


            if (_LongTapChrono >= _holdTapTime)
            {
                _currentDomino.FallController.Init(_fallingSpeed * 4, _stepSpeed / 2);
                _startLongTap = false;
                _LongTapChrono = 0f;
            }
        }


        // Si je suis en phase de detection de drag
        if (_startDrag)
        {
            _draggingChrono += Time.deltaTime;

            Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float distance = Vector2.Distance(currentPos, _pressStartPos);

            // si j'ai bougé ma souris, c'est que j'ai commencé à drag, on peut deplacer
            if (distance > _dragDistance)
            {
                _startDrag = false;
                _isDragged = true;
                _draggingChrono = 0;
            }

            // si j'ai relaché ma souris, c'etait un slique simple, je tourne le domino
            if (Input.GetMouseButtonUp(0))
            {
                _currentDomino.Visual.Rotate();
                _startDrag = false;
            }

            // si le temps de drag est atteint, je commence à drag
            if (_draggingChrono >= _holdTime)
            {
                _draggingChrono = 0f;
                if (_startDrag)
                {
                    _startDrag = false;
                    _isDragged = true;
                }
            }
        }


        // si je relache la souris, j'arrete de drag mon domino si je le draggait
        if (Input.GetMouseButtonUp(0))
        {
            _isDragged = false;
            _currentDomino.FallController.Init(_fallingSpeed, _stepSpeed);
        }

        // si je drag mon domino, je le bouge
        if (_isDragged)
        {
            _currentDomino.Visual.MoveOnX();
        }
    }

}
