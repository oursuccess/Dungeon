using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//所有可移动物体都带有rigidbody2D
public abstract class MoveObject : MonoBehaviour
{
    #region Var
    #region move
    [SerializeField]
    protected float moveTime = 1;
    [SerializeField]
    protected float moveDistance = 1;
    [SerializeField]
    protected float velocity = 1;
    public bool canMove;

    protected Vector2 moveDir;

    protected float lastMoveTime;

    protected float inverseMoveTime;
    protected Coroutine moveRoutine;
    #endregion
    #region object
    protected Rigidbody2D rigidBody2D;
    protected BoxCollider2D boxcollider2D;
    #endregion
    #region event
    public delegate void MovingDel(MoveObject moveObject);
    public event MovingDel Moving;
    #endregion
    #region Layer
    LayerMask layer;
    #endregion
    #endregion
    protected virtual void Start()
    {
        lastMoveTime = 0;
        moveDir = Vector2.zero;
        inverseMoveTime = 1 / moveTime;
        rigidBody2D = GetComponent<Rigidbody2D>();
    }
    #region AutoMove
    protected virtual void SimpleAutoMove(Vector2 direction)
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveDir = direction;
        moveDir.Normalize();
        StartCoroutine(AutoMoveTo(direction));
    }
    private IEnumerator AutoMoveTo(Vector2 direction)
    {
        while (canMove)
        {
            Invoke("Move", 1f);
            yield return null;
        }
    }
    #endregion
    #region MoveInterface
    public virtual void StartMove()
    {
        canMove = true;
    }
    public virtual void StopMove()
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            canMove = false;
        }
    }
    #endregion
    #region Move
    protected virtual void Move(Vector2 direction)
    {
        moveDir = direction;
        moveDir.Normalize();
        Move();
    }
    protected virtual void Move()
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveRoutine = StartCoroutine(SmoothMovement(moveDir));
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

            Moving?.Invoke(this);
            yield return null;
        }
        yield return true;
    }
    #endregion
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.transform.name.Contains("Ground"))
        //{
        //    rigidBody2D.MovePosition(new Vector2(transform.position.x, transform.position.y + 0.01f));
        //}
    }
    protected virtual bool FindThingOnDirection(LayerMask layer, Vector2 direction, float distance, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + direction * distance;
        boxcollider2D.enabled = false;
        hit = Physics2D.Linecast(start, end, layer);
        boxcollider2D.enabled = true;
        if(hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    protected virtual GameObject FindAnythingOnDirection(Vector2 direction, float distance)
    {
        GameObject res = null;
        boxcollider2D.enabled = false;
        var hit = Physics2D.Raycast(transform.position, direction * distance);
        boxcollider2D.enabled = true;
        if(hit.collider != null)
        {
            res = hit.collider.gameObject;
        }
        return res;
    }
}
