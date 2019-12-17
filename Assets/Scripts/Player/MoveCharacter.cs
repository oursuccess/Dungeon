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
    #region MoveState
    public enum MoveState
    {
        Idle,
        Move,
        Run,
        Jump,
        Crawl,
        Fall,
        FindThing,
        Attack,
        Die,
    }
    public MoveState prevState { get; protected set; }
    public MoveState currState { get; protected set; }
    #endregion
    #endregion
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        sight *= sight;
        
        base.Start();
    }
    #region Old
    #region AutoMove(Add Animator)
    protected override void SimpleAutoMove(Vector2 direction)
    {
        animator.SetBool("Running", true);
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        base.SimpleAutoMove(direction);
    }
    #endregion
    #endregion
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
    public virtual void MoveWithAnim(Vector2 direction, float velocity)
    {
        Move(direction);
        PlayMoveAnim(direction, velocity);
    }
    #endregion
    #region Animator
    private void PlayMoveAnim(Vector2 direction, float velocity)
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
    #region State(Add Animator)
    public virtual void ChangeState(MoveState newState)
    {
        if (currState == newState) return;
        prevState = currState;
        currState = newState;
        switch (currState)
        {
            case MoveState.Run:
            case MoveState.Move:
                PlayMoveAnim(moveDir, velocity);
                break;
            case MoveState.Idle:
                animator.SetBool("Running", false);
                break;
            case MoveState.Attack:
                animator.SetTrigger("Attack");
                break;
            case MoveState.Die:
                Dead();
                break;
            case MoveState.FindThing:
                break;
            case MoveState.Fall:
                animator.SetBool("Running", false);
                break;
            default:
                break;
        }
    }
    #endregion
    #region Collider
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
    #endregion
}
