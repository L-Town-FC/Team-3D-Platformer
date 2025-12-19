using UnityEngine;

public class DestructibleMovingPlatforms : MovingPlatform, IDamegeable
{
    //values are arbitrary and for testing purposes
    public float health { get; set; }
    float maxHealth = 100f;
    [SerializeField]
    float timeToDestroyPlatform = 10f; //seconds to destroy platform
    [SerializeField]
    float timeToRegenPlatform = 5f; //seconds to regen platform
    float regenAmount; //how fast the platform respawns
    float damageAmount; //how fast the platform dies while the player is on it
    bool isDead = true;
    bool isDying = false;
    Material material;
    ParticleSystem particleSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        particleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
        health = maxHealth;
        damageAmount = maxHealth / timeToDestroyPlatform;
        regenAmount = maxHealth / timeToRegenPlatform;
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

        if (isTouchingPlayer || isDying)
        {
            TakeDamage(damageAmount * Time.deltaTime);
            isDying = true;
        }

        if (isDead)
        {
            isDying = false;
            if (!isTouchingPlayer)
            {
                TakeDamage(-regenAmount * Time.deltaTime);
            }
        }

        material.SetFloat("_Health", health);

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

        if(health == 0f && !isDead)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;
        particleSystem.Play();
    }
}
