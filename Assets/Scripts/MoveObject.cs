using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveObject : MonoBehaviour
{
    [SerializeField]
    protected float moveTime = 1;
    [SerializeField]
    protected float moveDistance = 1;
    [SerializeField]
    protected float velocity = 1;

    protected Vector2 moveDir;

    protected float lastMoveTime;

    protected float inverseMoveTime;
    protected Coroutine moveRoutine;

    protected Rigidbody2D rigidBody2D;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        lastMoveTime = 0;
        inverseMoveTime = 1 / moveTime;
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    protected virtual void SimpleMove(Vector2 direction)
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveRoutine = StartCoroutine(SmoothMovement(direction));
    }

    protected IEnumerator SmoothMovement(Vector2 direction)
    {
        Vector2 start = transform.position;
        Vector2 end = start + direction;
        float distanceNow = direction.sqrMagnitude;

        while(distanceNow >= float.Epsilon)
        {
            Vector2 target = Vector2.MoveTowards(start, end, inverseMoveTime * Time.deltaTime * velocity);
            rigidBody2D.MovePosition(target);
            start = transform.position;
            distanceNow = (end - start).sqrMagnitude;
            yield return null;
        }
        yield return true;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "ground")
        {
            rigidBody2D.gravityScale = 0f;
            rigidBody2D.MovePosition(new Vector2(0, 0.05f));
        }
    }

}
