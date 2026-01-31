using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DominoPiece : MonoBehaviour
{
    private int _pieceUniqueId;
    public int PieceUniqueId { get => _pieceUniqueId; }


    private int _currentRotation = 0;
   

    List<RegionData> _dominoRegion;


    public void RotateVisual()
    {
        // pas besoin de tourner une piece de 1
        if (_dominoRegion.Count != 2) return;

        switch (_currentRotation) 
        {
            case 0:
                // on trourne vers le haut
                transform.GetChild(1).localPosition = new Vector2(0, +GridManager.Instance.CellSize);
            
                break;
            case 1:
                // on trourne vers la droite
                transform.GetChild(1).localPosition = new Vector2(+GridManager.Instance.CellSize, 0);
                break;
            case 2:
                // on trourne vers le bas
                transform.GetChild(1).localPosition = new Vector2(0, -GridManager.Instance.CellSize);
                break;
            case 3:
                // on trourne vers la gauche
                transform.GetChild(1).localPosition = new Vector2(-GridManager.Instance.CellSize, 0);
                break;
            default:
                break;

        }
    }

    public void Init(int _UniqueId, int rotation, List<RegionData> dominoRegions)
    {
        _pieceUniqueId = _UniqueId;
        _currentRotation = rotation;
        _dominoRegion = dominoRegions;

        
        UpdateVisual();
        RotateVisual();
    }

    public void Rotate()
    {
        _currentRotation++;
        if (_currentRotation > 3)
            _currentRotation = 0;

        RotateVisual();
    }

    private void UpdateVisual()
    {   
        for (int r = 0; r < _dominoRegion.Count; r++)
        {
            if (transform.GetChild(r).TryGetComponent<RegionPiece>(out RegionPiece region))
            {
                
                region.Init(_dominoRegion[r]);

                region.gameObject.SetActive(true);
            }
        }

        if (_dominoRegion.Count > 1 )
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
        if (_dominoRegion.Count != 2) return transform.position;

        Vector2 _currentCenter = transform.position;

        switch (_currentRotation)
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
}
