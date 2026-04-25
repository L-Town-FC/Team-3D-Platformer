using UnityEngine;

public class DoubleJumpPowerUp : BasePlayerPowerUp
{
    protected override string powerUpName => PowerUpList.DoubleJump;

    protected override void OnCollect()
    {
        UpdatePlayerPrefs(PowerUpList.DoubleJump);
        base.OnCollect();
    }
}
