using System.Collections;
using UnityEngine;

public class AccelerationEffect : BossEffect
{

    [SerializeField] private float _effectDuration = 10;

    protected override void OnActivated()
    {
        // on accelere la chute du current domino
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.CurrentDomino != null)
            {
                Debug.Log("boss acceleration");
                GameManager.Instance.CurrentDomino.FallController.IsAccelerating = true;
                StartCoroutine(WaitForDeactivate());
            }
        }
    }

    private IEnumerator WaitForDeactivate()
    {
        yield return new WaitForSeconds(_effectDuration);
        OnDeactivated();
    }

    protected override void OnDeactivated()
    {
        GameManager.Instance.CurrentDomino.FallController.IsAccelerating = false;
    }
}
