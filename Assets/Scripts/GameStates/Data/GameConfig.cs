using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Game Config", fileName = "New Game Config")]
public class GameConfig : ScriptableObject
{
    [SerializeField, Foldout ("Rounds")] private int roundsBeforeShop = 2;
    [SerializeField, Foldout("Rounds")]  private int roundsBeforeBoss = 3;
    public int RoundsBeforeShop => roundsBeforeShop;
    public int RoundsBeforeBoss => roundsBeforeBoss;

    [SerializeField, Foldout("Debug"), ReadOnly] private bool loopAfterBoss = true;
    public bool LoopAfterBoss => loopAfterBoss;
}