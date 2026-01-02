using UnityEngine;

public interface IDamageable
{
    //hold health for entity
    float health { get; set; }

    //mex health for entity
    float maxHealth { get; set; }

    //trigger on entity damage
    void TakeDamage(float damageAmount);

    //trigger on entity death
    void Die();

}
