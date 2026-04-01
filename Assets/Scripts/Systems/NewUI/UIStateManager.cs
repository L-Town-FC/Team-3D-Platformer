using UnityEngine;

[RequireComponent(typeof(UIInput))]
public class UIStateManager : MonoBehaviour
{
    // A public static property to access the single instance of the class
    public static UIStateManager Instance { get; private set; }
    [HideInInspector]
    public UIInput UIInput;

    public UIBaseState currentUIState;
    public UIBaseState lastUIState; //need to keep track of last state so back button works more easily

    Transform allMenus; //holds all possible UI menus in scene
    Transform activeMenu; //the menu that the player is currently navigating
    Transform currentlyHighlightedField; //the active menu's child that is currently being highlighted by the player
    int currentMenuIndex; //the index of the highlighted field of the active menu

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If another instance exists, destroy this one
            Destroy(this.gameObject);
        }
        else
        {
            // Otherwise, set this as the instance
            Instance = this;
            // Optional: Keep the object alive when loading new scenes
            DontDestroyOnLoad(this.gameObject);
        }

        UIInput = GetComponent<UIInput>();
    }

    private void Update()
    {
        currentUIState.UpdateState(this);
    }

    private void ChangeState(UIBaseState _newState)
    {
        lastUIState = currentUIState; //need to carve out exception between pause/unpause
        //this was made because there is a main menu settings and a pause menu settings and hitting back should send you
        //to the appropriate one. May just simplify and have two different settings that have the same functionality

        currentUIState.ExitState(this);
        currentUIState = _newState;
        currentUIState.EnterState(this);
    }

    //navigate a menu by cycling through the child object of the active menu
    public void NavigateMenu()
    {
        int navigateDir = (int)UIInput.Controls.UI.Navigate.ReadValue<Vector2>().y;

        int activeMenuChildCount = activeMenu.childCount;

        currentMenuIndex += navigateDir;

        if(currentMenuIndex > activeMenuChildCount - 1)
        {
            currentMenuIndex = 0;
        }else if(currentMenuIndex < 0)
        {
            currentMenuIndex = activeMenuChildCount - 1;
        }
    }

    public void EnableMenu()
    {
        //disables all child objects except the currently selected menu
        for (int i = 0; i < activeMenu.childCount; i++)
        {
            activeMenu.GetChild(i).gameObject.SetActive(i == currentMenuIndex);
        }
    }

    public void SelectOption()
    {

    }
}
