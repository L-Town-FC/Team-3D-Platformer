using UnityEngine;

public class DestructibleMovingPlatforms : MovingPlatform, IDamageable
{
    //values are arbitrary and for testing purposes
    float _maxHealth = 100f;
    public float maxHealth { get { return _maxHealth; } set { } }
    public float health { get; set; }

    [SerializeField]
    float timeToDestroyPlatform = 10f; //seconds to destroy platform
    [SerializeField]
    float timeToRegenPlatform = 5f; //seconds to regen platform
    float regenAmount; //how fast the platform respawns
    float damageAmount; //how fast the platform dies while the player is on it
    bool isDead = true;
    bool isDying = false;
    Material material;
    ParticleSystem particles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        health = _maxHealth;
        base.Start();
        material = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        particles = transform.GetChild(0).GetComponent<ParticleSystem>();
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

        //Paltform death is triggered whenever a player touches it and cant be stopped once it starts
        //once its been touched, platform takes damage until it dies
        if (isTouchingPlayer || isDying)
        {
            TakeDamage(damageAmount * Time.deltaTime);
            isDying = true;
        }

        //when the platform is dead, it starts regening health
        if (isDead)
        {
            isDying = false;
            //stops platform from popping back into existence when player is in its collider
            if (!isTouchingPlayer)
            {
                //putting a negative numner into take damage is the same as adding health
                TakeDamage(-regenAmount * Time.deltaTime);
            }
        }

        //_Health is a  float in the platform shader script
        //it is directly linked to the platforms transparency when its alive
        //so as the platform loses health it becomes more transparent
        material.SetFloat("_Health", health);
        material.SetFloat("_ShakeSpeed", SetShakeSpeed());

        //hide and disable collision for the platform while its dead/regenerating
        base.boxCollider.enabled = !isDead;
        base.meshRenderer.enabled = !isDead;
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    float SetShakeSpeed()
    {
        float shakeStartHealth = 35f;
        float shakeSpeed = 0f;
        float shakeMultiplier = 15f;
        if(health < shakeStartHealth)
        {
            shakeSpeed = Mathf.InverseLerp(shakeStartHealth, 0f, health) * shakeMultiplier;
        }
        return shakeSpeed;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);

        //Stops a platform from triggering its death effects multiple times
        if(health == 0f && !isDead)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;

        //basic explosion effects when platform is destroyed
        particles.Play();
    }
}
