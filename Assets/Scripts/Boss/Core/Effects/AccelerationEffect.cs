using System.Collections;
using UnityEngine;

public class AccelerationEffect : BossEffect
{

    [SerializeField] private float _effectDuration = 10;

    private void Start()
    {
        if (GameManager.Instance != null) 
            GameManager.Instance.OnCurrentDominoChanged += UpdateDomino;
    }
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnCurrentDominoChanged -= UpdateDomino;
    }

    private void UpdateDomino()
    {
        if (_isActive)
        {
            if (GameManager.Instance.CurrentDomino != null)
                GameManager.Instance.CurrentDomino.FallController.IsAccelerating = true;
        }
    }

    protected override void OnActivated()
    {
        // on accelere la chute du current domino
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.CurrentDomino != null)
            {
                Debug.Log("boss acceleration");

                _visualController.ShowAttack("↓ Speed x2 ↓");


                GameManager.Instance.CurrentDomino.FallController.IsAccelerating = true;
                StartCoroutine(WaitForDeactivate());
            }
        }
    }

    private IEnumerator WaitForDeactivate()
    {
        yield return new WaitForSeconds(_effectDuration);
        Deactivate();
    }

    protected override void OnDeactivated()
    {
        if (GameManager.Instance.CurrentDomino != null)
            GameManager.Instance.CurrentDomino.FallController.IsAccelerating = false;
    }
}
