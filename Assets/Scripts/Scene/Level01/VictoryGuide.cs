using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryGuide : Guide
{
    protected override void Start()
    {
        base.Start();

        GameObject door = GameObject.Find("Door");
        transform.position = new Vector3(door.transform.position.x - 3, door.transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            //应该是游戏暂停运行
            Physics2D.IgnoreCollision(collision, gameObject.GetComponent<BoxCollider2D>());

            GuideStart(0.5f, 1f, player);
        }

    }
}
