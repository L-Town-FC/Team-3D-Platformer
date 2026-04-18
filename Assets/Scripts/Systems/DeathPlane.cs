using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.Die();
        }
        Destroy(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.Die();
            return;
        }

        Destroy(collision.collider.transform.gameObject);
    }
}
