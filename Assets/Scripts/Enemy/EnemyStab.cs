using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStab : Enemy, IHandlePlayerSought
{
    protected override void Start()
    {
        base.Start();
    }

    public override void Effect(Player player)
    {
        player.Dead();
    }

    public override void OnPlayerHit(Player player)
    {
        Effect(player);
    }

    public void OnPlayerSought(Player player)
    {
    }
}
