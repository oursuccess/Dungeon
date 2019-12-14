using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveEnemy : MoveObject
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
        public static int Jump { get; private set; } = 2;
        public static int Fall { get; private set; } = 3;
        public static ushort StateNums { private set; get; }
        public static Dictionary<string, int> States { get; private set; }
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
        public void InitState()
        {
            if(States == null || States.Count != StateNums)
            {
                var properties = GetType().GetProperties();
                if (properties != null)
                {
                    States = new Dictionary<string, int>();
                    foreach (var prop in properties)
                    {
                        if (prop.PropertyType == typeof(int))
                        {
                            States.Add(prop.Name, (int)prop.GetValue(null));
                        }
                    }
                    StateNums = (ushort)States.Count;
                }
            }
        }
        public BaseMoveState()
        {
            InitState();
        }
        #endregion
    }
    #endregion
    protected virtual void OnEnable()
    {
        StartMove();
    }
    public override void StartMove()
    {
        StartCoroutine(MovingImpl());
    }
    protected abstract IEnumerator MovingImpl();
}
