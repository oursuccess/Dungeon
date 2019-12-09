using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Text dieText;

    private GameManager() { }
    public static GameManager Instance;
    // Start is called before the first frame update
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

    }

    public void GameOver()
    {
        ShowDieMessage();
    }
    private void ShowDieMessage()
    {
        dieText.gameObject.SetActive(true);
    }

}
