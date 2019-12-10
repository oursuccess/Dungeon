using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuide : Guide
{
    protected override void Start()
    {
        base.Start();

        GameObject stab = GameObject.Find("Stab01");
        transform.position = new Vector3(stab.transform.position.x - 3f, Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + 1f);
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
