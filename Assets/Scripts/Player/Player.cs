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
        moveState = MoveState.Idle;

        if(emote != null)
        {
            emote.SetActive(false);
        }

        //to clean
        gameObject.transform.localScale = new Vector2(animSizeXRatio, animSizeYRatio);

        canMove = true;
        animator.enabled = false;
        moveDir = Vector2.right;

        StartMove();
    }
    protected override void SimpleAutoMove(Vector2 direction)
    {
        base.SimpleAutoMove(direction);
    }
    #region MoveInterface
    public override void StopMove(float stopTime = 0)
    {
        canMove = false;
        base.StopMove();
    }
    public override void StartMove()
    {
        canMove = true;
        StartCoroutine(MoveImpl());
    }
    #endregion
    #region GameOver
    public override void Dead()
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
                hit.OnPlayerHit(this);
            }
        }
        base.OnCollisionEnter2D(collision);
    }
    protected override bool OnHitArea(Collision2D collision)
    {
        bool res = false;
        //这样要求所有角色的位置放在下方而非中心；或者所有角色的大小相近（不超过2）
        if (Mathf.Abs(collision.transform.position.y - transform.position.y) <= 1f)
        {
            res = true;
        }
        return res;
    }
    #endregion
    #region CallEnemy(Hit,Saw)
    protected void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.isTrigger)
        {
            ICanSawPlayer sought = collision.gameObject.GetComponent<ICanSawPlayer>();
            if (sought != null)
            {
                sought.ISawPlayer(this);
            }
        }
        else
        {
            IHandlePlayerSought hit = collision.gameObject.GetComponent<IHandlePlayerSought>();
            if (hit != null)
            {
                hit.OnPlayerSought(this);
            }
        }
    }
    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            ICanSawPlayer sought = collision.gameObject.GetComponent<ICanSawPlayer>();
            if (sought != null)
            {
                sought.ILosePlayer(this);
            }
        }
        else
        {
            IHandlePlayerSought hit = collision.gameObject.GetComponent<IHandlePlayerSought>();
            if (hit != null)
            {
                hit.OnPlayerSought(this);
            }
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
                switch (moveState)
                {
                    case MoveState.Idle:
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
                                    Move(new Vector2(target.transform.position.x , 0));
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
            }
            }
            yield return null;
        }
        yield return null;
    }
    #endregion
}
