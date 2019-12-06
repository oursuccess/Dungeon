using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MoveObject
{
    [SerializeField]
    protected float sight = 1;
    [SerializeField]
    protected LayerMask layer;

    protected override void Start()
    {
        base.Start();
    }

    protected bool Find(Vector2 direction, LayerMask layer, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + direction * sight;
        hit = Physics2D.Linecast(transform.position, end, layer);
        if(hit.transform != null)
        {
            return true;
        }
        return false;
    }
    protected virtual void TryMove(Vector2 direction, LayerMask layer)
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveRoutine = StartCoroutine(SmoothMovement(direction));
    }

    protected IEnumerator SmoothMovement(Vector2 direction, LayerMask layer)
    {
        Vector2 start = transform.position;
        Vector2 end = start + direction;
        float distanceNow = direction.sqrMagnitude;
        RaycastHit2D hit;

        while(distanceNow >= float.Epsilon)
        {
            Vector2 target = Vector2.MoveTowards(start, end, inverseMoveTime * Time.deltaTime * velocity);
            rigidBody2D.MovePosition(target);
            start = transform.position;
            distanceNow = (end - start).sqrMagnitude;
            if (Find(direction, layer, out hit))
            {
                Debug.Log("found");
                IHandlePlayerSought handler = hit.transform.GetComponent<IHandlePlayerSought>();
                if (handler != null)
                {
                    handler.OnPlayerSought();
                }
            }
            yield return null;
        }
        yield return true;
    }

}
