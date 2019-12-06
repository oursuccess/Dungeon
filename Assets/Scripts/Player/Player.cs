using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MoveCharacter
{
    protected override void Start()
    {
        base.Start();

        moveDir = Vector2.right;

        SimpleAutoMove(moveDir);
    }

    protected override void Update()
    {
    }

    protected override void SimpleAutoMove(Vector2 direction)
    {
        base.SimpleAutoMove(direction);
    }

    public void Dead()
    {
        enabled = false;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    { 
        IHandlePlayerHit hit = collision.transform.GetComponent<IHandlePlayerHit>();
        if (hit != null)
        {
            hit.OnPlayerHit();
        }
        base.OnCollisionEnter2D(collision);
    }
}
