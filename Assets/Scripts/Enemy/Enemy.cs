using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IHandlePlayerHit
{
    // Start is called before the first frame update
    protected virtual void Start()
    {
        transform.position = new Vector2(transform.position.x, Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + 0.7f);
    }

    public abstract void Effect(Player player);
    public abstract void OnPlayerHit(Player player);
}
