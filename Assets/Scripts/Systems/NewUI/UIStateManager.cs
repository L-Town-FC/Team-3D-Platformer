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
}
