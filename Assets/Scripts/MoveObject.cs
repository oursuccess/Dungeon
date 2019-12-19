using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//所有可移动物体都带有rigidbody2D
public abstract class MoveObject : MonoBehaviour
{
    #region Var
    #region Gravity
    private float baseG;
    #endregion
    #region Collision
    #region Support
    protected float edgeFindDistance = 0.01f;
    protected float positionOffset = 0.03f;
    protected int overlapTriggers = 0;
    protected int overlapCollisions = 0;
    #endregion
    public float colliderWidthToLeft { get; protected set; }
    public float colliderWidthToRight { get; protected set; }
    public float colliderHeightToUp { get; protected set; }
    public float colliderHeightToBottom { get; protected set; }
    public float colliderWidth { get; protected set; }
    public float colliderHeight { get; protected set; }
    public bool isInTrigger
    {
        get
        {
            return overlapTriggers > 0;
        }
    }
    public bool isInCollision
    {
        get
        {
            return overlapCollisions > 0;
        }
    }
    #endregion
    #region move
    #region Editor
    [SerializeField]
    [Range(0f, 10f)]
    [Tooltip("移动所需的时间，单位（秒）")]
    public float moveTime = 1;
    [SerializeField]
    [Range(0f, 10f)]
    [Tooltip("单次移动的距离")]
    public float moveDistance = 1;
    [SerializeField]
    [Tooltip("移动速度")]
    public float velocity = 1;
    [Tooltip("是否可以移动")]
    public bool canMove;
    [Tooltip("目前的移动方向")]
    public Vector2 moveDir;
    [SerializeField]
    [Range(0, 100)]
    [Tooltip("移动的意愿")]
    protected int moveWill;
    #endregion
    protected float lastMoveTime;
    protected Coroutine moveRoutine;
    protected float findDistance = 0.3f;
    public float fallDistance { get; protected set; }
    #endregion
    #region Componnent
    public Rigidbody2D rigidBody2D { get; protected set; }
    public BoxCollider2D boxcollider2D { get; protected set; }
    #endregion
    #region event
    public delegate void MovingDel(MoveObject moveObject);
    public event MovingDel Moving;
    public delegate void DieDel(MoveObject moveObject);
    public event DieDel OnDead;
    #endregion
    #region Sight
    [Tooltip("影响对象的视野距离，10为全屏范围")]
    [SerializeField]
    protected float sightDistance;
    [Tooltip("影响视野范围，360表示所有角度均可观测，0表示前方一条线，90代表前方上下各45度")]
    [SerializeField]
    protected int sightRange;
    public Vector2 sightDirection;
    #endregion
    #endregion
    #region Init
    protected virtual void Start()
    {
        Init();
    }
    public virtual void Init()
    {
        canMove = true;
        lastMoveTime = 0;
        fallDistance = 0;
        if(rigidBody2D == null)
        {
            rigidBody2D = GetComponent<Rigidbody2D>();
        }
        if(boxcollider2D == null)
        {
            boxcollider2D = GetComponent<BoxCollider2D>();
        }
        baseG = rigidBody2D.gravityScale;

        Vector2 start = transform.position;
        colliderWidthToLeft = start.x - FindEdge(Vector2.left * edgeFindDistance).x;
        colliderWidthToRight = FindEdge(Vector2.right * edgeFindDistance).x - start.x;
        colliderWidth = colliderWidthToLeft + colliderWidthToRight;
        colliderHeightToUp = FindEdge(Vector2.up * edgeFindDistance).y - start.y;
        colliderHeightToBottom = start.y - FindEdge(Vector2.down * edgeFindDistance).y;
        colliderHeight = colliderHeightToUp + colliderHeightToBottom;
    }
    protected virtual Vector2 FindEdge(Vector2 direction)
    {
        Vector2 start = transform.position;
        Collider2D target;
        while((target = Physics2D.Raycast(start, direction).collider) != null && target.gameObject == gameObject)
        {
            start += direction;
        }
        return start;
    }
    #endregion
    #region Move
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
        Move(moveDir);
    }
    public virtual void Move(Vector2 direction)
    {
        Move(moveDir, moveDistance, velocity);
    }
    public virtual void Move(Vector2 direction, float distance, float velocity)
    {
        if (canMove)
        {
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }
            moveRoutine = StartCoroutine(SmoothMovement(direction, distance, velocity));
        }
    }
    public virtual void MoveTo(Vector2 targetPosition, float moveTime)
    {
        float distance = (targetPosition - (Vector2)transform.position).sqrMagnitude;
        float velocity = distance / moveTime;
        Vector2 direction = targetPosition - (Vector2)transform.position;
        direction.Normalize();
        Move(direction, distance, velocity);
    }
    public virtual void ForceMove(Vector2 direction)
    {
        ForceMove(direction, velocity);
    }
    public virtual void ForceMove(Vector2 direction, float velocity)
    {
        ForceMove(direction, moveDistance, velocity);
    }
    public virtual void ForceMove(Vector2 direction, float distance, float velocity)
    {
        StartCoroutine(ForceMovement(direction, distance, velocity));
    }
    private IEnumerator SmoothMovement(Vector2 direction, float distance, float velocity) 
    {
        if (canMove)
        {
            Vector2 start = transform.position;
            Vector2 end = start + direction * distance;
            float distanceNow = direction.sqrMagnitude;
            float t = 0;

            while (distanceNow >= 0.01f)
            {
                Vector2 target = Vector2.Lerp(start, end, t * velocity / moveTime);
                Collider2D collision = Physics2D.Linecast(start, target).collider;
                if (collision != null && IsUnmovableWall(collision))
                {
                    distanceNow = 0;
                    yield return true;
                }
                else
                {
                    rigidBody2D.MovePosition(target);
                    start = transform.position;
                    distanceNow = (end - start).sqrMagnitude;
                    t += Time.deltaTime;

                    Moving?.Invoke(this);
                    yield return null;
                }
            }
        }
        else
        {
            yield return true;
        }
    }
    private IEnumerator ForceMovement(Vector2 direction, float distance, float velocity)
    {
        Vector2 start = transform.position;
        Vector2 end = start + direction * distance;
        float distanceNow = direction.sqrMagnitude;

        while(distanceNow >= 0.01f)
        {
            Vector2 target = Vector2.Lerp(start, end, velocity / moveTime);
            Collider2D collision = Physics2D.Linecast(start, target).collider;
            if(collision != null && IsUnmovableWall(collision))
            {
                distanceNow = 0;
                yield return true;
            }
            else
            {
                rigidBody2D.MovePosition(target);
                start = transform.position;
                distanceNow = (end - start).sqrMagnitude;

                Moving?.Invoke(this);
                yield return null;
            }
        }
    }
    #endregion
    #endregion
    #region CollisionNFind
    #region Collision
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        overlapCollisions++;
        if (IsBelowThanMe(collision.gameObject))
        {
            IHaveTrampleEffect trampleObj = collision.gameObject.GetComponent<IHaveTrampleEffect>();
            if(trampleObj != null)
            {
                trampleObj.OnBeenTrampled(this);
            }
        }
    }
    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        overlapCollisions--;
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        overlapTriggers++;
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        overlapTriggers--;
    }
    private bool IsBelowThanMe(GameObject gameObject)
    {
        if (gameObject.transform.position.y < transform.position.y && Mathf.Abs(gameObject.transform.position.x - transform.position.x) <= 1.5)
        {
            return true;
        }
        return false;
    }
    protected virtual bool OnHitArea(Collision2D collision)
    {
        bool res = false;
        //这样要求所有角色的位置放在下方而非中心；或者所有角色的大小相近（不超过2）
        if (Mathf.Abs(collision.transform.position.y - transform.position.y) <= 1f)
        {
            res = true;
        }
        return res;
    }
    protected virtual bool IsGround(Collider2D collision)
    {
        return collision.gameObject.name.Contains("Level");
    }
    protected virtual bool IsGround(GameObject gameObject)
    {
        return gameObject.name.Contains("Level");
    }
    protected virtual bool IsUnmovableWall(Collider2D collision)
    {
        return collision.gameObject.name.Contains("Level");
    }
    #endregion
    #region Find
    protected virtual bool FindThingOnDirection(LayerMask layer, Vector2 direction, float distance, out RaycastHit2D hit)
    {
        if (direction == Vector2.zero)
        {
            hit = new RaycastHit2D();
            return false;
        }
        float baseXPos = transform.position.x;
        float baseYPos = transform.position.y;
        positionOffset = distance / 3;
        
        if(direction.x == 0)
        {
            float yOffset = direction.y > 0 ? colliderHeightToUp - positionOffset: -colliderHeightToBottom + positionOffset;
            float YPos = baseYPos + yOffset;
            baseXPos -= colliderWidthToLeft;

            for (float x = 0f; x < colliderWidth; x += findDistance)
            {
                Vector2 start = new Vector2(baseXPos + x, YPos);
                Vector2 end = start + direction * distance;

                boxcollider2D.enabled = false;
                hit = Physics2D.Linecast(start, end, layer);
#if UNITY_EDITOR
                Debug.DrawLine(start, end, Color.red);
#endif
                boxcollider2D.enabled = true;
                if (hit.collider != null)
                {
                    return true;
                }
            }
        }
        else if (direction.x != 0 && direction.y == 0)
        {
            float xOffset = direction.x > 0 ? colliderWidthToRight - positionOffset: -colliderWidthToLeft + positionOffset; 
            float XPos = baseXPos + xOffset;
            baseYPos -= colliderHeightToBottom;

            for (float y = 0f; y < colliderHeight; y += findDistance)
            {
                Vector2 start = new Vector2(XPos, baseYPos + y);
                Vector2 end = start + direction * distance;

                boxcollider2D.enabled = false;
                hit = Physics2D.Linecast(start, end, layer);
#if UNITY_EDITOR
                Debug.DrawLine(start, end, Color.red);
#endif
                boxcollider2D.enabled = true;
                if (hit.collider != null)
                {
                    return true;
                }
            }
        }
        else
        {
            Debug.Log("尚未实现非横向纵向检测");
        }
        hit = new RaycastHit2D();
        return false;
    }
    protected virtual Collider2D FindAnythingOnDirection(Vector2 direction, float distance)
    {
        Collider2D res = null;
        if (direction == Vector2.zero) return res;

        float baseXPos = transform.position.x;
        float baseYPos = transform.position.y;
        positionOffset = distance / 3;

        if(direction.x == 0)
        {
            float yOffset = direction.y > 0 ? colliderHeightToUp - positionOffset: -colliderHeightToBottom + positionOffset;
            float YPos = baseYPos + yOffset;
            baseXPos -= colliderWidthToLeft;

            for(float x = 0f; x <= colliderWidth; x += findDistance)
            {
                Vector2 start = new Vector2(baseXPos + x, YPos);
                Vector2 end = start + direction * distance;

                boxcollider2D.enabled = false;
                var hit = Physics2D.Linecast(start, end);
#if UNITY_EDITOR
                Debug.DrawLine(start, end, Color.red);
#endif
                boxcollider2D.enabled = true;
                if (hit.collider != null)
                {
                    res = hit.collider;
                    return res;
                }
            }
        }
        else if(direction.x != 0 && direction.y == 0)
        {
            float xOffset = direction.x > 0 ? colliderWidthToRight - positionOffset: -colliderWidthToLeft + positionOffset;
            float XPos = baseXPos + xOffset;
            baseYPos -= colliderHeightToBottom;

            for (float y = 0f; y <= colliderHeight; y += findDistance)
            {
                Vector2 start = new Vector2(XPos, baseYPos + y);
                Vector2 end = start + direction * distance;

                boxcollider2D.enabled = false;
                var hit = Physics2D.Linecast(start, end);
                boxcollider2D.enabled = true;
#if UNITY_EDITOR
                Debug.DrawLine(start, end, Color.red);
#endif
                if (hit.collider != null)
                {
                    res = hit.collider;
                    return res;
                }
            }
        }
        else
        {
            Debug.Log("尚未实现斜向检测");
        }
        return res;
    }
    protected virtual Collider2D FindWithEye<T>()
    {
        boxcollider2D.enabled = false;
        Collider2D res = null;
        //查找相关内容
        RaycastHit2D hit;
        Vector2 start = transform.position;
        for(int i = 0; i <= sightRange / 2; i += 10)
        {
            float f = (float)i / 180;
            Vector2 end = start + new Vector2(sightDirection.x, sightDirection.y + f) * sightDistance;
            hit = Physics2D.Linecast(start, end);
            if(hit.collider == null)
            {
                hit = Physics2D.Linecast(start, end);
            }
            if (hit.collider != null && hit.collider.gameObject.GetComponent<T>() != null)
            {
                res = hit.collider;
                boxcollider2D.enabled = true;
                return res;
            }
        }
        boxcollider2D.enabled = true;
        return res;
    }
    #endregion
    #endregion
    #region Gravity
    public void ChangeGravity(float gravity, float resetTime = 0)
    {
        if(resetTime > 0)
        {
            StartCoroutine(SetGravity(gravity, resetTime));
        }
        else
        {
            rigidBody2D.gravityScale = gravity;
        }
    }
    private IEnumerator SetGravity(float gravity, float resetTime)
    {
        rigidBody2D.gravityScale = gravity;
        float passT = 0f;
        while(passT < resetTime)
        {
            passT += Time.deltaTime;
            yield return null;
        }
        rigidBody2D.gravityScale = baseG;
        yield return true;
    }
    #endregion
    #region Die
    protected virtual void Dead()
    {
        OnDead?.Invoke(this);
    }
    #endregion
}
