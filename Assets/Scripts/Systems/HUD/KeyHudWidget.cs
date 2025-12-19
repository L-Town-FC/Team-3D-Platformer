using TMPro;
using UnityEngine;

/// <summary>
/// HUD widget that displays key progress (e.g., "Keys: 1 / 3").
/// 
/// This script is resilient to execution order issues:
/// If the HUD enables before KeyManager exists, it will keep trying to bind
/// until KeyManager.Instance becomes available.
/// </summary>
public class KeyHudWidget : MonoBehaviour
{
    [SerializeField] private TMP_Text keyText;                  // Text component we will update.
    [SerializeField] private string format = "Keys: {0} / {1}"; // Format: {0} = collected, {1} = required.

    private bool isBound; // True once we've subscribed to KeyManager events.

    private void Awake()
    {
        // If the TMP_Text wasn't wired in the Inspector, auto-find it on this GameObject.
        if (keyText == null)
            keyText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        // Try to subscribe immediately (may fail if KeyManager isn't alive yet).
        TryBind();

        // Draw something right away if possible.
        Refresh();
    }

    private void OnDisable()
    {
        // Always unsubscribe to avoid duplicate subscriptions / memory leaks.
        Unbind();
    }

    private void Update()
    {
        // If the HUD enabled before KeyManager existed, bind as soon as it appears.
        if (!isBound)
        {
            TryBind();

            // Once bound, force a refresh so the UI immediately reflects the real count.
            if (isBound)
                Refresh();
        }
    }

    /// <summary>
    /// Subscribes to KeyManager's change event (if not already bound).
    /// Safe to call multiple times.
    /// </summary>
    private void TryBind()
    {
        // Already subscribed.
        if (isBound)
            return;

        // KeyManager not present yet (e.g., HUD enabled first).
        if (KeyManager.Instance == null)
            return;

        // Subscribe to updates.
        KeyManager.Instance.OnKeyCountChanged += HandleKeyCountChanged;
        isBound = true;
    }

    /// <summary>
    /// Unsubscribes from KeyManager's change event (if currently bound).
    /// </summary>
    private void Unbind()
    {
        if (!isBound)
            return;

        // KeyManager may have been destroyed during scene transitions; guard it.
        if (KeyManager.Instance != null)
            KeyManager.Instance.OnKeyCountChanged -= HandleKeyCountChanged;

        isBound = false;
    }

    /// <summary>
    /// Forces the widget to redraw using the latest values from KeyManager.
    /// </summary>
    private void Refresh()
    {
        // If the TMP_Text is missing, we can't render.
        if (keyText == null)
            return;

        // If KeyManager doesn't exist yet, we can't read values.
        if (KeyManager.Instance == null)
            return;

        // Reuse the same render path as event-driven updates.
        HandleKeyCountChanged(KeyManager.Instance.CollectedKeys, KeyManager.Instance.RequiredKeys);
    }

    /// <summary>
    /// Event handler invoked by KeyManager whenever key counts change.
    /// </summary>
    private void HandleKeyCountChanged(int collected, int required)
    {
        if (keyText == null)
            return;

        keyText.text = string.Format(format, collected, required);
    }
}
