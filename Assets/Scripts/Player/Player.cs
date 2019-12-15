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

        if(emote != null)
        {
            emote.SetActive(false);
        }

        //to clean
        gameObject.transform.localScale = new Vector2(animSizeXRatio, animSizeYRatio);

        canMove = true;
        animator.enabled = false;
        moveDir = Vector2.right;

        SimpleAutoMove(moveDir);
    }
    protected override void SimpleAutoMove(Vector2 direction)
    {
        base.SimpleAutoMove(direction);
    }
    #region MoveInterface
    public override void StopMove()
    {
        canMove = false;
        base.StopMove();
    }
    public override void StartMove()
    {
        canMove = true;
        rigidBody2D.gravityScale = 1;
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
        IHandlePlayerHit hit = collision.gameObject.GetComponent<IHandlePlayerHit>();
        if (hit != null)
        {
            hit.OnPlayerHit(this);
        }

        //else if (canMove && collision.transform.name.Contains("Ground"))
        //{
        //    animator.enabled = true;
        //    SimpleAutoMove(moveDir);
        //}

        base.OnCollisionEnter2D(collision);
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
}
