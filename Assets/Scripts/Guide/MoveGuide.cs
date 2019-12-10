using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGuide : Guide 
{
    void Start()
    {
        transform.position = GameObject.Find("Player").transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if(player != null)
        {
            //应该是游戏暂停运行
            player.StopMove();

            TextShow();

            if(Input.anyKeyDown)
            {
                Debug.Log("down");
                TextClose();
                player.StartMove();
            }
        }
    }
}
