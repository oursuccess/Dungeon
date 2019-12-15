using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveItem : MoveObject
{
    #region State
    protected class BaseMoveState
    {
        #region State
        public int currState;
        public int prevState;
        #endregion
        #region StateEnum
        public static int Idle { get; private set; } = 0;
        public static int Move { get; private set; } = 1;
        public static int Fall { get; private set; } = 2;
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
    #endregion
    #region Sight
    [Tooltip("影响对象的视野距离，10为全屏范围")]
    [SerializeField]
    protected float sightDistance;
    [Tooltip("影响视野范围，360表示所有角度均可观测，0表示前方一条线，90代表前方上下各45度")]
    [SerializeField]
    protected int sightRange;
    protected CircleCollider2D sightCollider;
    public Vector2 direction;
    [SerializeField]
    [Range(0, 100)]
    [Tooltip("移动的意愿")]
    protected int moveWill;
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
        StartCoroutine(MovingImpl());
    }
    protected abstract IEnumerator MovingImpl();
    protected virtual GameObject Find<T>()
    {
        boxcollider2D.enabled = false;
        GameObject res = null;
        //查找相关内容
        RaycastHit2D hit;
        Vector2 start = transform.position;
        for(int i = 0; i <= sightRange / 2; i += 10)
        {
            float f = (float)i / 180;
            Vector2 dir = new Vector2(direction.x, direction.y + f);
            hit = Physics2D.Raycast(start, dir, sightDistance);
            if(hit.collider == null)
            {
                hit = Physics2D.Raycast(start, dir, sightDistance);
            }
            if (hit.collider != null && hit.collider.gameObject.GetComponent<T>() != null)
            {
                res = hit.collider.gameObject;
                boxcollider2D.enabled = true;
                return res;
            }
        }
        boxcollider2D.enabled = true;
        return res;
    }
    public override void Init() 
    {
        moveDir = direction;
        base.Init();
    }
}
