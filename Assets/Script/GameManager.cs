using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Game Manager to manage game states and score accumulation.
/// </summary>

public class GameManager : MonoBehaviour
{ 
    private UIManager _uIManager;
    public PlayerController Controller;
    public GameObject player;
    private bool _isGameOver = false;
    
    public int totalScore = 0;

    private void Awake()
    {
        Controller = new PlayerController();
        _uIManager = FindFirstObjectByType<UIManager>();
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        player.SetActive(true);
        
        Reactional.Playback.Playlist.Random();
    }

    public void PauseGame(bool isPaused)
    {
        player.SetActive(!isPaused);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        _isGameOver = true;
        Debug.Log("Game Over");
    }
    
    public void ReloadGame(InputAction.CallbackContext context)
    {
        if (!_isGameOver) return;
        
        SceneManager.LoadScene(0);
            
        // Set Player Visible Again
        gameObject.SetActive(true);
        StartCoroutine(PlayerOnDeath.Instance.SpawnPlayer(true, false));
        
        //TODO add gameover check here so it only works when game is over
    }

    public void AddScore(int score)
    {
        totalScore = Mathf.Max(0, totalScore + score);
        _uIManager.AddScore(totalScore);
    }
    
    //--------------- ButtonClicks------------------
    
    private void OnEnable()
    {
        // Subscribe to the Fly action
        Controller.UI.Submit.Enable();
        Controller.UI.Submit.performed += ReloadGame;
    }

    private void OnDisable()
    {
        // Unsubscribe from the Fly action
        Controller.UI.Submit.performed -= ReloadGame;
        Controller.UI.Submit.Disable();
    }
    
}
