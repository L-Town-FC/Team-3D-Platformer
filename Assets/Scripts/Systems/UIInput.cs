using UnityEngine;

public class UIInput : MonoBehaviour
{
    public PlayerControls Controls { get; private set; }

    private void Awake()
    {
    }

    private void OnEnable()
    {
        Controls = new PlayerControls();
        Controls.Enable();
    }

    private void OnDisable()
    {
        Controls.Disable();
    }
}
