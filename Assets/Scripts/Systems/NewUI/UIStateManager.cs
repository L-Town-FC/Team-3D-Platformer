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

    Transform activeMenu; //the menu that the player is currently navigating
    Transform currentlyHighlightedField; //the active menu's child that is currently being highlighted by the player
    public int currentMenuIndex; //the index of the highlighted field of the active menu

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

        int activeMenuChildCount = activeMenu.childCount;
        Debug.Log("Active Menu: " + activeMenu.name);
        Debug.Log("Navigate Dir: " + navigateDir);

        currentMenuIndex -= navigateDir;

        if (currentMenuIndex > activeMenuChildCount - 1)
        {
            currentMenuIndex = 0;
        }
        else if (currentMenuIndex < 0)
        {
            currentMenuIndex = activeMenuChildCount - 1;
        }
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
        Debug.Log(_startingState == startingState.Main);
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
        activeMenu = uiMenu.transform.GetChild(currentMenuIndex);
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

