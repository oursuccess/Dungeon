using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSlime : MoveItem, ICanFindThings 
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
    private void ChangeState(int state)
    {
        moveState.ChangeState(state);
    }
    #endregion
    protected override void Start()
    {
        moveState = new MoveState();
        base.Start();

        StartMove();
    }
    protected override IEnumerator MovingImpl()
    {
        GameObject target = null;
        while (true)
        {
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
           }
            if (moveState.currState == MoveState.FindPlayer)
            {
                Move(target.transform.position - transform.position);
                if (Find<Player>() == null)
                {
                    ChangeState(MoveState.LosePlayer);
                }
                yield return null;
            }
            if (moveState.currState == MoveState.FindMetal)
            {
                Move(target.transform.position - transform.position);
                if (Find<IMadeByMetal>() == null)
                {
                    ChangeState(MoveState.LoseMetal);
                }
                yield return null;
            }
            if (moveState.currState == MoveState.Fall)
            {
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
            yield return null;
        }
    }
    private void LoseMetal()
    {
    }
    private void LosePlayer()
    {
    }
    public GameObject Find<T>()
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
    public void OnPlayerHit(Player player)
    {
        var playerCollider = player.gameObject.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollider, gameObject.GetComponent<BoxCollider2D>());
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
}
