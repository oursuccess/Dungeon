using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MoveCharacter
{
    [SerializeField]
    private Text dieText;
    protected override void Start()
    {
        base.Start();

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
        base.StopMove();
    }

    public void StartMove()
    {
        SimpleAutoMove(moveDir);
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
        else if (collision.transform.name.Contains("Ground"))
        {
            animator.enabled = true;
            SimpleAutoMove(moveDir);
        }

        base.OnCollisionEnter2D(collision);
    }
}
