using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
    {
        [Header("Main UI Elements")]
        UIDocument uiDoc;
        VisualElement root;
        
        [Header("Startup UI Elements")]
        Button startButton;
        Button quitButton;
        VisualElement startupContainer;
        
        void Start()
        {
            uiDoc = GetComponent<UIDocument>();
            Debug.Log(uiDoc);
            
            SetupVisualElements();
            
        }

        void SetupVisualElements()
        {
            root = uiDoc.rootVisualElement;
            
            //Startup UI Elements
            startupContainer = root.Q<VisualElement>("StartupPage");
            startButton = startupContainer.Q<Button>("startup__start-button");
            Debug.Log(startButton);
            quitButton = startupContainer.Q<Button>("startup__quit-button");
            
            RegisterCallbacks();
            //UnregisterCallbacks();
        }

        void RegisterCallbacks()
        {
            startButton.RegisterCallback<ClickEvent>(ClickStartButton);
            quitButton.RegisterCallback<ClickEvent>(ClickQuitButton);
        }

        void UnregisterCallbacks()
        {
            startButton.UnregisterCallback<ClickEvent>(ClickStartButton);
            quitButton.UnregisterCallback<ClickEvent>(ClickQuitButton);
        }

        private void ClickStartButton(ClickEvent evt)
        {
            Debug.Log("Called ClickStartButton");
            startupContainer.style.display = DisplayStyle.None;
            FindFirstObjectByType<GameManager>().StartGame();
        }
        
        private static void ClickQuitButton(ClickEvent evt)
        {
            Application.Quit();
        }
    }