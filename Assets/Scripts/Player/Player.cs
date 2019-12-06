using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MoveCharacter
{
    protected override void Start()
    {
        moveDir = Vector2.right;
        base.Start();
    }

    private void Update()
    {
        lastMoveTime += Time.deltaTime;
        if(lastMoveTime >= moveTime)
        {
            TryMove(moveDir, layer);
        }
    }

    public void Dead()
    {
        enabled = false;
        Debug.Log("Dead");
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
}
