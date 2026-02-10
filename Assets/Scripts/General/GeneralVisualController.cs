using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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


        Debug.Log("Sprite height = " + spriteSize.y);
        Debug.Log("TileDepth = " + GridManager.Instance.TileDepth);

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

    public Vector2 GetCenterPosition(List<RegionPiece> allPieces)
    {
        Vector2 center = Vector2.zero;

        foreach (RegionPiece piece in allPieces)
        {
            center += (Vector2)piece.transform.position;
        }

        return center / allPieces.Count;
    }


    public void FallAtoB(Transform transform, float duration, Vector2 startingPos, Vector2 destination)
    {
        StartCoroutine(LerpPosition(transform, duration, startingPos, destination));
    }

    private IEnumerator LerpPosition(Transform transform, float duration, Vector2 startingPos, Vector2 destination)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.position = Vector2.Lerp(startingPos, destination, easeInExpo(t));
            yield return null;
        }

        transform.position = destination;
    }


    private float easeInExpo(float x)
    {
        return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
    }
}
