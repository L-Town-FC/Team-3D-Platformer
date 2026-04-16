using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : BaseMenu
{
    Transform xSensObject;
    Transform ySensObject;
    Transform volumeObject;

    List<Transform> sliders = new List<Transform>();

    private void Awake()
    {
        SetPlayerPrefs("xsens", 5);
        SetPlayerPrefs("ysens", 5);
        SetPlayerPrefs("volume", 5);

        Debug.Log(transform.GetChild(1).name);
        Debug.Log(transform.GetChild(1).GetChild(0).name);
        xSensObject = transform.GetChild(1).GetChild(0);
        sliders.Add(xSensObject);

        ySensObject = transform.GetChild(1).GetChild(1).transform;
        sliders.Add(ySensObject);

        volumeObject = transform.GetChild(1).GetChild(2).transform;
        sliders.Add(volumeObject);
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    void SetPlayerPrefs(string playerPrefName, int playerPrefValue)
    {
        if (PlayerPrefs.HasKey(playerPrefName))
        {
            return;
        }

        PlayerPrefs.SetInt(playerPrefName, playerPrefValue);
    }

    public void UpdateSlider(int sliderIndex, bool isIncrease)
    {
        string playerPrefName;
        int playerPrefValue;
        Slider slider;

        switch (sliderIndex)
        {
            case 0:
                playerPrefName = "xsens";
                break;
            case 1:
                playerPrefName = "ysens";
                break;
            case 2:
                playerPrefName = "volume";
                break;
            default:
                playerPrefName = null;
                break;
        }


        playerPrefValue = PlayerPrefs.GetInt(playerPrefName);
        slider = sliders[sliderIndex].GetChild(0).GetComponent<Slider>();

        if (isIncrease)
        {
            playerPrefValue++;
        }
        else
        {
            playerPrefValue--;
        }

        playerPrefValue = Mathf.Clamp(playerPrefValue, 0, 10);
        slider.value = playerPrefValue;
        slider.transform.parent.GetChild(1).GetComponent<TMP_Text>().text = playerPrefValue.ToString();
        
    }
}
