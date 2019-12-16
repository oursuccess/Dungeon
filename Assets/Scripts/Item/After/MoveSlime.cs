using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSlime : MoveItem, IHaveTrampleEffect
{
    #region MoveState
    private class MoveState : BaseMoveState
    {
        public static int FindPlayer { get; private set; } = 0;
        public static int FindMetal { get; private set; } = 1;
        public static int BeenTrampled { get; private set; } = 2;
        public static int LosePlayer { get; private set; } = 3;
        public static int LoseMetal { get; private set; } = 4;
        public static ushort StateNums;
        public static Dictionary<string, int> States;
        public MoveState()
        {
            InitState();
            currState = MoveState.Idle;
            prevState = MoveState.Idle;
        }
        public void InitState()
        {
            if (States == null || States.Count != StateNums)
            {
                States = new Dictionary<string, int>();
                var baseProperties = GetType().BaseType.GetProperties();
                foreach(var baseProp in baseProperties) {
                    if(baseProp.PropertyType == typeof(int))
                    {
                        if (!States.ContainsKey(baseProp.Name))
                        {
                            States.Add(baseProp.Name, (int)baseProp.GetValue(null));
                        }
                    }
                    StateNums = (ushort)States.Count;
                }
                var properties = GetType().GetProperties();
                foreach (var prop in properties)
                {
                    if (prop.PropertyType == typeof(int))
                    {
                        int propNo = (int)prop.GetValue(null) + StateNums;
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
    public void ChangeState(int state)
    {
        moveState.ChangeState(state);
    }
    #endregion
    protected override void Start()
    {
        moveState = new MoveState();
        base.Start();
    }
    #region Move
    public override void StartMove()
    {
        base.StartMove();
    }
    public override void StopMove(float stopTime = 0)
    {
        base.StopMove(stopTime);
    }
    protected override IEnumerator MovingImpl()
    {
        GameObject target = null;
        while (true)
        {
            if(lastMoveTime <= moveTime)
            {
                lastMoveTime += Time.deltaTime;
            }
            else
            {
                lastMoveTime = 0f;
                #region WhetherOnAir
                target = FindAnythingOnDirection(Vector2.down, 0.7f);
                if (target == null)
                {
                    ChangeState(MoveState.Fall);
                }
                #endregion
                #region WhetherBeenTrampled
                if (moveState.currState != MoveState.Fall)
                {
                    target = FindAnythingOnDirection(Vector2.up, 1.1f);
                    if (target != null)
                    {
                        ChangeState(MoveState.BeenTrampled);
                    }
                }
                #endregion
                if (moveState.currState == MoveState.Idle || moveState.currState == MoveState.Move)
                {

                    target = Find<Player>();
                    if (target != null)
                    {
                        ChangeState(MoveState.FindPlayer);
                        yield return null;
                    }
                    else
                    {
                        target = Find<IMadeByMetal>();
                        if (target != null)
                        {
                            ChangeState(MoveState.FindMetal);
                            yield return null;
                        }
                    }
                    if (target == null)
                    {
                        if (UnityEngine.Random.Range(0, 100) <= moveWill)
                        {
                            Move();
                            ChangeState(MoveState.Move);
                            yield return null;
                        }
                        else
                        {
                            ChangeState(MoveState.Idle);
                            yield return null;
                        }
                    }
                }
                if (moveState.currState == MoveState.FindPlayer)
                {
                    MoveNChangeDirection(target.transform.position - transform.position);
                    if (Find<Player>() == null)
                    {
                        ChangeState(MoveState.LosePlayer);
                }
                yield return null;
                }
                if (moveState.currState == MoveState.FindMetal)
                {
                    MoveNChangeDirection(target.transform.position - transform.position);
                    if (Find<IMadeByMetal>() == null)
                    {
                        ChangeState(MoveState.LoseMetal);
                    }
                    yield return null;
                }
                if (moveState.currState == MoveState.LoseMetal)
                {
                    LoseMetal();
                    ChangeState(MoveState.Idle);
                    yield return null;
                }
                if (moveState.currState == MoveState.LosePlayer)
                {
                    LosePlayer();
                    ChangeState(MoveState.Idle);
                    yield return null;
                }
                if (moveState.currState == MoveState.Fall)
                {
                    target = FindAnythingOnDirection(Vector2.down, 0.6f);
                    if (target != null)
                    {
                        ChangeState(MoveState.Idle);
                    }
                    else
                    {
                        Move(new Vector2(moveDir.x, -1));
                    }
                    yield return null;
                }
                if (moveState.currState == MoveState.BeenTrampled)
                {
                    target = FindAnythingOnDirection(Vector2.up, 1f);
                    if (target == null)
                    {
                        ChangeState(MoveState.Idle);
                    }
                }
            }
            yield return null;
        }
    }
    private void LoseMetal()
    {
    }
    private void LosePlayer()
    {
    }
    #endregion
    public void OnPlayerHit(Player player)
    {
        var playerCollider = player.gameObject.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollider, gameObject.GetComponent<BoxCollider2D>());
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
    public void OnBeenTrampled(MoveObject moveObj)
    {
        moveObj.ForceMove(new Vector2(moveObj.moveDir.x == 0 ? moveObj.moveDir.x : 1, 1), 10);
    }
}
