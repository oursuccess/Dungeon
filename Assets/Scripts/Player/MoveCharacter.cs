﻿using System;
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
    public delegate void StateChangedDel(MoveCharacter character);
    public event StateChangedDel OnStateChanged;
    #endregion
    #endregion
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        sight *= sight;
        OnStateChanged += OnStateChange;

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
        OnStateChanged -= OnStateChange;
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
        switch (prevState)
        {
            case MoveState.Fall:
                fallDistance = 0;
                break;
        }
    }
    public virtual void ChangeState(MoveState stateFirst, MoveState stateLast, float changeTime)
    {
        ChangeState(stateFirst);
        StartCoroutine(WaitToChangeState(stateLast, changeTime));
    }
    protected virtual IEnumerator WaitToChangeState(MoveState state, float time)
    {
        float t = 0f;
        while(t < time) 
        {
            t += Time.deltaTime;
            yield return null;
        }
        ChangeState(state);
        yield return true;
    }
    public virtual void OnStateChange(MoveCharacter character)
    {
        switch (currState)
        {
            case MoveState.Move:
            case MoveState.Crawl:
            case MoveState.Run:
            case MoveState.Fall:
                {
                    StartCoroutine(WaitToChangePositionWhenInCollision(1f));
                }
                break;
            case MoveState.Idle:
                {
                    StartCoroutine(WaitToChangePositionWhenInCollision(3f));
                }
                break;
        }
    }
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
    #endregion
    #region Collider
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
    #endregion
}
