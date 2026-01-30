/// <summary>
/// Définis les données d'un domino
/// </summary>

using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New Domino Data", menuName = "Domino/DominoData")]
public class DominoData : ScriptableObject
{
    [BoxGroup("Sprites"), ShowAssetPreview] [InfoBox("Tout les sprites possibles pour les faces du domino", EInfoBoxType.Normal)]
    [SerializeField] private Sprite[] dominoSprites;

    public Sprite GetRandomSprite() => dominoSprites[Random.Range(0, dominoSprites.Length)];

    #region Getter
    public Sprite[] DominoSprites => dominoSprites;
    #endregion
}