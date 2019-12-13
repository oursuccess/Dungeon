using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapStab : Trap, IHandlePlayerSought
{
    protected override void Start()
    {
        transform.position = new Vector2(transform.position.x, Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + 0.7f);
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

    public void OnPlayerNotSeeing(Player player)
    {
    }
}
