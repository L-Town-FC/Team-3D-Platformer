using UnityEngine;

public class DestructibleMovingPlatforms : MovingPlatform, IDamegeable
{
    //values are arbitrary and for testing purposes
    public float health { get; set; }
    float maxHealth = 100f;
    float regenAmount = 0.1f; //how fast the platform respawns
    float damageTaken = 0.2f; //how fast the platform dies while the player is on it
    bool isDead = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        health = maxHealth;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //when the platform reaches max health, re-enable it
        if(health == maxHealth)
        {
            isDead = false;
        }

        //putting a negative numner into take damage is the same as adding health
        //this is always called but since its less than the damage taken modifier, the platform will always break while the player is standing on it
        TakeDamage(-regenAmount);

        if (isTouchingPlayer)
        {
            TakeDamage(damageTaken);
        }

        //hide and disable collision for the platform while its dead/regenerating
        base.boxCollider.enabled = !isDead;
        base.meshRenderer.enabled = !isDead;
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);

        if(health == 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
    }
}
