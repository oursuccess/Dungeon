using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//加入了Find的能力和移动动画
public abstract class MoveCharacter : MoveCreature
{
    #region AnimatorVar
    protected Animator animator;
    #endregion
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        sight *= sight;

        base.Start();
    }
    #region MoveInterface
    public override void StopMove(float stopTime = 0)
    {
        StopMoveAnimation();

        base.StopMove(stopTime);
    }
    protected virtual void StopMoveAnimation()
    {
        animator.SetBool("Running", false);
        animator.SetFloat("Horizontal", 0f);
        animator.SetFloat("Vertical", 0f);
    }
    protected override void Dead()
    {
        StopMove();

        animator.SetBool("Dead", true);
        base.Dead();
    }
    public virtual void MoveWithAnim(Vector2 direction, float distance, float velocity)
    {
        Move(direction, distance, velocity);
        PlayMoveAnim(direction, velocity);
    }
    public virtual void MoveWithAnim(Vector2 targetPosition, float moveTime)
    {
        float distance = (targetPosition - (Vector2)transform.position).sqrMagnitude;
        float velocity = distance / moveTime;
        Vector2 direction = targetPosition - (Vector2)transform.position;
        direction.Normalize();
        MoveWithAnim(direction, distance, velocity);
    }
    #endregion
    #region Animator
    protected virtual void PlayMoveAnim(Vector2 direction, float velocity)
    {
        if(direction != Vector2.zero)
        {
            animator.SetBool("Running", true);
            animator.SetFloat("Horizontal", moveDir.x);
            animator.SetFloat("Vertical", moveDir.y);
        }
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
    protected virtual IEnumerator WaitToChangePositionWhenInCollision(float waitTime)
    {
        float stateChangedTime = 0;
        Vector3 originPosition = transform.position;
        while(stateChangedTime <= waitTime)
        {
            stateChangedTime += Time.deltaTime;
            yield return null;
        }
        if (isInCollision)
        {
            FixPosition();
        }
    }
    protected virtual void FixPosition()
    {
        while (isInCollision)
        {
            ForceMove(Vector2.up, 1f, 0.1f);
        }
    }
    #region Collider
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
    #endregion
}
