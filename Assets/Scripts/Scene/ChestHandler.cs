using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestHandler : MonoBehaviour
{
    [SerializeField]
    private float xPos = 27f;
    void Start()
    {
        transform.position = new Vector3(xPos, Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + 1f);
    }

    void Update()
    {
        
    }
}
