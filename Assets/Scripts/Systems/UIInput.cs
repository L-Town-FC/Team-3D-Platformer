using UnityEngine;

public class UIInput : MonoBehaviour
{
    public static UIInput Instance { get; private set; }

    public PlayerControls Controls { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

        var go = new GameObject("UIInput");
        go.AddComponent<UIInput>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Controls = new PlayerControls();
        Controls.Enable();
    }
}
