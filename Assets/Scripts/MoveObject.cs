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

    public delegate void MovingDel();
    public event MovingDel OnMoving;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        lastMoveTime = 0;
        inverseMoveTime = 1 / moveTime;
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
    }

    protected virtual void SimpleAutoMove(Vector2 direction)
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveRoutine = StartCoroutine(AutoMoveTo(direction));
    }

    public virtual void StopMove()
    {
        if(moveRoutine != null)
        {
            Debug.Log("stop");
            StopCoroutine(moveRoutine);
        }
    }

    private IEnumerator AutoMoveTo(Vector2 direction)
    {
        while (true)
        {
            Vector2 start = transform.position;
            rigidBody2D.MovePosition(start + direction * velocity * Time.deltaTime);
            OnMoving?.Invoke();
            yield return null;
        }
    }

    protected virtual void Move(Vector2 direction)
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveRoutine = StartCoroutine(SmoothMovement(direction));
    }

    private IEnumerator SmoothMovement(Vector2 direction)
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

            OnMoving?.Invoke();
            yield return null;
        }
        yield return true;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name.Contains("Ground"))
        {
            rigidBody2D.gravityScale = 0f;
            rigidBody2D.MovePosition(new Vector2(transform.position.x, transform.position.y + 0.01f));
        }
    }

}
