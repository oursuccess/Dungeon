using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MoveCharacter
{
    #region Emote
    public GameObject emote;
    #endregion
    #region Temp
    private float animSizeXRatio = 0.1f;
    private float animSizeYRatio = 0.1f;
    #endregion
    protected override void Start()
    {
        base.Start();

        //to clean
        gameObject.transform.localScale = new Vector2(animSizeXRatio, animSizeYRatio);

        moveDir = Vector2.right;
        currState = MoveState.Idle;
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
        emote.SetActive(false);
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
        GameManager.Instance.ToNextStage();
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
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        IHandlePlayerHit hit = collision.gameObject.GetComponent<IHandlePlayerHit>();
        if (hit != null)
        {
            hit.OnPlayerHit(this);
        }
    }
    #endregion
    #region Move
    private IEnumerator MoveImpl()
    {
        GameObject target = null;
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
                            ChangeState(MoveState.Move);
                        }
                        break;
                    case MoveState.Move:
                    case MoveState.Run:
                        {
                            target = FindAnythingOnDirection(Vector2.down, 1f);
                            if (target == null)
                            {
                                ChangeState(MoveState.Fall);
                            }
                            else
                            {
                                Move();
                                target = FindAnythingOnDirection(moveDir, 1f);
                                IHandlePlayerSought playerSought = target.GetComponent<IHandlePlayerSought>();
                                if (playerSought != null)
                                {
                                    playerSought.OnPlayerSought(this);
                                }
                            }
                        }
                        break;
                    case MoveState.Jump:
                        {
                            target = FindAnythingOnDirection(Vector2.down, 1);
                            if (target == null)
                            {
                                ChangeState(MoveState.Fall);
                            }
                        }
                        break;
                    case MoveState.Fall:
                        {
                            target = FindAnythingOnDirection(Vector2.down, 3);
                            if (target != null)
                            {
                                IHaveTrampleEffect ihte = target.GetComponent<IHaveTrampleEffect>();
                                if (ihte != null)
                                {
                                    ChangeState(MoveState.Idle);
                                }
                                else
                                {
                                    fallDistance += rigidBody2D.gravityScale * moveTime;
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
                            target = FindAnythingOnDirection(moveDir, 1f);
                            IHandlePlayerSought playerSought = target.GetComponent<IHandlePlayerSought>();
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
