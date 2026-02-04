using UnityEngine;

public class RegionPiece : MonoBehaviour
{
    private int _dominoUniqueId;
    public int PieceUniqueId { get => _dominoUniqueId; }

    private RegionData _regionData;
    public RegionData Region => _regionData;


    [SerializeField] private SpriteRenderer _terrainRenderer;
    [SerializeField] private SpriteRenderer _overlayRenderer;

    public void Init(RegionData data)
    {
        _regionData = data;

        _terrainRenderer.sprite = data.RegionTerrain;
        // _overlayRenderer.sprite = data.RegionOverlay;

        if (_terrainRenderer != null)
            _terrainRenderer.sortingOrder = 1;

        if (_overlayRenderer != null)
            _overlayRenderer.sortingOrder = 2;

        GeneralVisualController.Instance.FitSpriteInCell(_terrainRenderer);
        GeneralVisualController.Instance.FitSpriteInCell(_overlayRenderer);
    }

    



}
