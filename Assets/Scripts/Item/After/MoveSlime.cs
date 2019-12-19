using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSlime : MoveItem, IHaveTrampleEffect
{
    #region MoveState
    public class MoveState : BaseMoveState
    {
        #region StateEnum
        public const int FindPlayer = 21;
        public const int FindMetal = 22;
        public const int BeenTrampled = 23;
        public const int LosePlayer = 24;
        public const int LoseMetal = 25;
        public const int Consumed = 26;
        #endregion
        public MoveState()
        {
            currState = MoveState.Idle;
            prevState = MoveState.Idle;
        }
    }
    public override void ChangeState(int state)
    {
        if(state != moveState.currState)
        {
            moveState.ChangeState(state);
            if (state == MoveState.Die)
            {
                Dead();
            }
            else if (state == MoveState.Consumed)
            {
                //播放吞食动画
                canMove = false;
            }
            if (moveState.prevState == MoveState.Fall)
            {
                fallDistance = 0;
            }
        }
    }
    #endregion
    protected override void Start()
    {
        moveState = new MoveState();
        base.Start();
    }
    #region Move
    public override void StartMove()
    {
        base.StartMove();
    }
    public override void StopMove(float stopTime = 0)
    {
        base.StopMove(stopTime);
    }
    protected override IEnumerator MovingImpl()
    {
        Collider2D target = null;
        while (true)
        {
            if (lastMoveTime <= moveTime)
            {
                lastMoveTime += Time.deltaTime;
            }
            else
            {
                lastMoveTime = 0f;
                #region WhetherOnAir
                target = FindAnythingOnDirection(Vector2.down, 0.3f);
                if (target == null)
                {
                    fallDistance += rigidBody2D.gravityScale * moveTime;
                    ChangeState(MoveState.Fall);
                }
                #endregion
                #region WhetherBeenTrampled
                if (moveState.currState != MoveState.Fall)
                {
                    target = FindAnythingOnDirection(Vector2.up, 1.1f);
                    if (target != null)
                    {
                        ChangeState(MoveState.BeenTrampled);
                    }
                }
                #endregion
                switch (moveState.currState)
                {
                    case MoveState.Jump:
                        {
                            continue;
                        }
                    case MoveState.Idle:
                    case MoveState.Move:
                        {
                            target = FindWithEye<Player>();
                            if (target != null)
                            {
                                ChangeState(MoveState.FindPlayer);
                                yield return null;
                            }
                            else
                            {
                                target = FindWithEye<IMadeByMetal>();
                                if (target != null)
                                {
                                    ChangeState(MoveState.FindMetal);
                                    yield return null;
                                }
                                else
                                {
                                    target = FindAnythingOnDirection(moveDir, moveDistance);
                                    if (target == null)
                                    {
                                        if (UnityEngine.Random.Range(0, 100) <= moveWill)
                                        {
                                            Move();
                                            ChangeState(MoveState.Move);
                                            yield return null;
                                        }
                                        else
                                        {
                                            ChangeState(MoveState.Idle);
                                            yield return null;
                                        }
                                    }
                                }
                            }

                        }
                        break;
                    case MoveState.FindMetal:
                        {
                            MoveNChangeDirection(target.transform.position - transform.position);
                            if (FindWithEye<IMadeByMetal>() == null)
                            {
                                ChangeState(MoveState.LoseMetal);
                            }
                            yield return null;
                        }
                        break;
                    case MoveState.LoseMetal:
                        {
                            LoseMetal();
                            ChangeState(MoveState.Idle);
                            yield return null;
                        }
                        break;
                    case MoveState.FindPlayer:
                        {
                            MoveNChangeDirection(target.transform.position - transform.position);
                            if (FindWithEye<Player>() == null)
                            {
                                ChangeState(MoveState.LosePlayer);
                            }
                            yield return null;
                        }
                        break;
                    case MoveState.LosePlayer:
                        {
                            LosePlayer();
                            ChangeState(MoveState.Idle);
                            yield return null;
                        }
                        break;
                    case MoveState.Fall:
                        {
                            target = FindAnythingOnDirection(Vector2.down, rigidBody2D.gravityScale * moveTime);
                            if (target != null)
                            {
                                ChangeState(MoveState.Idle);
                            }
                            else
                            {
                                fallDistance += rigidBody2D.gravityScale * moveTime;
                            }
                            yield return null;
                        }
                        break;
                    case MoveState.BeenTrampled:
                        {
                            target = FindAnythingOnDirection(Vector2.up, 1f);
                            if (target == null)
                            {
                                ChangeState(MoveState.Idle);
                            }
                        }
                        break;
                    case MoveState.Consumed:
                        {
                            StopMove();
                        }
                        break;
                }
                yield return null;
            }
        }
    }
    #region MoveComp
    private void ConsumeMetal(IMadeByMetal metal)
    {
        metal.ConsumeMe();
    }
    public void OnBeenTrampled(MoveObject moveObj)
    {
        float x, y = moveObj.fallDistance;
        moveObj.ChangeGravity(0, 2f);
        if (moveObj is MoveCreature creature)
        {
            creature.ChangeState(MoveState.Jump, MoveState.Idle, 2f);
            if(creature.moveState.prevState == MoveState.Idle)
            {
                x = 0;
            }
            else
            {
                x = creature.moveDir.x;
            }
        }
        else
        {
            x = moveObj.moveDir.x;
        }
        moveObj.ForceMove(new Vector2(x, y), 2, moveObj.velocity);
    }
    private void LoseMetal()
    {
        //播放动画
    }
    private void LosePlayer()
    {
    }
    #endregion
    #endregion
    public void OnPlayerHit(Player player)
    {
        var playerCollider = player.gameObject.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollider, gameObject.GetComponent<BoxCollider2D>());
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (OnHitArea(collision))
        {
            var obj = collision.gameObject;
            IMadeByMetal metal = obj.GetComponent<IMadeByMetal>();
            if (metal != null)
            {
                ConsumeMetal(metal);
            }
        }
        base.OnCollisionEnter2D(collision);
    }
  }
