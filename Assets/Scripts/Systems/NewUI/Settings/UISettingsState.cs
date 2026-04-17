using UnityEngine;

public class UISettingsState : UIBaseState
{
    protected override BaseMenu UIMenu { get; set; }

    public override void EnterState(UIStateManager ui)
    {
        stateEnterTime = Time.time;
        UIMenu = ui.GetComponentInChildren<SettingsMenu>(true);

        //grabs menu transform associated with this script, enables it, while disabling all other menus
        ui.GetMenu(UIMenu);

        //highlights top field in menu
        ui.HighlightActiveField(ui, ui.currentlyHighlightedField);
    }

    public override void ExitState(UIStateManager ui)
    {
    }

    public override void UpdateState(UIStateManager ui)
    {
        
        if (ui.uiInputActions.Navigate.WasPressedThisFrame())
        {
            Vector2 navigateVector = ui.uiInputActions.Navigate.ReadValue<Vector2>().normalized;

            if(Mathf.Abs(navigateVector.y) >= 0.5f)
            {
                ui.NavigateMenu((int)(navigateVector.y));
                ui.HighlightActiveField(ui, ui.currentlyHighlightedField);
                return;
            }

            UIMenu.GetComponent<SettingsMenu>().UpdateSlider(ui.currentlyHighlightedFieldIndex, navigateVector.x > 0f);
            return;
        }
    }
}
