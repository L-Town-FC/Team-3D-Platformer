using UnityEngine;

public class DoubleJumpPowerUp : BasePlayerPowerUp
{
    protected override void OnCollect()
    {
        UpdatePlayerPrefs(PowerUpList.DoubleJump);
        base.OnCollect();
    }
}
