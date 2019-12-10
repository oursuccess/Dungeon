using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IHandlePlayerHit
{
    // Start is called before the first frame update
    protected virtual void Start()
    {
    }

    public abstract void Effect(Player player);
    public abstract void OnPlayerHit(Player player);
}
