using UnityEngine;

public class RegionPiece : MonoBehaviour
{
    private int _dominoUniqueId;
    public int PieceUniqueId { get => _dominoUniqueId; }

    private RegionData _regionData;


    [SerializeField] private SpriteRenderer _terrainRenderer;
    [SerializeField] private SpriteRenderer _overlayRenderer;

    public void Init(RegionData data)
    {
        _regionData = data;

        _terrainRenderer.sprite = data.RegionTerrain;
        _overlayRenderer.sprite = data.RegionOverlay;

    }


   


}
