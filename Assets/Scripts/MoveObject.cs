using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//所有可移动物体都带有rigidbody2D
public abstract class MoveObject : MonoBehaviour
{
    #region State
    protected class MoveState
    {
        #region State
        public int currState;
        public int prevState;
        #endregion
        #region StateEnum
        public static int Idle = 0;
        public static int Move = 1;
        public static int Jump = 2;
        public static int Fall = 3;
        #endregion
        #region PublicInterface
        public virtual void ChangeState(int state)
        {
            prevState = currState;
            currState = state;
        }
        #endregion
        #region event
        public delegate void StateChangedDel(int state);
        public event StateChangedDel StateChanged;
        #endregion
        #region InitState
        public MoveState()
        {
            currState = Idle;
            prevState = Idle;
        }
        #endregion
    }
    protected MoveState moveState;
    #endregion
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
    #endregion
    #region event
    public delegate void MovingDel(MoveObject moveObject);
    public event MovingDel Moving;
    #endregion
    protected virtual void Start()
    {
        moveState = new MoveState();
        lastMoveTime = 0;
        inverseMoveTime = 1 / moveTime;
        rigidBody2D = GetComponent<Rigidbody2D>();
    }
    protected virtual void Update()
    {
    }
    #region AutoMove
    protected virtual void SimpleAutoMove(Vector2 direction)
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveRoutine = StartCoroutine(AutoMoveTo(direction));
    }
    private IEnumerator AutoMoveTo(Vector2 direction)
    {
        while (true)
        {
            Vector2 start = transform.position;
            rigidBody2D.MovePosition(start + direction * velocity * Time.deltaTime);
            Moving?.Invoke(this);
            yield return null;
        }
    }
    #endregion
    #region MoveInterface
    public virtual void StartMove()
    {
        moveState.ChangeState(MoveState.Move);
    }
    public virtual void StopMove()
    {
        if(moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveState.ChangeState(MoveState.Idle)
        }
    }
    #endregion
    #region Move
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

            Moving?.Invoke(this);
            yield return null;
        }
        yield return true;
    }
    #endregion
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name.Contains("Ground"))
        {
            rigidBody2D.gravityScale = 0f;
            rigidBody2D.MovePosition(new Vector2(transform.position.x, transform.position.y + 0.01f));
        }
    }
}
