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
        player.ChangeState(MoveCharacter.MoveState.Crawl);
        player.MoveWithAnim(transform.position - player.transform.position, 1);
        Invoke("StartCrawl", player.moveTime);
    }
    private void StartCrawl()
    {
       transform.GetComponent<BoxCollider2D>().enabled = false;
        float realD = CalculatePlayerMoveDistance();
        player.ChangeGravity(0, (realD + 4f) * player.moveTime);
        player.ForceMove(new Vector2(0, realD), 1f);
        Invoke("PlayerMoveNext", realD * player.moveTime);
    }
    private void PlayerMoveNext()
    {
        player.MoveWithAnim(player.moveDir, 1f);
        Invoke("StopCrawl", player.moveTime);
    }
    private void StopCrawl()
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

        spineHeight = up.y - down.y + 0.2f;
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
        return spineHeight + playerH;
    }
}
