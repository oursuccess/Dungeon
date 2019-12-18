using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MoveCharacter
{
    protected override void Start()
    {
        moveDir = Vector2.right;
        currState = MoveState.Idle;
        
        base.Start();

        StartMove();
    }
    
    #region Old
    protected override void SimpleAutoMove(Vector2 direction)
    {
        base.SimpleAutoMove(direction);
    }
    #endregion
    #region MoveInterface
    public override void StopMove(float stopTime = 0)
    {
        canMove = false;
        base.StopMove();
    }
    public override void StartMove()
    {
        StartCoroutine(MoveImpl());
    }
    #endregion
    #region GameOver
    protected override void Dead()
    {
        base.Dead();

        //to enable
        Invoke("GameOver", 1f);
    }
    private void GameOver()
    {
        GameManager.Instance.GameOver();
    }
    #endregion
    #region Won
    public void Won()
    {
        StopMove();
        Invoke("GameWon", 1f);
    }
    private void GameWon()
    {
        GameManager.Instance.ToNextLevel();
    }
    #endregion
    #region HandleHit(Hit,Ground)
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (OnHitArea(collision))
        {
            IHandlePlayerHit hit = collision.gameObject.GetComponent<IHandlePlayerHit>();
            if (hit != null)
            {
                ChangeState(MoveState.FindThing);
                hit.OnPlayerHit(this);
            }
        }
        base.OnCollisionEnter2D(collision);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        IHandlePlayerHit hit = collision.gameObject.GetComponent<IHandlePlayerHit>();
        if (hit != null)
        {
            hit.OnPlayerHit(this);
        }
        base.OnTriggerEnter2D(collision);
    }
    #endregion
    #region Move
    private IEnumerator MoveImpl()
    {
        Collider2D target = null;
        while (canMove)
        {
            if(lastMoveTime <= moveTime) 
            {
                lastMoveTime += Time.deltaTime;
            }
            else
            {
                lastMoveTime = 0f;
                switch (currState)
                {
                    case MoveState.Idle:
                        {
                            target = FindAnythingOnDirection(Vector2.down, 0.3f);
                            if (target == null)
                            {
                                ChangeState(MoveState.Fall);
                            }
                            else
                            {
                                target = FindAnythingOnDirection(moveDir, moveDistance);
                                if(target == null || target.isTrigger == true)
                                {
                                    ChangeState(MoveState.Move);
                                }
                            }
                        }
                        break;
                    case MoveState.Move:
                    case MoveState.Run:
                        {
                            target = FindAnythingOnDirection(Vector2.down, 0.3f);
                            if (target == null)
                            {
                                ChangeState(MoveState.Fall);
                            }
                            else
                            {
                                target = FindAnythingOnDirection(moveDir, moveDistance);
                                if(target == null || target.isTrigger == true)
                                {
                                    Move();
                                }
                                else
                                {
                                    target = FindAnythingOnDirection(moveDir, sightDistance);
                                    if(target != null)
                                    {
                                        IHandlePlayerSought playerSought = target.gameObject.GetComponent<IHandlePlayerSought>();
                                        if (playerSought != null)
                                        {
                                            playerSought.OnPlayerSought(this);
                                        }
                                    }
                                }
                               
                            }
                        }
                        break;
                    case MoveState.Jump:
                        {
                            target = FindAnythingOnDirection(Vector2.down, 0.3f);
                            if (target == null)
                            {
                                ChangeState(MoveState.Fall);
                            }
                        }
                        break;
                    case MoveState.Fall:
                        {
                            target = FindAnythingOnDirection(Vector2.down, 0.3f);
                            Debug.Log("fall" + target);
                            if (target != null)
                            {
                                IHaveTrampleEffect ihte = target.gameObject.GetComponent<IHaveTrampleEffect>();
                                if (ihte != null)
                                {
                                    ihte.OnBeenTrampled(this);
                                }
                                else
                                {
                                    if (IsGround(target))
                                    {
                                        Debug.Log("fall to idle");
                                        ChangeState(MoveState.Idle);
                                    }
                                    else
                                    {
                                        fallDistance += rigidBody2D.gravityScale * moveTime;
                                    }
                                }
                            }
                            //wait for hit collider
                        }
                        break;
                    case MoveState.Crawl:
                        {
                        }
                        break;
                    case MoveState.FindThing:
                        {
                            Move();
                            target = FindAnythingOnDirection(moveDir, sightDistance);
                            IHandlePlayerSought playerSought = target.gameObject.GetComponent<IHandlePlayerSought>();
                            if (playerSought == null)
                            {
                                ChangeState(MoveState.Idle);
                            }
                        }
                        break;
                    case MoveState.Attack:
                        {
                            ChangeState(MoveState.Idle);
                        }
                        break;
                    default:
                        break;
                }
            }
            yield return null;
        }
        yield return null;
    }
    #endregion
}
