using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossCanvasController : MonoBehaviour
{
    #region Static
    public static CrossCanvasController Instance
    {
        get;
        private set;
    }
    #endregion

    #region Properties
    public GameObject ItemNumText { get; private set; }
    public GameObject DieText { get; private set; }
    public GameObject Guide { get; private set; }
    public GameObject DirectionText { get; private set; }
    #endregion

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ItemNumText = transform.Find("ItemNumText").gameObject;
        DieText = transform.Find("DieText").gameObject;
        Guide = transform.Find("Guide").gameObject;
        DirectionText = transform.Find("DirectionText").gameObject;
    }
}
