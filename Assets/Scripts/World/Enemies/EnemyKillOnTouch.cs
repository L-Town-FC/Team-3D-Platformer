using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyKillOnTouch : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private DeathMenuController deathMenu;

    private void Awake()
    {
        deathMenu = FindFirstObjectByType<DeathMenuController>();
        if (deathMenu == null)
            Debug.LogWarning("EnemyKillOnTouch: No DeathMenuController found in scene.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag(playerTag))
            return;

        if (deathMenu != null)
            if(collision.transform.TryGetComponent<IDamegeable>(out IDamegeable _damageableInterface))
            {
                //this could also just be _damageableInterface.Die(), but wanted to test out take damage function
                _damageableInterface.TakeDamage(_damageableInterface.health);
            }
            //deathMenu.Die();
    }

    // If you decide to use Trigger instead of Collision, swap to this and set your enemy collider to Is Trigger.
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (deathMenu != null)
            deathMenu.Die();
    }
    */
}
