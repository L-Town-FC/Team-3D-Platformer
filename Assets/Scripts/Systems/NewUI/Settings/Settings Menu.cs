using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : BaseMenu
{
    enum UISlider { xsens, ysens, volume}
    List<Transform> sliders = new List<Transform>();

    private void Awake()
    {
        SetPlayerPrefs("xsens", 5);
        SetPlayerPrefs("ysens", 5);
        SetPlayerPrefs("volume", 5);

        //sets the initial slider values to match the player prefs and also adds them to slider list
        sliders.Add(SetInitialSlider(((int)UISlider.xsens), "xsens"));
        sliders.Add(SetInitialSlider(((int)UISlider.ysens), "ysens"));
        sliders.Add(SetInitialSlider(((int)UISlider.volume), "volume"));

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

    Transform SetInitialSlider(int sliderIndex, string playerPrefName)
    {
        //grabs slider transform by index
        //if the order of the children changes this will break
        Transform sliderTransform = transform.GetChild(1).GetChild(sliderIndex);

        //sets sliders values according to their player pref value
        sliderTransform.GetChild(0).GetComponent<Slider>().value = PlayerPrefs.GetInt(playerPrefName);

        UpdateSliderAndPlayerPrefValue(sliderTransform.GetChild(0).GetComponent<Slider>(), playerPrefName, PlayerPrefs.GetInt(playerPrefName));
        return sliderTransform;
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

        //increase the slider value if right, decrease if left
        if (isIncrease)
        {
            playerPrefValue++;
        }
        else
        {
            playerPrefValue--;
        }

        //make sure value is between 0 and 10
        playerPrefValue = Mathf.Clamp(playerPrefValue, 0, 10);

        UpdateSliderAndPlayerPrefValue(slider, playerPrefName, playerPrefValue);
    }

    void UpdateSliderAndPlayerPrefValue(Slider slider, string playerPrefName, int playerPrefValue)
    {
        //set sliders value
        slider.value = playerPrefValue;
        //update text next to slider to match slider value
        slider.transform.parent.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerPrefValue.ToString();
        //update player pref value
        PlayerPrefs.SetInt(playerPrefName, playerPrefValue);
    }
}
