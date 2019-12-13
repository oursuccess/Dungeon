using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    private Vector3 offset;
    public float xSizeMin { get; private set;}
    public float xSizeMax { get; private set;}
    public float ySizeMin { get; private set; }
    public float ySizeMax { get; private set; }

    public delegate void CameraPositionChange();
    public event CameraPositionChange OnCameraPositionChanged;

    void Start()
    {
        offset = transform.position - target.transform.position;

        target.GetComponent<MoveObject>().Moving += ChangeCameraPosition;
        
        xSizeMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        xSizeMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        ySizeMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        ySizeMax = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
    }

    void ChangeCameraPosition(MoveObject moveObject)
    {
        xSizeMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        xSizeMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        ySizeMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        ySizeMax = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;

        transform.position = target.transform.position + offset;
        OnCameraPositionChanged?.Invoke();
    }
}
