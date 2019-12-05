using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    private string level;
    public void OnScenePressed()
    {
        level = "Level" + GetComponentInChildren<Text>().text;
        SceneManager.LoadScene(level);
    }
}
