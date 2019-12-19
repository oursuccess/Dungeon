using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveCreature : MoveObject, IHaveMoveState
{
    protected override void Start()
    {
        base.Start();
    }
    #region Find
    #region Var
    [SerializeField]
    protected float sight = 1;
    #endregion
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
    #region MoveState
    public class BaseMoveState
    {
        #region State
        public int currState;
        public int prevState;
        #endregion
        #region StateEnum
        public const int Idle = 0;
        public const int Move = 1;
        public const int Fall = 2;
        public const int Die = 3;
        public const int Jump = 4;
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
    #region Del
    public delegate void StateChangedDel(MoveCharacter character);
    public event StateChangedDel OnStateChanged;
    #endregion
    #endregion
    #region MoveImpl
    public override void StartMove()
    {
        if (canMove)
        {
            StartCoroutine(MovingImpl());
        }
    }
    protected abstract IEnumerator MovingImpl();
#endregion
}
