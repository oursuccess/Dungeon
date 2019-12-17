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
    #region Player
    [SerializeField]
    [Tooltip("玩家的预制体")]
    private GameObject playerPrefab;
    public GameObject player { get; private set; }
    public Vector2 RespawnPosition { get; private set; }
    public void Restart()
    {
        Destroy(player);
        var camManager = Camera.main.GetComponent<CameraController>();
        camManager.ChangeCameraPosition(RespawnPosition);
        player = Instantiate(playerPrefab, RespawnPosition, Quaternion.identity);
        player.name = playerPrefab.name;
        camManager.ChangeFollow(player);
    }
    private void InitPlayer()
    {
        player = GameObject.Find("Player");
        if(player != null)
        {
            RespawnPosition = player.transform.position;
        }
    }
    public void UpdateRespawnPosition(Vector2 newPos)
    {
        RespawnPosition = newPos;
    }
    public void GameOver()
    {
        Restart();
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
    public void ToNextLevel()
    {
        Stage++;
        ToLevel("Level" + Stage.ToString().PadLeft(2, '0'));
    }
    public void ToLevel(string Level)
    {
        SceneManager.sceneLoaded += OnStageLoaded;
        SceneManager.LoadScene(Level);
    }
    private void OnStageLoaded(Scene scene, LoadSceneMode mode)
    {
        InitPlayer();
        SceneManager.sceneLoaded -= OnStageLoaded;
    }
    #endregion
}
