using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Game Config", fileName = "New Game Config")]
public class GameConfig : ScriptableObject
{
    //[SerializeField, Foldout ("Rounds")] private int roundsBeforeShop = 2;
    [SerializeField, Foldout("Rounds")]  private int roundsBeforeBoss = 1;
    //public int RoundsBeforeShop => roundsBeforeShop;
    public int RoundsBeforeBoss => roundsBeforeBoss;

    [SerializeField, Foldout("Debug")] private bool loopAfterBoss = false;
    public bool LoopAfterBoss {get => loopAfterBoss; set => loopAfterBoss =  value;}
}