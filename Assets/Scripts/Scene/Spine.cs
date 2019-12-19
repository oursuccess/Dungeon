using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spine : MonoBehaviour, IHandlePlayerHit
{
    private Player player;
    private float spineHeight;
    void Start()
    {
        FindHeight();
    }
    public void OnPlayerHit(Player player)
    {
        PreCrawl(player);
    }
    private void PreCrawl(Player player)
    {
        this.player = player;
        player.ChangeState(Player.MoveState.Crawl, Player.MoveState.Idle, 3f);
        player.MoveWithAnim(transform.position, 0.3f);
        Invoke("StartCrawl", player.moveTime);
    }
    private void StartCrawl()
    {
        transform.GetComponent<BoxCollider2D>().enabled = false;
        player.ChangeGravity(0, (spineHeight + 2) * player.moveTime);
        player.ForceMove(Vector2.up, spineHeight, 1f);
        Invoke("PlayerMoveNext", spineHeight * player.moveTime);
    }
    private void PlayerMoveNext()
    {
        player.MoveWithAnim(player.moveDir, player.moveDistance, player.velocity);
        Invoke("StopCrawl", player.moveTime);
    }
    private void StopCrawl()
    {
        transform.GetComponent<BoxCollider2D>().enabled = true;
    }
    private void FindHeight()
    {
        Vector2 start = transform.position;
        Vector2 findVUp = new Vector2(0, 0.03f);
        Collider2D target;
        while((target = Physics2D.Raycast(start, findVUp).collider) != null && target.gameObject == gameObject)
        {
            start += findVUp;
        }
        Vector2 up = start;
        start = transform.position;
        Vector2 findVDown = new Vector2(0, -0.03f);
        while((target = Physics2D.Raycast(start, findVDown).collider) != null && target.gameObject == gameObject)
        {
            start += findVDown;
        }
        Vector2 down = start;

        spineHeight = up.y - down.y;
    }
}
