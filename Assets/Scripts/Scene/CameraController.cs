﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region FollowTarget
    [SerializeField]
    [Tooltip("镜头跟随对象")]
    private GameObject target;
    [SerializeField]
    [Tooltip("镜头跟随对象在屏幕中的位置 ")]
    private Vector3 offset;
    #endregion
    #region ScreenSize
    public float xSizeMin { get; private set;}
    public float xSizeMax { get; private set;}
    public float ySizeMin { get; private set; }
    public float ySizeMax { get; private set; }
    #endregion
    #region delagte
    public delegate void CameraPositionChange();
    public event CameraPositionChange OnCameraPositionChanged;
    #endregion
    void Start()
    {
        xSizeMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        xSizeMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        ySizeMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        ySizeMax = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;
    }
    private void ChangeCameraPosition(MoveObject moveObject)
    {
        xSizeMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        xSizeMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
        ySizeMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        ySizeMax = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;

        transform.position = target.transform.position + offset;
        OnCameraPositionChanged?.Invoke();
    }
    private void ToggleOffFollw(MoveObject target)
    {
        target.GetComponent<MoveObject>().Moving -= ChangeCameraPosition;
        target.GetComponent<MoveObject>().OnDead -= ToggleOffFollw;
    }
    private void StartFollow(MoveObject target)
    {
        target.GetComponent<MoveObject>().Moving += ChangeCameraPosition;
        target.GetComponent<MoveObject>().OnDead += ToggleOffFollow;
    }
}
