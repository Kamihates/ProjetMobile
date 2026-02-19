using UnityEngine;

public abstract class BossEffect : MonoBehaviour
{

    protected bool _isActive = false;

    public virtual void Activate()
    {
        _isActive = true;
        OnActivated();
    }

    public virtual void Deactivate()
    {
        _isActive = false;
        OnDeactivated();
    }

    protected virtual void OnActivated() { }
    protected virtual void OnDeactivated() { }
    
}
