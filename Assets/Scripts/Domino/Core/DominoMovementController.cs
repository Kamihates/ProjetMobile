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
    
    [BoxGroup("Gestion du drag"), SerializeField]  private float holdTime = 0.2f;
    [BoxGroup("Gestion du drag"), SerializeField] private float _dragDistance = 0.2f;


    private float _time = 0; // chrono du temps qu'on reste appuyer sur le domino pour detecter un drag
    private bool _isDragged = false; // est ce que je suis en train de deplacer mon domino ?
    private bool _startDrag = false; // est ce que je suis en train de detecter un drag ?
    private Vector2 _pressStartPos; // position de la souris quand je clique sur le domino pour verifier si j'ai bougé pour detecter un drag plus vite que le timer


    private void Update()
    {


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
                        _currentDomino.Rotate();
                    }

                }
            }
        }


        // Si je suis en phase de detection de drag
        if (_startDrag)
        {
            _time += Time.deltaTime;

            Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            float distance = Vector2.Distance(currentPos, _pressStartPos);

            // si j'ai bougé ma souris, c'est que j'ai commencé à drag, on peut deplacer
            if (distance > _dragDistance)
            {
                _startDrag = false;
                _isDragged = true;
                _time = 0;
            }

            // si j'ai relaché ma souris, c'etait un slique simple, je tourne le domino
            if (Input.GetMouseButtonUp(0))
            {
                _currentDomino.Rotate();
                _startDrag = false;
            }

            // si le temps de drag est atteint, je commence à drag
            if (_time >= holdTime)
            {
                _time = 0f;
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
        }

        // si je drag mon domino, je le bouge
        if (_isDragged)
        {
            MoveOnX();
        }






        //if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        //{
        //    Interract();
        //}

        
            

    }


    void MoveOnX()
    {

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 center = _currentDomino.GetCenter();

        if (GridManager.Instance == null) return;

        // on bouge à droite
        if (mousePos.x > center.x + 0.1f)
        {
            // ca c'est plus proche du point de doite que la position on le met a droite
            if ( Mathf.Abs(mousePos.x - (center.x + GridManager.Instance.CellSize)) < Mathf.Abs(mousePos.x - center.x))
            {
                _currentDomino.transform.position = new Vector2(_currentDomino.transform.position.x + GridManager.Instance.CellSize, _currentDomino.transform.position.y);
                return;
            }
        }
        // on bouge à gauche
        else
        {
            // ca c'est plus proche du point de doite que la position on le met a droite
            if (Mathf.Abs(mousePos.x - (center.x - GridManager.Instance.CellSize)) < Mathf.Abs(mousePos.x - center.x))
            {
                _currentDomino.transform.position = new Vector2(_currentDomino.transform.position.x - GridManager.Instance.CellSize, _currentDomino.transform.position.y);
                return;
            }

        }



    }


}
