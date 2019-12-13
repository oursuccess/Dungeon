using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBat : Item, IHandlePlayerHit 
{
    private Player player;
    private Animator animator;

    protected override void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.enabled = false;

        base.Start();
    }
    public void OnPlayerHit(Player p)
    {
        animator.enabled = true;

        player = p;

        player.StopMove();
        player.transform.parent = transform;
        Physics2D.IgnoreCollision(player.gameObject.GetComponent<BoxCollider2D>(), gameObject.GetComponent<BoxCollider2D>());

        StartCoroutine(SmoothMoveTo(new Vector2(1, 1)));

        Invoke("ResetPlayerNBat", 2.4f);
    }

    private void ResetPlayerNBat()
    {
        animator.enabled = false;
        player.transform.parent = null;
        player.StartMove();
    }

    private IEnumerator SmoothMoveTo(Vector2 direction)
    {
        float distance;
        if(direction.y != 0)
        {
            distance = direction.y;
            while(distance >= float.Epsilon)
            {
                transform.Translate(new Vector2(0 ,Time.deltaTime));
                distance -= Time.deltaTime;
                yield return null;
            }
        }
        if(direction.x != 0)
        {
            distance = direction.x;
            while(distance >= float.Epsilon)
            {
                transform.Translate(new Vector2(Time.deltaTime, 0));
                distance -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
