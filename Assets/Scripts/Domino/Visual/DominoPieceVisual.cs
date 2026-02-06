using NUnit.Framework.Internal;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

public class DominoPieceVisual : MonoBehaviour
{
    private DominoPiece _piece;

    public void Init(DominoPiece dominoLogique)
    {
        _piece = dominoLogique;
    }

    public void Rotate()
    {
        if (_piece.Data.Regions.Count != 2) return;


        

        // on test 3 rotations max, si aucune marche on garde l'actuelle
        for (int t = 1; t < 4; t++)
        {
            int targetRotation = _piece.Rotation + t;
            if (targetRotation > 3)
                targetRotation = 0;

            Vector2 tagetPos = transform.GetChild(1).localPosition;

            switch (targetRotation)
            {
                case 0:
                    // on trourne vers le haut
                    tagetPos = new Vector2(0, +GridManager.Instance.CellSize);

                    break;
                case 1:
                    // on trourne vers la droite
                    tagetPos = new Vector2(+GridManager.Instance.CellSize, 0);
                    break;
                case 2:
                    // on trourne vers le bas
                    tagetPos = new Vector2(0, -GridManager.Instance.CellSize);
                    break;
                case 3:
                    // on trourne vers la gauche
                    tagetPos = new Vector2(-GridManager.Instance.CellSize, 0);
                    break;
                default:
                    break;

            }

            // si la piece est en main, mais en current piece, on peut la tourner 
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.CurrentDomino == null || GameManager.Instance.CurrentDomino.PieceUniqueId != _piece.PieceUniqueId)
                {
                    transform.GetChild(1).localPosition = tagetPos;
                    _piece.Rotation = targetRotation;
                    return;
                }
            }

            // on regarde si la rotation ne depace pas de la grille : 
            if (transform.GetChild(1).TryGetComponent(out RegionPiece piece))
            {
                if (!GridManager.Instance.IsRegionInGrid(tagetPos + (Vector2)piece.transform.position))
                {
                    // on peut pas tourner, on essaye la rotation suivante
                    continue;
                }
                // si ca depasse pas la grille, on regarde si il n'y a pas d'elements genant mais de base ca devrait deja fonctionner avec le calcul de position final
                else
                {
                    transform.GetChild(1).localPosition = tagetPos;
                    _piece.Rotation = targetRotation;
                    return;
                }

            }
        }        
    }
    public void UpdateVisual()
    {
        for (int r = 0; r < _piece.Data.Regions.Count; r++)
        {
            if (transform.GetChild(r).TryGetComponent<RegionPiece>(out RegionPiece region))
            {

                region.Init(_piece.Data.Regions[r], _piece.Data.IsDominoFusion, _piece);

                region.gameObject.SetActive(true);
            }
        }

        if (_piece.Data.Regions.Count > 1)
        {
            transform.GetChild(0).localPosition = Vector2.zero;

            //if (_dominoRegion.Count == 2)
            //{
            //    transform.GetChild(1).localPosition = new Vector2(+GridManager.Instance.CellSize, 0);
            //}
        }

    }
    public Vector2 GetCenter()
    {

        // pas besoin de tourner une piece de 1
        if (_piece.Data.Regions.Count != 2) return transform.position;

        Vector2 _currentCenter = transform.position;

        switch (_piece.Rotation)
        {
            case 0:
                _currentCenter = new Vector2(transform.position.x, transform.position.y + GridManager.Instance.CellSize / 2);
                break;
            case 1:
                _currentCenter = new Vector2(transform.position.x + GridManager.Instance.CellSize / 2, transform.position.y);
                break;
            case 2:
                _currentCenter = new Vector2(transform.position.x, transform.position.y - GridManager.Instance.CellSize / 2);
                break;
            case 3:
                _currentCenter = new Vector2(transform.position.x - GridManager.Instance.CellSize / 2, transform.position.y);
                break;
            default:
                break;


        }
        return _currentCenter;
    }
    public void MoveOnX()
    {

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 center = GetCenter();

        if (GridManager.Instance == null) return;

        // on bouge à droite
        if (mousePos.x > center.x + 0.3f)
        {
            // ca c'est plus proche du point de doite que la position on le met a droite
            if (Mathf.Abs(mousePos.x - (center.x + GridManager.Instance.CellSize)) < Mathf.Abs(mousePos.x - center.x))
            {
                float ClampedX = Mathf.Clamp(transform.position.x + GridManager.Instance.CellSize, GridManager.Instance.Origin.position.x, GridManager.Instance.Origin.position.x + (GridManager.Instance.CellSize * (GridManager.Instance.Column - 1)));

                if (_piece.Rotation == 1)
                {
                    ClampedX = Mathf.Clamp(transform.position.x + GridManager.Instance.CellSize, GridManager.Instance.Origin.position.x, GridManager.Instance.Origin.position.x + (GridManager.Instance.CellSize * (GridManager.Instance.Column - 2)));
                }

                // On vérifie si la position est bonne
                Vector2 targetPos = new Vector2(ClampedX, transform.position.y);

                if (IsPositionValid(targetPos))
                    transform.position = targetPos;



                return;
            }
        }
        // on bouge à gauche
        else if (mousePos.x < center.x - 0.3f)
        {
            // ca c'est plus proche du point de doite que la position on le met a droite
            if (Mathf.Abs(mousePos.x - (center.x - GridManager.Instance.CellSize)) < Mathf.Abs(mousePos.x - center.x))
            {
                float ClampedX = Mathf.Clamp(transform.position.x - GridManager.Instance.CellSize, GridManager.Instance.Origin.position.x, GridManager.Instance.Origin.position.x + (GridManager.Instance.CellSize *( GridManager.Instance.Column - 1)));

                if (_piece.Rotation == 3)
                {
                    ClampedX = Mathf.Clamp(transform.position.x - GridManager.Instance.CellSize, GridManager.Instance.Origin.position.x + GridManager.Instance.CellSize, GridManager.Instance.Origin.position.x + (GridManager.Instance.CellSize * (GridManager.Instance.Column - 1)));
                }

                // On vérifie si la position est bonne
                Vector2 targetPos = new Vector2(ClampedX, transform.position.y);

                if (IsPositionValid(targetPos))
                    transform.position = targetPos;

                return;
            }
        }
    }

    private bool IsPositionValid(Vector2 position)
    {
        if (_piece.transform.GetChild(0).gameObject.activeSelf)
        {
            if (GridManager.Instance.GetRegionAtIndex(GridManager.Instance.GetIndexFromPosition(position)) != null)
                return false;
        }
        if (_piece.transform.GetChild(1).gameObject.activeSelf)
        {
            // calcule de position de la region selon la targetPos
            Vector2 RegionPosSimulation = position + (Vector2)_piece.transform.GetChild(1).transform.localPosition;

            if (GridManager.Instance.GetRegionAtIndex(GridManager.Instance.GetIndexFromPosition(RegionPosSimulation)) != null)
                return false;
        }

        return true;
        
    }

}
