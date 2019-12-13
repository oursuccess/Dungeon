using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InScreenController : MonoBehaviour
{
    //控制敌人行动与否，检测敌人是否处在视线中
    private BoxCollider2D trigger;

    void Start()
    {
        SetTrigger();
    }
    private void SetTrigger()
    {
        trigger = gameObject.GetComponent<BoxCollider2D>();

        Camera cam = Camera.main;
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        int xSize = Mathf.CeilToInt(Mathf.Max(xPos - cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x, cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - xPos));
        int ySize = Mathf.CeilToInt(Mathf.Max(yPos - cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y, cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - yPos));

        trigger.size = new Vector2(xSize, ySize);
    }
}
