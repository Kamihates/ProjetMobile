using UnityEngine;

public class AccelerationEffect : BossEffect
{
    public override void Activate()
    {
        // on accelere la chute du current domino
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.CurrentDomino != null)
            {
                GameManager.Instance.CurrentDomino.FallController.Init();
            }
        }
    }

    public override void Deactivate()
    {
       
    }
}
