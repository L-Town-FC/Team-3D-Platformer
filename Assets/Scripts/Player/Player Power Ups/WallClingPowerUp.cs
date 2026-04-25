using UnityEngine;

public class WallClingPowerUp : BasePlayerPowerUp
{
    protected override string powerUpName => PowerUpList.WallCling;
    protected override void OnCollect()
    {
        UpdatePlayerPrefs(PowerUpList.WallCling);
        base.OnCollect();
    }
}
