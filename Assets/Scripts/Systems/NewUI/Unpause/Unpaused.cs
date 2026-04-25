using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class Unpaused : BaseMenu
{
    bool receivedPowerUp = false;
    float powerUpDisplayLength = 4f;
    float powerUpDisplayStartTime;
    string powerUpName;
    [SerializeField]
    GameObject powerUpDisplay;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        powerUpDisplay.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(receivedPowerUp)
        {
            PowerUpCheck(powerUpName);
        }
    }

    void PowerUpCheck(string powerUpName)
    {
        if(powerUpDisplayStartTime + powerUpDisplayLength < Time.time)
        {
            receivedPowerUp = false;
            powerUpDisplay.SetActive(false);
            return;
        }

        if (!powerUpDisplay.activeInHierarchy) {
            powerUpDisplay.GetComponent<TextMeshProUGUI>().text = powerUpName + " Received";
            powerUpDisplay.SetActive(true);
        }
    }

    private void OnEnable()
    {
        BasePlayerPowerUp.collectedPowerUp += EnablePowerUpScreen;
    }

    private void OnDisable()
    {
        BasePlayerPowerUp.collectedPowerUp -= EnablePowerUpScreen;
    }

    void EnablePowerUpScreen(string _powerUpName)
    {
        powerUpName = _powerUpName;
        receivedPowerUp = true;
        powerUpDisplayStartTime = Time.time;
    }
}
