using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBat : Item, IHandlePlayerHit 
{
    private void OnDisable()
    {
        Debug.Log("disable");
    }
    void Start()
    {
        Debug.Log("start");
        StartCoroutine(SmoothMoveTo(new Vector2(1, 1)));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("enter");
    }
    public void OnPlayerHit(Player player)
    {
        player.StopMove();
        StartCoroutine(SmoothMoveTo(new Vector2(1, 1)));
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
