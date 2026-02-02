using NaughtyAttributes;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEditor.PlayerSettings;

public class DominoMovementController : MonoBehaviour
{
    public DominoPiece CurrentDomino { get => _currentDomino; set => _currentDomino = value; }
    private DominoPiece _currentDomino;
    
    [BoxGroup("Gestion du drag"), SerializeField]  private float _holdTime = 0.2f;
    [BoxGroup("Gestion du drag"), SerializeField] private float _dragDistance = 0.2f;


    private float _draggingChrono = 0; // chrono du temps qu'on reste appuyer sur le domino pour detecter un drag
    private bool _isDragged = false; // est ce que je suis en train de deplacer mon domino ?
    private bool _startDrag = false; // est ce que je suis en train de detecter un drag ?
    private Vector2 _pressStartPos; // position de la souris quand je clique sur le domino pour verifier si j'ai bougé pour detecter un drag plus vite que le timer

    private bool _canFall = false;
    public bool CanFall { get => _canFall; set => _canFall = value; }

    [SerializeField] private float _fallingSpeed;
    private float _currentFallingSpeed;
    private float _currentStepSpeed;
    [SerializeField] private DominoPlacementController dominoPlacement;


    private bool _startLongTap = false; // est ce que je suis en train de detecter un drag ?
    private float _LongTapChrono = 0;
    [SerializeField] private float _holdTapTime = 0.2f;


    private void Start()
    {
        _currentFallingSpeed = _fallingSpeed;
        _currentStepSpeed = TEST_GD.Instance.FallingStepStoppingTime;
    }


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

        if(_startLongTap)
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
                _currentFallingSpeed *= 4;
                _currentStepSpeed /= 2;
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
            _currentFallingSpeed = _fallingSpeed;
            _currentStepSpeed = TEST_GD.Instance.FallingStepStoppingTime;
        }

        // si je drag mon domino, je le bouge
        if (_isDragged)
        {
            _currentDomino.Visual.MoveOnX();
        }






        //if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        //{
        //    Interract();
        //}

        
            

    }


    private void FixedUpdate()
    {
        if (_currentDomino != null && _canFall)
        {
            Vector2 targetPos = dominoPlacement.GetFinalDestination(_currentDomino, new Vector2Int(-1, -1));
            Fall();

            // si on arrive a destination
            if ((Vector2.Distance(targetPos, _currentDomino.transform.position) < 0.01f) || _currentDomino.transform.position.y < targetPos.y) // evite un depassement
            {
                _canFall = false;
                _currentDomino.transform.position = targetPos; // on snap au cas ou

                DominoPiece dominoToDiscard = _currentDomino;

                _currentDomino = null;
                GridManager.Instance.OnDominoPlaced?.Invoke(dominoToDiscard);
            }

        }


    }

   
    private float _fallingStepChrono = 0;
    private bool _canDoOneStep = true;
    private void Fall()
    {
        // on fait tomber le domino a vitesse constante
        Vector2 newPos = _currentDomino.transform.position;

        if (TEST_GD.Instance != null)
        {
            if (TEST_GD.Instance.FallPerCase)
            {
                if (_canDoOneStep)
                {
                    newPos.y -= GridManager.Instance.CellSize;
                    _canDoOneStep = false;
                }
                _fallingStepChrono += Time.deltaTime;
                if (_fallingStepChrono >= _currentStepSpeed)
                {
                    _canDoOneStep = true;
                    _fallingStepChrono = 0;

                    
                }
            }
            else
            {
                newPos.y -= _currentFallingSpeed * Time.deltaTime;
            }
        }

        



        _currentDomino.transform.position = newPos;

    }

}
