using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
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
        Init();
    }
    #endregion
    private void Init()
    {
        InitState();
    }
    #region PlayerDied
    public GameObject player { get; private set; }
    public Vector2 RespawnPosition { get; private set; }
    public void Restart()
    {
        var oldOne = player;
        var camManager = Camera.main.GetComponent<CameraController>();
        camManager.ChangeCameraPosition(RespawnPosition);
        player = Instantiate(player, RespawnPosition, Quaternion.identity);
        camManager.ChangeFollow(player);
        Destroy(oldOne);
    }
    private void InitPlayer()
    {
        player = GameObject.Find("Player");
        RespawnPosition = player.transform.position;
    }
    public void UpdateRespawnPosition(Vector2 newPos)
    {
        RespawnPosition = newPos;
    }
    public void GameOver()
    {
        ShowDieMessage();
    }
    private void ShowDieMessage()
    {
        CrossCanvasController.Instance.DieText.SetActive(true);
    }
    #endregion
    #region Stage
    public int Stage { get; private set; }
    private void InitState()
    {
        Stage = 1;
    }
    public void ToNextStage()
    {
        SceneManager.sceneLoaded += OnStageLoaded;
        Stage++;
        SceneManager.LoadScene("Level" + Stage.ToString().PadLeft(2, '0'));
    }
    private void OnStageLoaded(Scene scene, LoadSceneMode mode)
    {
        InitPlayer();
        SceneManager.sceneLoaded -= OnStageLoaded;
    }
    #endregion
}
