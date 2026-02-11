using UnityEngine;

public class MiniT1VisualController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer Region1Terrain;
    [SerializeField] private SpriteRenderer Region2Terrain;

    [SerializeField] private SpriteRenderer Region1Overlay;
    [SerializeField] private SpriteRenderer Region2Overlay;

    public void Init(RegionData region)
    {
        if (Region1Terrain == null && Region2Terrain == null) return;

        Region1Terrain.sprite = region.RegionTerrain;
        Region2Terrain.sprite = region.RegionTerrain;

        if (Region1Overlay == null && Region2Overlay == null) return;

        Region1Overlay.sprite = region.RegionOverlay;
        Region2Overlay.sprite = region.RegionOverlay;
    }
}
