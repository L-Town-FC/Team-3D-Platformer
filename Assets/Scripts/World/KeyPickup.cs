using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private ScenePortal portalToActivate;

    private bool collected;

    private void Awake()
    {
        // Key should be a trigger pickup.
        Collider c = GetComponent<Collider>();
        c.isTrigger = true;

        if (portalToActivate != null)
            portalToActivate.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
            return;

        if (!other.CompareTag(playerTag))
            return;

        collected = true;

        if (portalToActivate != null)
            portalToActivate.SetActive(true);

        // Disappear immediately
        Destroy(gameObject);
    }
}
