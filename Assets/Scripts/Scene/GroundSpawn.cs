using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject[] grounds;
    // Start is called before the first frame update
    void Start()
    {
        Camera camera = Camera.main;
        float xMin = camera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float xMax = camera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        float bottom = camera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + 0.5f;
        GameObject ground = grounds[0];
        float xSize = ground.GetComponent<BoxCollider2D>().size.x;

        for(var x = xMin; x < xMax; x += xSize)
        {
            ground = grounds[Random.Range(0, grounds.Length)];
            ground = Instantiate(ground, new Vector2(x, bottom), Quaternion.identity);
            ground.name = ground.name.Remove(ground.name.LastIndexOf('('));
        }
    }
}
