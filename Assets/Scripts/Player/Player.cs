using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MoveCharacter
{
    [SerializeField]
    private Text dieText;

    private bool canMove;
    protected override void Start()
    {
        base.Start();

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

    public void StartMove()
    {
        canMove = true;
        rigidBody2D.gravityScale = 1;
    }

    public override void Dead()
    {
        base.Dead();

        Invoke("ShowDieMessage", 3f);
    }

    private void ShowDieMessage()
    {
        dieText.gameObject.SetActive(true);
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
