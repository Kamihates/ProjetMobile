using System;
using UnityEngine;

[Serializable]
public class DominoCombination
{
    public int leftIndex;
    public int rightIndex;

    public DominoCombination(int left, int right)
    {
        leftIndex = left;
        rightIndex = right;
    }
}

public class Domino : MonoBehaviour
{
    [SerializeField] private SpriteRenderer leftRenderer;
    [SerializeField] private SpriteRenderer rightRenderer;

    public void Init(DominoCombination dominoCombination, DominoData dominoData)
    {
        if (leftRenderer != null)
            leftRenderer.sprite = dominoData.GetSprite(dominoCombination.leftIndex);
        if (rightRenderer != null)
            rightRenderer.sprite = dominoData.GetSprite(dominoCombination.rightIndex);
    }
}
