using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBat : Item, IHandlePlayerHit 
{
    public void OnPlayerHit(Player player)
    {
        player.StopMove();
        Debug.Log(player.transform.parent);
        player.transform.parent = transform;
        Coroutine routineMove = StartCoroutine(SmoothMoveTo(new Vector2(1, 1)));

        Invoke("ResetPlayer", 3f);
    }

    private void ResetPlayer(Player player)
    {
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
