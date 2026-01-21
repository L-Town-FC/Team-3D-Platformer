using UnityEngine;

public class SuperJumpPowerUp : BasePlayerPowerUp
{
    protected override void OnCollect()
    {
        UpdatePlayerPrefs(PowerUpList.SuperJump);
        base.OnCollect();
    }
}
