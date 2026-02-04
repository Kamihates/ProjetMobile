using UnityEngine;

public class GeneralVisualController : MonoBehaviour
{
    public static GeneralVisualController Instance;
    private void Awake() { Instance = this; }

    public void FitSpriteInCell(SpriteRenderer renderer)
    {
        if (renderer == null || GridManager.Instance == null) return;


        // Taille réelle du sprite avant scale
        Vector2 spriteSize = renderer.sprite.bounds.size;

        // on calcule le scale qu'il faut seulement en largeur car le bas a le truc profondeur qui depasse
        float scale = GridManager.Instance.CellSize / spriteSize.x;

        renderer.transform.localScale = new Vector3(scale, scale, 1f);

        // Décalage vertical pour que la zone carré soit dans la cellule (sans profondeur)
        float offsetY = (GridManager.Instance.TileDepth * scale) / 2f;

        renderer.transform.localPosition = new Vector3(0, offsetY, 0);

        Destroy(renderer.GetComponent<BoxCollider2D>());

        renderer.gameObject.AddComponent<BoxCollider2D>();
        renderer.GetComponent<BoxCollider2D>().isTrigger = true;

    }
}
