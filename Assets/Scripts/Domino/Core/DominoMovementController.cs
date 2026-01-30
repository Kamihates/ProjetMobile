using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class DominoMovementController : MonoBehaviour
{
    public int CurentDominoID => _currentDominoID;
    private int _currentDominoID;

    [SerializeField] private DominoSpawner _spawner;


    private bool _isDragged = false;
    private List<RaycastResult> _results = new List<RaycastResult>();

    private GameObject _currentDomino;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Interract();
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Interract();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragged = false;
        }

        if (_isDragged && _currentDomino != null)
        {
            MoveOnX();

        }
    }


    void MoveOnX()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 newPos = _currentDomino.transform.position;

        if (GridManager.Instance == null) return;

        // on bouge à droite
        if (mousePos.x > _currentDomino.transform.position.x)
        {
            // ca c'est plus proche du point de doite que la position on le met a droite
            if ( Mathf.Abs(mousePos.x - (_currentDomino.transform.position.x + GridManager.Instance.CellSize)) < Mathf.Abs(mousePos.x - _currentDomino.transform.position.x))
            {
                _currentDomino.transform.position = new Vector2(_currentDomino.transform.position.x + GridManager.Instance.CellSize, _currentDomino.transform.position.y);
                return;
            }
        }
        // on bouge à gauche
        else
        {
            // ca c'est plus proche du point de doite que la position on le met a droite
            if (Mathf.Abs(mousePos.x - (_currentDomino.transform.position.x - GridManager.Instance.CellSize)) < Mathf.Abs(mousePos.x - _currentDomino.transform.position.x))
            {
                _currentDomino.transform.position = new Vector2(_currentDomino.transform.position.x - GridManager.Instance.CellSize, _currentDomino.transform.position.y);
                return;
            }

        }

        // si ca depasse de la grille
        // pas fait


    }

    

    void Interract()
    {
        // raycast qui detecte sur quel element on clique et quoi faire 

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

        if (hit.collider != null)
        {
            DominoPiece domino = hit.collider.gameObject.GetComponentInParent<DominoPiece>();
            if (domino != null)
            {
                if (_spawner == null)
                    return;

                Debug.Log(domino.PieceUniqueId + " / " + _spawner.CurrentDominoId);
                if (domino.PieceUniqueId == _spawner.CurrentDominoId)
                {
                    _currentDomino = domino.gameObject;
                    _isDragged = true;
                }
                return;
            }
        }

    }
}
