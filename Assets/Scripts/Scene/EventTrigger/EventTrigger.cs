using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventTrigger : MonoBehaviour
{
    #region Init
    protected virtual void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        var collider = gameObject.GetComponent<BoxCollider2D>();
        if(collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
    }
    #endregion

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<Player>();
        if (player != null)
        {
            EventImpl(player);
        }
    }
    protected abstract void EventImpl(Player player);
}
