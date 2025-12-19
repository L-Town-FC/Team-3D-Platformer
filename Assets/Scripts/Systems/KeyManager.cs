using System;
using UnityEngine;

/// <summary>
/// Central manager that tracks how many keys have been collected in the level.
/// When all keys are collected, it activates the ScenePortal.
/// 
/// Also exposes an event so HUD widgets can update without polling.
/// </summary>
public class KeyManager : MonoBehaviour
{
    // Simple singleton for easy access (KeyPickup, HUD, etc.).
    public static KeyManager Instance { get; private set; }

    [Header("Keys")]
    [SerializeField] private int requiredKeys = 3; // Total keys needed to activate the portal.

    [Header("Portal")]
    [SerializeField] private ScenePortal portal;   // Portal to activate when requirements are met.

    // Public read-only accessors for other systems (HUD, etc.).
    public int RequiredKeys => requiredKeys;
    public int CollectedKeys { get; private set; }

    // Event raised whenever counts change: (collected, required)
    public event Action<int, int> OnKeyCountChanged;

    private void Awake()
    {
        // Enforce singleton (one KeyManager per scene).
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Optional: keep across scenes if you want persistent progression.
        // DontDestroyOnLoad( gameObject );

        // Portal starts off until we collect enough keys.
        if (portal != null)
            portal.SetActive(false);

        // Broadcast initial state so HUD can show "0 / 3" immediately.
        RaiseChanged();
    }

    /// <summary>
    /// Called by KeyPickup when the player collects a key.
    /// Increments count, updates HUD, and activates portal if complete.
    /// </summary>
    public void RegisterKeyCollected()
    {
        // Clamp to avoid going above requiredKeys (e.g., if keys overlap or duplicate triggers).
        CollectedKeys = Mathf.Clamp(CollectedKeys + 1, 0, requiredKeys);

        // Notify listeners (HUD, etc.).
        RaiseChanged();

        // If we have enough keys, activate the portal.
        if (CollectedKeys >= requiredKeys && portal != null)
            portal.SetActive(true);
    }

    /// <summary>
    /// Resets key progress and deactivates the portal again.
    /// Useful when restarting a level, respawning, etc.
    /// </summary>
    public void ResetKeys()
    {
        CollectedKeys = 0;

        if (portal != null)
            portal.SetActive(false);

        RaiseChanged();
    }

    /// <summary>
    /// Helper to raise the change event.
    /// </summary>
    private void RaiseChanged()
    {
        OnKeyCountChanged?.Invoke(CollectedKeys, requiredKeys);
    }
}
