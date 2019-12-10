using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVi : Enemy, IHandlePlayerSought
{
    private Animator animator;
    protected override void Start()
    {
        animator = transform.GetComponent<Animator>();

        base.Start();
    }

    public override void Effect(Player player)
    {
        animator.SetTrigger("Attack");

        player.Dead();
    }

    public override void OnPlayerHit(Player player)
    {
        Effect(player);
    }

    public void OnPlayerSought(Player player)
    {
        animator.SetBool("Idle", true);
    }

}
