using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStab : Enemy, IHandlePlayerSought
{
    // Start is called before the first frame update
    protected override void Start()
    {

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Effect(Player player)
    {
        player.Dead();
    }

    public override void OnPlayerHit(Player player)
    {
        player.Dead();
    }

    public void OnPlayerSought(Player player)
    {
        Debug.Log("he saw me!");
    }
}
