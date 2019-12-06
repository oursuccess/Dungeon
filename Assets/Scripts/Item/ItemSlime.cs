using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlime : Item, IHandlePlayerHit
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Effect(collision.gameObject);
        }
    }

    public override void Effect(GameObject target)
    {
        transform.position = target.transform.position;
        //应该由对象选择如何失效
        Destroy(target.GetComponent<BoxCollider2D>());
        enabled = false;
    }

    private void OnDisable()
    {
        transform.rotation = Quaternion.identity;
    }

    public void OnPlayerHit(Player player)
    {
        var playerCollider = player.gameObject.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollider, gameObject.GetComponent<BoxCollider2D>());
    }
}
