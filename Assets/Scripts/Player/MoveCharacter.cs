using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//加入了Find的能力和移动动画
public abstract class MoveCharacter : MoveObject
{
    #region Var
    #region FindVar
    [SerializeField]
    protected float sight = 1;
    [SerializeField]
    protected LayerMask layer;
    #endregion
    #region AnimatorVar
    protected Animator animator;
    #endregion
    #endregion
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        sight *= sight;
        
        base.Start();
    }
    #region AutoMove(Add Animator)
    protected override void SimpleAutoMove(Vector2 direction)
    {
        animator.SetBool("Running", true);
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        base.SimpleAutoMove(direction);
    }
    #endregion
    #region MoveInterface
    public override void StopMove()
    {
        StopMoveAnimation();

        base.StopMove();
    }
    protected virtual void StopMoveAnimation()
    {
        animator.SetBool("Running", false);
        animator.SetFloat("Horizontal", 0f);
        animator.SetFloat("Vertical", 0f);
    }
    public virtual void Dead()
    {
        StopMove();

        animator.SetBool("Dead", true);
    }
    #endregion
    #region Find(Layer)
    protected bool Find(Vector2 direction, LayerMask layer, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        return Find(start, direction, layer,out hit);
    }
    protected bool Find(Vector2 start, Vector2 direction, LayerMask layer, out RaycastHit2D hit)
    {
        Vector2 end = start + direction * sight;
        hit = Physics2D.Linecast(transform.position, end, layer);
        if(hit.transform != null)
        {
            return true;
        }
        return false;
    }
    #endregion
}
