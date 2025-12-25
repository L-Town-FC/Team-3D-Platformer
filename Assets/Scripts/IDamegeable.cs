using UnityEngine;

public interface IDamegeable
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
