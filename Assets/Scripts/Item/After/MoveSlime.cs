using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSlime : MoveEnemy , ICanFindThings, IHandlePlayerHit, IHandlePlayerSought
{
    #region MoveState
    private class MoveState : BaseMoveState
    {
        public static int FindPlayer { get; private set; } = 0;
        public static int FindMetal { get; private set; } = 1;
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
    private void ChangeState(int state)
    {
        moveState.ChangeState(state);
    }
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
        if(moveState.currState == MoveState.Idle || moveState.currState == MoveState.Move)
        {
            var player = Find<Player>();
            if (player != null)
            {
                ChangeState(MoveState.FindPlayer);
                Move(player.transform.position - transform.position);
                yield return null;
            }
            var metal = Find<IMadeByMetal>();
            if(metal != null)
            {
                ChangeState(MoveState.FindMetal);
                Move(metal.transform.position - transform.position);
                yield return null;
            }
        }
        yield return null;
    }
    public GameObject Find<T>()
    {
        GameObject res = null;
        //查找相关内容
        return res;
    }
    public void OnPlayerHit(Player player)
    {
        var playerCollider = player.gameObject.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollider, gameObject.GetComponent<BoxCollider2D>());
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Effect(collision.gameObject);
        }
        base.OnCollisionEnter2D(collision);
    }
    public void Effect(GameObject target)
    {
        transform.position = target.transform.position;
        //应该由对象选择如何失效
        Destroy(target.GetComponent<BoxCollider2D>());
        enabled = false;
    }
}
