using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryGuide : Guide
{
    GameObject door;
    protected override void Start()
    {
        base.Start();

        door = GameObject.Find("Door");
        door.GetComponent<ChestHandler>().PositionChanged += OnDoorPositionChanged;
    }

    private void OnDoorPositionChanged()
    {
        transform.position = new Vector3(door.transform.position.x - 3, door.transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            //应该是游戏暂停运行
            Physics2D.IgnoreCollision(collision, gameObject.GetComponent<BoxCollider2D>());

            GuideStart(player);
        }

    }
}
