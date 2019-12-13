using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPig : Item, ICanSawPlayer, IHandlePlayerSought
{
    public void ILosePlayer(Player player)
    {
    }

    public void ISawPlayer(Player player)
    {
    }

    public void OnPlayerNotSeeing(Player player)
    {
        
    }

    public void OnPlayerSought(Player player)
    {

    }

    protected override void Start()
    {
        base.Start();

        OnItemDraged += WaitToSetDirection;
    }

    private void WaitToSetDirection(Item item)
    {
    }

}
