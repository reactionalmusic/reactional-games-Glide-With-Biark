using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Main UI Elements")]
    UIDocument uiDoc;
    VisualElement root;
    
    [Header("Startup UI Elements")]
    Button startButton;
    Button quitButton;
    VisualElement startupContainer;
    
    [Header("Ingame UI Elements")]
    Label pointsLabel;
    Button pauseButton;
    VisualElement ingameContainer;

    [Header("Pause UI Elements")] 
    Label pausePointsLabel;
    Button resumeButton;
    Button restartButton;
    Button pauseQuitButton;
    VisualElement pauseContainer;
    
    [Header("Misc")] 
    private static AudioClip clickSound;
    private static AudioSource audioSource;
    
    bool isPaused = false;
    public PlayerController controller;
    [SerializeField] GameManager gameManager;
    public ReactionalDeepAnalysisProceduralMapGenerator proceduralMapGenerator;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        clickSound = Resources.Load<AudioClip>("BiarkButtonclickSound");
        audioSource.clip = clickSound;
        uiDoc = GetComponent<UIDocument>();
       
        SetupVisualElements();
        EnableController();
    }
    
    //------------------------------- Controller -------------------------------
    private void EnableController()
    {
        // Subscribe to the controller action
        gameManager.Controller.UI.Pause.Enable();
        gameManager.Controller.UI.Pause.performed += OnPause;
        
        gameManager.Controller.UI.Start.Enable();
        gameManager.Controller.UI.Start.performed += OnStart;
    }

    private void OnDisable()
    {
        // Unsubscribe from the controler action
        gameManager.Controller.UI.Pause.performed -= OnPause;
        gameManager.Controller.UI.Pause.Disable();
        
        gameManager.Controller.UI.Start.performed -= OnStart;
        gameManager.Controller.UI.Start.Disable();
    }

    //---------------------------------------- Callbacks -------------------------------
    
    void OnDestroy()
    {
        UnregisterCallbacks();
    }

    void SetupVisualElements()
    {
        root = uiDoc.rootVisualElement;
        
        //Startup UI Elements
        startupContainer = root.Q<VisualElement>("StartupPage");
        startButton = startupContainer.Q<Button>("startup__start-button");
        quitButton = startupContainer.Q<Button>("startup__quit-button");
        
        //Ingame UI Elements
        ingameContainer = root.Q<VisualElement>("IngamePage");
        pointsLabel = ingameContainer.Q<Label>("ingame__points-label");
        pauseButton = ingameContainer.Q<Button>("ingame__pause-button");
        
        //Pause UI Elements
        pauseContainer = root.Q<VisualElement>("PausePage"); 
        pausePointsLabel = pauseContainer.Q<Label>("pause__points-label");
        resumeButton = pauseContainer.Q<Button>("pause__resume-button");
        restartButton = pauseContainer.Q<Button>("pause__restart-button");
        pauseQuitButton = pauseContainer.Q<Button>("pause__quit-button");
        
        RegisterCallbacks();
    }

    void RegisterCallbacks()
    {
        //Startup UI Elements
        startButton.RegisterCallback<ClickEvent>(ClickStartButton);
        quitButton.RegisterCallback<ClickEvent>(ClickQuitButton);
        
        //Ingame UI Elements
        pauseButton.RegisterCallback<ClickEvent>(TogglePause);
        
        //Pause UI Elements
        resumeButton.RegisterCallback<ClickEvent>(TogglePause);
        restartButton.RegisterCallback<ClickEvent>(ClickRestartButton);
        pauseQuitButton.RegisterCallback<ClickEvent>(ClickQuitButton);
    }

    void UnregisterCallbacks()
    {
        startButton.UnregisterCallback<ClickEvent>(ClickStartButton);
        quitButton.UnregisterCallback<ClickEvent>(ClickQuitButton);
        
        
        //remove?
        pauseButton.UnregisterCallback<ClickEvent>(TogglePause);
        
        resumeButton.UnregisterCallback<ClickEvent>(TogglePause);
        restartButton.UnregisterCallback<ClickEvent>(ClickRestartButton);
        pauseQuitButton.UnregisterCallback<ClickEvent>(ClickQuitButton);
    }

    private void ClickStartButton(ClickEvent evt)
    {
        audioSource.PlayOneShot(clickSound);
        
        startupContainer.style.display = DisplayStyle.None;
        ingameContainer.style.display = DisplayStyle.Flex;
        FindFirstObjectByType<GameManager>().StartGame();
        
        StartCoroutine(proceduralMapGenerator.SpawnSongs());
        
        pointsLabel.text = "Points: " + 0;
        
        //TODO FIX THIS SO IT DESOLVES IN ON SPAWN
        StartCoroutine(PlayerOnDeath.Instance.SpawnPlayer(true, false));
    }
    
    
    //------------------------------------ Buttons ----------------------------

   
    
    private static void ClickQuitButton(ClickEvent evt)
    {
        audioSource.PlayOneShot(clickSound);
        Application.Quit();
    }
    
    private static void ClickRestartButton(ClickEvent evt)
    {
        audioSource.PlayOneShot(clickSound);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private void TogglePause(ClickEvent evt)
    {
        
        audioSource.PlayOneShot(clickSound);
        if (isPaused)
        {   
            //Unpause
            pauseContainer.style.display = DisplayStyle.None;
            ingameContainer.style.display = DisplayStyle.Flex;
            Time.timeScale = 1;
            isPaused = false;
            Reactional.Setup.AllowPlay = true;
        }
        else
        {
            //Pause
            pauseContainer.style.display = DisplayStyle.Flex;
            ingameContainer.style.display = DisplayStyle.None;
            pausePointsLabel.text = pointsLabel.text;
            Time.timeScale = 0;
            isPaused = true;
            Reactional.Setup.AllowPlay = false;
        }
        FindFirstObjectByType<GameManager>().PauseGame(isPaused);
    }
    
    
    /// <summary>
    /// Press Escape to Pause
    /// </summary>
    /// <param name="context"></param>
    private void OnPause(InputAction.CallbackContext context)
    {
        audioSource.PlayOneShot(clickSound);
        TogglePause(null);
    }
    
    /// <summary>
    /// Press Enter To Start
    /// </summary>
    /// <param name="context"></param>
    private void OnStart(InputAction.CallbackContext context)
    {
        audioSource.PlayOneShot(clickSound);
        startupContainer.style.display = DisplayStyle.None;
        ingameContainer.style.display = DisplayStyle.Flex;
        FindFirstObjectByType<GameManager>().StartGame();
        
        pointsLabel.text = "Points: " + 0;
    }

    public void AddScore(int score)
    {
        pointsLabel.text = "Points: " + score;
    }
}