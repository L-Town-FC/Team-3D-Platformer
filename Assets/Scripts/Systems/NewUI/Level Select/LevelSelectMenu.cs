using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : BaseMenu
{
    [SerializeField]
    Transform allLevels;
    List<string> lockedLevels = new List<string>();

    Color unlockedColor = Color.white;
    Color lockedColor = Color.gray;

    private void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    private void OnEnable()
    {
        LockAndUnlockLevels();
    }

    void LockAndUnlockLevels()
    {
        lockedLevels.Clear();

        foreach(Transform levelTransform in allLevels)
        {
            Color newColor;
            if (!PlayerPrefs.HasKey(levelTransform.name))
            {
                newColor = lockedColor;
                lockedLevels.Add(levelTransform.name);
            }
            else
            {
                newColor = unlockedColor;
            }

            levelTransform.GetComponent<TMP_Text>().color = newColor;
        }
    }

    public override bool Submit(string levelName)
    {
        //only lets you load the unlocked levels
        if (lockedLevels.Contains(levelName))
        {
            return false;
        }
        SceneManager.LoadScene(levelName);

        return true;
    }
}
