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
            //if levelTransform is the index 0 it should always be unlocked because its the first level
            //otherwise if the level isnt in the player prefs, it should remain locked
            if (!PlayerPrefs.HasKey(levelTransform.name) && levelTransform.GetSiblingIndex() != 0)
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
