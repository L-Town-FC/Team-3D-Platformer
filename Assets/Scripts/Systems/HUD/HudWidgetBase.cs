using UnityEngine;

public abstract class HudWidgetBase : MonoBehaviour
{
    protected virtual void Awake() { }

    protected virtual void OnEnable()
    {
        Subscribe();
        Refresh();
    }

    protected virtual void OnDisable()
    {
        Unsubscribe();
    }

    protected abstract void Subscribe();
    protected abstract void Unsubscribe();

    // Force a redraw (useful on enable, scene load, etc.)
    public abstract void Refresh();
}
