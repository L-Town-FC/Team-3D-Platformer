using UnityEngine;

public class BasePlayerPowerUp : MonoBehaviour
{
    protected virtual bool isPowerUpEnabled { get; set; }
    protected virtual string powerUpName { get; set; }

    string playerTag = "Player";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void OnCollect()
    {
        //power up collection effect
        //time stop?
        Destroy(this.gameObject);
    }

    protected void UpdatePlayerPrefs(string powerupName)
    {
        //0 = does not have power up, 1 = has power up
        PlayerPrefs.SetInt(powerupName, 1);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)){
            return;
        }

        OnCollect();
    }
}
