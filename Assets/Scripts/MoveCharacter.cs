using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MoveObject
{
    [SerializeField]
    protected float sight = 1;
    [SerializeField]
    protected LayerMask layer;
    Animator animator;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        
        base.Start();
    }

    protected override void SimpleAutoMove(Vector2 direction)
    {
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        base.SimpleAutoMove(direction);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected bool Find(Vector2 direction, LayerMask layer, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + direction * sight;
        hit = Physics2D.Linecast(transform.position, end, layer);
        if(hit.transform != null)
        {
            return true;
        }
        return false;
    }

}
