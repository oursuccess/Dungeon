using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChest : MonoBehaviour, IHandlePlayerHit
{
    Animator animator;
    public void OnPlayerHit(Player player)
    {
        animator.enabled = true;
        animator.SetTrigger("Open");

        player.Won();
    }

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.enabled = false;
    }

}
