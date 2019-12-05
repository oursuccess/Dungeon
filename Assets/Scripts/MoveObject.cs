using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveObject : MonoBehaviour
{
    [SerializeField]
    protected float moveTime;
    [SerializeField]
    protected float moveDistance;
    [SerializeField]
    protected float velocity;

    private float inverseMoveTime;

    private Rigidbody2D rigidBody2D;

    // Start is called before the first frame update
    void Start()
    {
        inverseMoveTime = 1 / moveTime;
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    protected void Move(Vector2 direction)
    {
        StartCoroutine(SmoothMovement(direction));
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
        }

        yield return true;
    }
}
