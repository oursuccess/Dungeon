using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    private Vector3 offset;

    public delegate void CameraPositionChange();
    public event CameraPositionChange OnCameraPositionChanged;

    void Start()
    {
        offset = transform.position - target.transform.position;

        target.GetComponent<MoveObject>().OnMoving += ChangeCameraPosition;
    }

    void ChangeCameraPosition()
    {
        transform.position = target.transform.position + offset;
        OnCameraPositionChanged?.Invoke();
    }
}
