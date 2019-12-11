using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int Stage { get; private set; }

    private GameManager() { }
    public static GameManager Instance;

    void Start()
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

        Stage = 1;
    }

    public void GameOver()
    {
        ShowDieMessage();
    }
    private void ShowDieMessage()
    {
        CrossCanvasController.Instance.DieText.SetActive(true);
    }

    public void ToNextStage()
    {
        Stage++;
        SceneManager.LoadScene("Level" + Stage.ToString().PadLeft(2, '0'));
    }
}
