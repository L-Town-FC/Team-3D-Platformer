using UnityEngine;

public class SuperJumpPowerUp : BasePlayerPowerUp
{
    protected override string powerUpName => PowerUpList.SuperJump;

    protected override void OnCollect()
    {
        UpdatePlayerPrefs(PowerUpList.SuperJump);
        base.OnCollect();
    }
}
