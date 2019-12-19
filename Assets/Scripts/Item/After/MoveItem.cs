using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveItem : MoveObject, IHaveMoveState
{
    #region State
    public class BaseMoveState
    {
        #region State
        public int currState;
        public int prevState;
        #endregion
        #region StateEnum
        public static int Idle { get; private set; } = 0;
        public static int Move { get; private set; } = 1;
        public static int Fall { get; private set; } = 2;
        public static int Die { get; private set; } = 3;
        public static int Jump { get; private set; } = 4;
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
    }
    public virtual void ChangeState(int state)
    {
        if(moveState.currState != state)
        {
            moveState.ChangeState(state);
            if (state == BaseMoveState.Die)
            {
                Dead();
            }
        }
    }
    public virtual void ChangeState(int stateFirst, int stateLast, float switchTime)
    {
        ChangeState(stateFirst);
        StartCoroutine(WaitToChangeState(stateLast, switchTime));
    }
    protected IEnumerator WaitToChangeState(int state, float time)
    {
        float t = 0f;
        while(t <= time)
        {
            t += Time.deltaTime;
            yield return null;
        }
        ChangeState(state);
        yield return true;
    }
    public BaseMoveState moveState { get; protected set; }
    #endregion
    protected override void Start()
    {
        base.Start();
    }
    protected virtual void OnEnable()
    {
        if(moveDir != Vector2.zero)
        {
            StartMove();
        }
    }
    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }
    public override void StartMove()
    {
        if (canMove)
        {
            StartCoroutine(MovingImpl());
        }
    }
    public override void StopMove(float stopTime = 0)
    {
        base.StopMove(stopTime);
    }
    protected abstract IEnumerator MovingImpl();
    public override void Init() 
    {
        moveWill = UnityEngine.Random.Range(0, 100);
        sightDirection = moveDir;
        base.Init();
    }
    protected override void Dead()
    {
        Invoke("AfterDead", 3f);
        base.Dead();
    }
    protected virtual void AfterDead()
    {
        Destroy(this);
    }
}
