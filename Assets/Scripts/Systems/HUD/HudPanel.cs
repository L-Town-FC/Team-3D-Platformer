using UnityEngine;

/// <summary>
/// Optional convenience component:
/// Finds all HUD widgets under this object and can force them all to redraw.
/// Useful when you want to refresh the entire HUD after a scene load, state reset, etc.
/// </summary>
public class HudPanel : MonoBehaviour
{
    // Cache of widgets so we don't have to search the hierarchy every refresh.
    private HudWidgetBase[] widgets;

    private void Awake()
    {
        // Grab all widgets under this panel (including inactive objects).
        // includeInactive is helpful because some HUD elements might be disabled/enabled at runtime.
        widgets = GetComponentsInChildren<HudWidgetBase>(includeInactive: true);
    }

    /// <summary>
    /// Forces every child HUD widget to redraw its UI.
    /// </summary>
    public void RefreshAll()
    {
        // If this panel was instantiated or re-parented after Awake,
        // widgets might be null. Re-fetch defensively.
        if (widgets == null)
            widgets = GetComponentsInChildren<HudWidgetBase>(includeInactive: true);

        // Tell each widget to redraw itself.
        for (int i = 0; i < widgets.Length; i++)
            widgets[i].Refresh();
    }
}
