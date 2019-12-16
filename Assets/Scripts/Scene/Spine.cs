using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spine : MonoBehaviour, IHandlePlayerHit
{
    private Player player;
    private float height;
    void Start()
    {
        FindHeight();
    }
    public void OnPlayerHit(Player player)
    {
        StartCrawl(player);
        Invoke("StopPlayerHit", 1f);
    }
    private void StartCrawl(Player player)
    {
        this.player = player;
        player.ChangeState(MoveCharacter.MoveState.Crawl);
        transform.GetComponent<BoxCollider2D>().enabled = false;
        float realD = CalculatePlayerMoveDistance();
        player.ForceMove(new Vector2(0, realD), 1f);
        Invoke("PlayerMoveNext", realD);
    }
    private void PlayerMoveNext()
    {
        player.MoveWithAnim(player.moveDir, 1f);
        Invoke("StopPlayerCrawl", 1f);
    }
    private void StopPlayerCrawl()
    {
        player.ChangeState(MoveCharacter.MoveState.Idle);
        transform.GetComponent<BoxCollider2D>().enabled = true;
    }
    private void FindHeight()
    {
        Vector2 start = transform.position;
        Vector2 findVUp = new Vector2(0, 0.1f);
        Collider2D target;
        while((target = Physics2D.Raycast(start, findVUp).collider) != null && target.gameObject == gameObject)
        {
            start += findVUp;
        }
        Vector2 up = start;
        start = transform.position;
        Vector2 findVDown = new Vector2(0, -0.1f);
        while((target = Physics2D.Raycast(start, findVDown).collider) != null && target.gameObject == gameObject)
        {
            start += findVDown;
        }
        Vector2 down = start;

        height = up.y - down.y + 0.1f;
    }
    private float CalculatePlayerMoveDistance()
    {
        float playerH;
        Collider2D target;
        Vector2 down = player.transform.position;
        Vector2 findVDown = new Vector2(0, -0.1f);
        while((target = Physics2D.Raycast(down , findVDown).collider) != null && target.gameObject == player.gameObject)
        {
            down += findVDown;
        }
        playerH = player.transform.position.y - down.y;
        return height - playerH;
    }
}
