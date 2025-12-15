using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public abstract class MenuControllerBase : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] protected GameObject panel;
    [SerializeField] protected TMP_Text[] optionTexts;
    [SerializeField] protected string cursor = "> ";

    [Header("Input")]
    [SerializeField] protected bool useNavigateRepeatGuard = true;

    protected int selectedIndex;
    protected bool isOpen;

    private float lastNavigateY;

    protected virtual void Start()
    {
        SetOpen(false);
        selectedIndex = 0;
        RefreshUI();
    }

    protected virtual void OnEnable()
    {
        var ui = UIInput.Instance;
        if (ui == null)
            return;

        ui.Controls.UI.Submit.performed += OnSubmit;
        ui.Controls.UI.Cancel.performed += OnCancel;
    }

    protected virtual void OnDisable()
    {
        var ui = UIInput.Instance;
        if (ui == null)
            return;

        ui.Controls.UI.Submit.performed -= OnSubmit;
        ui.Controls.UI.Cancel.performed -= OnCancel;
    }

    protected virtual void Update()
    {
        if (!isOpen)
            return;

        var ui = UIInput.Instance;
        if (ui == null)
            return;

        Vector2 nav = ui.Controls.UI.Navigate.ReadValue<Vector2>();

        // We only care about vertical navigation (W/S)
        float y = nav.y;

        if (useNavigateRepeatGuard)
        {
            // Trigger only on a fresh “press” across threshold
            if (Mathf.Abs(y) < 0.5f)
            {
                lastNavigateY = 0f;
                return;
            }

            if (lastNavigateY == 0f)
            {
                if (y < 0f) Next();
                else Prev();

                lastNavigateY = y;
            }
        }
        else
        {
            // If you want continuous scroll, remove guard and add your own cooldown timer
            if (y < -0.5f) Next();
            else if (y > 0.5f) Prev();
        }
    }

    public virtual void Open()
    {
        selectedIndex = 0;
        SetOpen(true);
        RefreshUI();
        OnOpened();
    }

    public virtual void Close()
    {
        SetOpen(false);
        OnClosed();
    }

    protected void Next()
    {
        if (optionTexts == null || optionTexts.Length == 0)
            return;

        selectedIndex = (selectedIndex + 1) % optionTexts.Length;
        RefreshUI();
    }

    protected void Prev()
    {
        if (optionTexts == null || optionTexts.Length == 0)
            return;

        selectedIndex = (selectedIndex - 1 + optionTexts.Length) % optionTexts.Length;
        RefreshUI();
    }

    protected void RefreshUI()
    {
        if (optionTexts == null || optionTexts.Length == 0)
            return;

        for (int i = 0; i < optionTexts.Length; i++)
        {
            if (optionTexts[i] == null)
                continue;

            string label = GetLabelForIndex(i);
            optionTexts[i].text = (i == selectedIndex ? cursor : "  ") + label;
        }
    }

    protected virtual string GetLabelForIndex(int index)
    {
        // Default: keep whatever text is already on the TMP object (minus any cursor)
        // Override if you want labels enforced.
        string raw = optionTexts[index].text;
        if (raw.StartsWith(cursor)) raw = raw.Substring(cursor.Length);
        if (raw.StartsWith("  ")) raw = raw.Substring(2);
        return raw;
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (!isOpen)
            return;

        OnSelect(selectedIndex);
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!isOpen)
            return;

        if (CanCancel())
            OnCancelPressed();
    }

    protected virtual bool CanCancel() => false;

    protected virtual void OnCancelPressed() { }

    protected abstract void OnSelect(int index);

    protected virtual void OnOpened() { }
    protected virtual void OnClosed() { }

    protected void SetOpen(bool open)
    {
        isOpen = open;

        if (panel != null)
            panel.SetActive(open);
    }
}
