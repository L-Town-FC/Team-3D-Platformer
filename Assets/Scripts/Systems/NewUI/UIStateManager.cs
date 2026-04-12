using UnityEngine;

[RequireComponent(typeof(UIInput))]
public class UIStateManager : MonoBehaviour
{
    //TODO: Find way to change active menu to proper menu

    // A public static property to access the single instance of the class
    public static UIStateManager Instance { get; private set; }
    [HideInInspector]
    public UIInput uiInput;
    public PlayerControls.UIActions uiInputActions;

    public UIBaseState currentUIState;
    public UIBaseState lastUIState; //need to keep track of last state so back button works more easily

    public Transform activeMenu; //the top level menu that the player is currently navigating (i.e. Main menu, Level Select, etc)
    public int currentMenuIndex; //the index of the current top level menu options (i.e. Main Menu = 0, Level Select = 0, etc)
    public Transform menuOptions; //each ui menu has transform with the actual options, this is that transform
    public Transform currentlyHighlightedField; //the Transform of the currently highlted menu (i.e. The transform that is named Level1, Level2, etc)
    public int currentlyHighlightedFieldIndex; //index of transform of the currently highlighted field relative to its parent (i.e. Level1 = 0, Level2 = 1)
    enum startingState { Main, Pause }
    [SerializeField]
    startingState initialState = startingState.Main;


    #region All UI States
    public UIMainMenuState UIMainMenuState;
    public UILevelSelectState UILevelSelectState;
    public UIPauseState UIPauseState;
    public UISettingsState UISettingsState;
    public UIUnpausedState UIUnpausedState;
    #endregion

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

        activeMenu = transform.GetChild(0);
        EnableMenu();
        CreateAllUIStates();
    }

    private void Start()
    {
        uiInput = GetComponent<UIInput>();
        uiInputActions = uiInput.Controls.UI;

        currentUIState = GetInitialState(initialState);
        currentUIState.EnterState(this);
    }

    private void Update()
    {
        currentUIState.UpdateState(this);
    }

    public void ChangeState(UIBaseState _newState)
    {
        lastUIState = currentUIState; //need to carve out exception between pause/unpause
        //this was made because there is a main menu settings and a pause menu settings and hitting back should send you
        //to the appropriate one. May just simplify and have two different settings that have the same functionality

        currentUIState.ExitState(this);
        currentUIState = _newState;
        currentUIState.EnterState(this);
    }

    //navigate a menu by cycling through the child object of the active menu
    public void NavigateMenu(int navigateDir)
    {
        if(navigateDir == 0)
        {
            return;
        }

        int menuOptionsChildCount = menuOptions.childCount;

        currentlyHighlightedFieldIndex -= navigateDir;

        if (currentlyHighlightedFieldIndex > menuOptionsChildCount - 1)
        {
            currentlyHighlightedFieldIndex = 0;
        }
        else if (currentMenuIndex < 0)
        {
            currentlyHighlightedFieldIndex = menuOptionsChildCount - 1;
        }

        currentlyHighlightedField = menuOptions.GetChild(currentlyHighlightedFieldIndex);
    }


    public void SelectOption()
    {

    }

    private void CreateAllUIStates()
    {
        UIMainMenuState = new UIMainMenuState();
        UISettingsState = new UISettingsState();
        UILevelSelectState = new UILevelSelectState();
        UIPauseState = new UIPauseState();
        UIUnpausedState = new UIUnpausedState();
    }

    //Possible change this to "Get state" and have it be the function that is called whenever the UI state is changed
    private UIBaseState GetInitialState(startingState _startingState)
    {
        UIBaseState currentState;
        if (_startingState == startingState.Main)
        {
            currentState = UIMainMenuState;
        }
        else
        {
            currentState = UIPauseState;
        }

        return currentState;
    }

    public void GetMenu(BaseMenu uiMenu)
    {
        //sets the UI index tracker to the index of the specified menu
        currentMenuIndex = uiMenu.transform.GetSiblingIndex();
        activeMenu = transform.GetChild(currentMenuIndex);
        if(activeMenu.childCount > 1)
        {
            menuOptions = activeMenu.GetChild(1);
            currentlyHighlightedFieldIndex = 0;
            currentlyHighlightedField = menuOptions.GetChild(currentlyHighlightedFieldIndex);
        }
        EnableMenu();
    }
    public void EnableMenu()
    {
        //disables all child objects except the currently selected menu
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == currentMenuIndex);
        }
    }
}

