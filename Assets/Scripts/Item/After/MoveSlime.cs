using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSlime : MoveEnemy
{
    #region MoveState
    private class MoveState : BaseMoveState
    {
        public static int FindMetal { get; private set; } = 0;
        public static int FindPlayer { get; private set; } = 1;
        public static int BeenTrampled { get; private set; } = 2;
        public new static ushort StateNums;
        public new static Dictionary<string, int> States;
        public MoveState()
        {
            InitState();
        }
        public new void InitState()
        {
            base.InitState();
            if (States == null || States.Count != StateNums)
            {
                States = BaseMoveState.States;
                var properties = GetType().GetProperties();
                foreach (var prop in properties)
                {
                    if (prop.PropertyType == typeof(int))
                    {
                        int propNo = (int)prop.GetValue(null) + BaseMoveState.StateNums;
                        prop.SetValue(null, propNo);
                        if (!States.ContainsKey(prop.Name))
                        {
                            States.Add(prop.Name, propNo);
                        }
                    }
                }
                StateNums = (ushort)States.Count;
            }
        }
    }
    private MoveState moveState;
    #endregion
    protected override void Start()
    {
        moveState = new MoveState();
        foreach(var state in MoveState.States)
        {
            Debug.Log("state: " + state.Key + "no: " + state.Value);
        }
        base.Start();
    }
    protected override IEnumerator MovingImpl()
    {
        yield return null;
    }
}
