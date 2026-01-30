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
            newPos = new Vector2
            (
                Mathf.Clamp(mousePos.x, _currentDomino.transform.position.x, _currentDomino.transform.position.x + GridManager.Instance.CellSize),
                _currentDomino.transform.position.y
            );
        }
        // on bouge à gauche
        else
        {
            newPos = new Vector2
            (
                Mathf.Clamp(mousePos.x, _currentDomino.transform.position.x - GridManager.Instance.CellSize, _currentDomino.transform.position.x),
                _currentDomino.transform.position.y
            );

        }

        // si ca depasse de la grille

        if (Vector2.Distance(newPos, _currentDomino.transform.position) > 0.001f)
            _currentDomino.transform.position = newPos;


    }

    

    void Interract()
    {
        // raycast qui detecte sur quel element on clique et quoi faire 

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.TryGetComponent(out DominoPiece domino))
            {
                if (_spawner == null)
                    return;

                if (domino.PieceUniqueId == _spawner.CurrentDominoId)
                {
                    _currentDomino = hit.collider.gameObject;
                    _isDragged = true;
                }
                return;
            }
        }

    }
}
