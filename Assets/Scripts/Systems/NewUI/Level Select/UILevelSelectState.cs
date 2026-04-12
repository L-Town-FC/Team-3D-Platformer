using UnityEngine;

public class UILevelSelectState : UIBaseState
{
    protected override BaseMenu UIMenu { get; set; }

    public override void EnterState(UIStateManager ui)
    {
        stateEnterTime = Time.time;
        UIMenu = ui.GetComponentInChildren<LevelSelectMenu>(true);
        UIMenu.enabled = true;
        ui.GetMenu(UIMenu);

        Debug.Log("Active Menu: " + ui.activeMenu.name);
        Debug.Log("menuOptions: " + ui.menuOptions.name);
        Debug.Log("highlighted: " + ui.currentlyHighlightedField.name);

    }

    public override void ExitState(UIStateManager ui)
    {
    }

    public override void UpdateState(UIStateManager ui)
    {
        if (ui.uiInputActions.Navigate.WasPressedThisFrame())
        {
            ui.NavigateMenu((int)ui.uiInputActions.Navigate.ReadValue<Vector2>().y);
        }

        if (ui.uiInputActions.Submit.WasPressedThisFrame())
        {
            if (UIMenu.Submit(ui.currentlyHighlightedField.name))
            {
                ui.ChangeState(ui.UIPauseState);
            }
        }
    }
}
