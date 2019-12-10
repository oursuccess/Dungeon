using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MoveCharacter
{
    private float animSizeXRatio = 0.1f;
    private float animSizeYRatio = 0.1f;
   
    protected override void Start()
    {
        base.Start();

        //to clean
        gameObject.transform.localScale = new Vector2(animSizeXRatio, animSizeYRatio);

        canMove = true;
        animator.enabled = false;
        moveDir = Vector2.right;
    }

    protected override void Update()
    {
    }

    protected override void SimpleAutoMove(Vector2 direction)
    {
        base.SimpleAutoMove(direction);
    }

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

    public override void Dead()
    {
        base.Dead();

        //to enable
        //Invoke("GameOver", 3f);
    }

    private void GameOver()
    {
        GameManager.Instance.GameOver();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        IHandlePlayerHit hit = collision.transform.GetComponent<IHandlePlayerHit>();
        if (hit != null)
        {
            hit.OnPlayerHit(this);
        }
        else if (canMove && collision.transform.name.Contains("Ground"))
        {
            animator.enabled = true;
            SimpleAutoMove(moveDir);
        }

        base.OnCollisionEnter2D(collision);
    }
}
