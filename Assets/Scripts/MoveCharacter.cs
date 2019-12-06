using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveCharacter : MoveObject
{
    [SerializeField]
    protected float sight = 1;
    [SerializeField]
    protected LayerMask layer;
    protected Animator animator;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        
        base.Start();
    }

    protected override void SimpleAutoMove(Vector2 direction)
    {
        animator.SetBool("Moving", true);
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        base.SimpleAutoMove(direction);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void StopMove()
    {
        StopMoveAnimation();
        base.StopMove();
    }

    public virtual void Dead()
    {
        StopMove();

        animator.SetBool("Dead", true);
    }

    protected virtual void StopMoveAnimation()
    {
        animator.SetBool("Moving", false);
        animator.SetFloat("Horizontal", 0f);
        animator.SetFloat("Vertical", 0f);
    }

    protected bool Find(Vector2 direction, LayerMask layer, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + direction * sight;
        hit = Physics2D.Linecast(transform.position, end, layer);
        if(hit.transform != null)
        {
            return true;
        }
        return false;
    }

}
