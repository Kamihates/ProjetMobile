using UnityEngine;

public class Domino : MonoBehaviour
{
    public SpriteRenderer leftRenderer;
    public SpriteRenderer rightRenderer;

    public Sprite LeftFace { get; private set; }
    public Sprite RightFace { get; private set; }
    public DominoData DominoData { get; private set; }

    public void Init(DominoData dominoData)
    {
        DominoData = dominoData;

        LeftFace = dominoData.GetRandomSprite();
        RightFace = dominoData.GetRandomSprite();

        // Assign aux enfants
        if (leftRenderer != null) leftRenderer.sprite = LeftFace;
        if (rightRenderer != null) rightRenderer.sprite = RightFace;
    }
}
