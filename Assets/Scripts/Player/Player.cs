using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MoveCharacter
{
    private float animSizeXRatio = 0.1f;
    private float animSizeYRatio = 0.1f;

    [SerializeField]
    private Image emote;
    private Image playerEmote;

    protected override void Start()
    {
        base.Start();

        if(emote != null)
        {
            playerEmote = Instantiate<Image>(emote, emote.transform.parent);
            playerEmote.gameObject.SetActive(false);
        }

        //to clean
        gameObject.transform.localScale = new Vector2(animSizeXRatio, animSizeYRatio);

        canMove = true;
        animator.enabled = false;
        moveDir = Vector2.right;
    }

    protected override void Update()
    {
        RaycastHit2D hit;
        if (Find(moveDir, layer, out hit) || Find(new Vector3(transform.position.x, Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + 0.8f),moveDir, layer, out hit))
        {

            //看到敌人后的表情变化
            playerEmote.transform.position = transform.position;
            playerEmote.gameObject.SetActive(true);

            var hitTarget = hit.transform.gameObject.GetComponent<IHandlePlayerSought>();
            if(hitTarget != null)
            {
                hitTarget.OnPlayerSought(this);
            }
        }
    }

    protected override void SimpleAutoMove(Vector2 direction)
    {
        base.SimpleAutoMove(direction);
    }

    public override void StopMove()
    {
        canMove = false;
        base.StopMove();
    }

    public override void StartMove()
    {
        canMove = true;
        rigidBody2D.gravityScale = 1;
    }

    public override void Dead()
    {
        base.Dead();

        //to enable
        Invoke("GameOver", 1f);
    }

    private void GameOver()
    {
        //GameManager.Instance.GameOver();
    }

    public void Won()
    {
        StopMove();
        Invoke("GameWon", 1f);
    }

    private void GameWon()
    {
        GameManager.Instance.ToNextStage();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        IHandlePlayerHit hit = collision.transform.GetComponent<IHandlePlayerHit>();
        if (hit != null)
        {
            hit.OnPlayerHit(this);
        }
        else if (canMove && collision.transform.name.Contains("Ground"))
        {
            animator.enabled = true;
            SimpleAutoMove(moveDir);
        }

        base.OnCollisionEnter2D(collision);
    }
}
