using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneSelector : MonoBehaviour
{
    private string level;
    public void OnScenePressed()
    {
        level = "Level" + GetComponentInChildren<Text>().text;
        GameManager.Instance.ToLevel(level);
    }
}
