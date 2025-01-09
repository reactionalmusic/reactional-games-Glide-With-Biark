using System;
using System.Collections;
using Reactional.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{ 
    private UIManager uIManager;
   public PlayerController controller;
   public GameObject player;
   private bool isGameOver = false;
   
   private int totalScore = 0;

   private void Awake()
   {
       controller = new PlayerController();
       uIManager = FindObjectOfType<UIManager>();
       
      
   }

   void Start()
   {
        //Time.timeScale = 0;
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
        isGameOver = true;
        Debug.Log("Game Over");
    }
    
    public void ReloadGame(InputAction.CallbackContext context)
    {
        if(isGameOver = true)
        {
            Debug.Log("Reload Game");
            SceneManager.LoadScene(0);
            
            // Set Player Visible Again
            gameObject.SetActive(true);
            StartCoroutine(PlayerOnDeath.Instance.SpawnPlayer(true, false));
            
            
        
            //TODO add gameover check here so it only works when game is over
        }
       
    }

    public void AddScore(int score)
    {
        totalScore += score;
        uIManager.AddScore(totalScore);
    }

    
   
    
    //--------------- Buttonklicks------------------
    
    private void OnEnable()
    {
        // Subscribe to the Fly action
        controller.UI.Submit.Enable();
        controller.UI.Submit.performed += ReloadGame;
        
        
    }

    private void OnDisable()
    {
        // Unsubscribe from the Fly action
        controller.UI.Submit.performed -= ReloadGame;
        controller.UI.Submit.Disable();
    }
    
    // --------------- Ienumerators -------------

    private IEnumerator WaitForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

}
