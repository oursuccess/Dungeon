using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveItem : MoveCreature 
{
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
