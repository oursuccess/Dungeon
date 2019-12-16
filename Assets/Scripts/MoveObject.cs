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

    public Vector2 moveDir {get; protected set;}

    protected float lastMoveTime;

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
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxcollider2D = GetComponent<BoxCollider2D>();
    }
    public virtual void Init()
    {
        canMove = true;
        lastMoveTime = 0;
        if(rigidBody2D == null)
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
        }
        if(boxcollider2D == null)
        {
            boxcollider2D = GetComponent<BoxCollider2D>();
        }
    }
    #region AutoMove
    protected virtual void SimpleAutoMove(Vector2 direction)
    {
        if (canMove)
        {
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }
            if(moveDir != direction)
            {
                moveDir = direction;
                moveDir.Normalize();
            }
            StartCoroutine(AutoMoveTo());
        }
    }
    private IEnumerator AutoMoveTo()
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
    public virtual void StopMove(float stopTime = 0)
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            canMove = false;
        }
        if(stopTime > float.Epsilon)
        {
            Invoke("StartMove", stopTime);
        }
    }
    #endregion
    #region Move
    protected virtual void MoveNChangeDirection(Vector2 direction)
    {
        if (canMove)
        {
            moveDir = direction;
            moveDir.Normalize();
            Move();
        }
    }
    public virtual void Move()
    {
        if (canMove)
        {
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }
            moveRoutine = StartCoroutine(SmoothMovement(moveDir));
        }
   }
    public virtual void Move(Vector2 direction)
    {
        if (canMove)
        {
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }
            moveRoutine = StartCoroutine(SmoothMovement(direction));

        }
    }
    public virtual void ForceMove(Vector2 direction, float velocity)
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        StartCoroutine(ForceMovement(direction, velocity));
    }
    private IEnumerator ForceMovement(Vector2 direction, float velocity)
    {
        Vector2 start = transform.position;
        Vector2 end = start + direction;
        float distanceNow = direction.sqrMagnitude;

        while(distanceNow >= 0.05f)
        {
            Vector2 target = Vector2.Lerp(start, end, lastMoveTime * velocity / moveTime);
            rigidBody2D.MovePosition(target);
            start = transform.position;
            distanceNow = (end - start).sqrMagnitude;
            lastMoveTime += Time.deltaTime;

            yield return null;
        }
    }
    private IEnumerator SmoothMovement(Vector2 direction)
    {
        Vector2 start = transform.position;
        Vector2 end = start + direction;
        float distanceNow = direction.sqrMagnitude;
        Debug.Log(this);

        while(distanceNow >= 0.05f)
        {
            Vector2 target = Vector2.Lerp(start, end, lastMoveTime * velocity / moveTime);
            rigidBody2D.MovePosition(target);
            start = transform.position;
            distanceNow = (end - start).sqrMagnitude;
            lastMoveTime += Time.deltaTime;

            Moving?.Invoke(this);
            yield return null;
        }
    }
    #endregion
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsBelowThanMe(collision.gameObject))
        {
            IHaveTrampleEffect trampleObj = collision.gameObject.GetComponent<IHaveTrampleEffect>();
            if(trampleObj != null)
            {
                trampleObj.OnBeenTrampled(this);
            }
        }
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
    private bool IsBelowThanMe(GameObject gameObject)
    {
        if (gameObject.transform.position.y < transform.position.y && Mathf.Abs(gameObject.transform.position.x - transform.position.x) <= 1.5)
        {
            return true;
        }
        return false;
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
