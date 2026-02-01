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

        FitSprite(_terrainRenderer);
        FitSprite(_overlayRenderer);
    }

    void FitSprite(SpriteRenderer renderer)
    {
        if (renderer == null || GridManager.Instance == null) return;


        // Taille réelle du sprite avant scale
        Vector2 spriteSize = renderer.sprite.bounds.size;

        // on calcule le scale qu'il faut seulement en largeur car le bas a le truc profondeur qui depasse
        float scale = GridManager.Instance.CellSize / spriteSize.x;

        transform.localScale = new Vector3(scale, scale, 1f);

        // Décalage vertical pour que la zone carré soit dans la cellule (sans profondeur)
        float offsetY = (GridManager.Instance.TileDepth * scale) / 2f;

        transform.localPosition = new Vector3(0, offsetY, 0);

    }



}
