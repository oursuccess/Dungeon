using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBat : Item, IHandlePlayerHit 
{
    public void OnPlayerHit(Player player)
    {
        player.StopMove();
        SmoothMoveTo(new Vector2(1, 1));
        player.StartMove();
    }

    private void SmoothMoveTo(Vector2 direction)
    {
        float distance;
        if(direction.y != 0)
        {
            distance = direction.y;
            while(distance >= float.Epsilon)
            {
                transform.Translate(new Vector2(0 ,Time.deltaTime));
                distance -= Time.deltaTime;
            }
        }
        if(direction.x != 0)
        {
            distance = direction.x;
            while(distance >= float.Epsilon)
            {
                transform.Translate(new Vector2(Time.deltaTime, 0));
                distance -= Time.deltaTime;
            }
        }
    }
}
