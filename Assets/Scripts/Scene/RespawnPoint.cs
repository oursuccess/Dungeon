using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour ,IHandlePlayerHit
{
    public void OnPlayerHit(Player player)
    {
        GameManager.Instance.UpdateRespawnPosition(transform.position);
        //播放音效和动画
        Debug.Log("updated position");
    }
}
