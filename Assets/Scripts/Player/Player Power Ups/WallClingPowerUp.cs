using UnityEngine;

public class WallClingPowerUp : BasePlayerPowerUp
{
    protected override void OnCollect()
    {
        UpdatePlayerPrefs(PowerUpList.WallCling);
        base.OnCollect();
    }
}
