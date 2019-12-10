using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGuide : Guide 
{
    protected override void Start()
    {
        transform.position = GameObject.Find("Player").transform.position + new Vector3(3f ,0);

        base.Start();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if(player != null)
        {
            //应该是游戏暂停运行
            Physics2D.IgnoreCollision(collision, gameObject.GetComponent<BoxCollider2D>());

            GuideStart(1f, 3f, player);
        }
    }

}
