using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BasicMenuFramework.Core
{
    /// <summary>
    /// The MenuControl class handles all the UI functionality for scene management, pause and quit.
    /// </summary>
    public class MenuControl : MonoBehaviour
    {
        [Header("Menu Buttons"),Tooltip("The quit button within the scene")]
        [SerializeField] private Button quitButton;
        [Tooltip("The pause button within the scene")]
        [SerializeField] private Button pauseButton;

        public bool showPopUp = false;
        [SerializeField,Tooltip("The GameObject that will appear when pause is pressed.")] 
        private GameObject popUp;
        
        [Tooltip("Buttons and their corresponding scenes to load when pressed.")]
        public List<SceneLoadButton> sceneLoadButtons = new List<SceneLoadButton>();
        
        private bool isPaused = false;
        
        /// <summary>
        /// This contains the data needed for the Scene Load Buttons list to function.
        /// </summary>
        [Serializable]
        public class SceneLoadButton
        {
            [Header("Button and Scene to load"), Tooltip("Assign a button")]
            public Button button;
            [Tooltip("Assign the scene for the button to load when clicked")]
            public string sceneToLoad;

            /// <summary>
            /// Loads the set scene in the class.
            /// </summary>
            public void LoadScene()
            {
                if(sceneToLoad != null)
                    SceneManager.LoadScene(sceneToLoad);
                else
                    throw new NullReferenceException($"No corresponding Scene to Load has been set in the Inspector for this Button.");
            }
        }

 

        // Start is called before the first frame update
        void Start()
        {
            // Adding listners to the UI buttons.
            if(quitButton != null)
                quitButton.onClick.AddListener(QuitGame);
            if(pauseButton != null)
                pauseButton.onClick.AddListener(Pause);

            if(popUp != null)
                popUp.SetActive(false);
            
            // Asigning the load scene method to each button in sceneLoadButtons.
            for(int i = 0; i < sceneLoadButtons.Count; i++)
            {
                if(sceneLoadButtons[i].sceneToLoad != null)
                    sceneLoadButtons[i].button.onClick.AddListener(sceneLoadButtons[i].LoadScene);
                else
                    throw new NullReferenceException($"No corresponding Scene to Load has been set in the Inspector for {sceneLoadButtons[i].button.name}.");
            }
            
        }

        
        /// <summary>
        /// Handles Pause functionality for the UI button.
        /// </summary>
        public void Pause()
        {
            if(!isPaused)
            {
                Time.timeScale = 0;
                isPaused = true;
                if(showPopUp && popUp != null)
                    popUp.SetActive(true);
                if(showPopUp && popUp == null)
                    Debug.LogError("No object has been set as the Pop Up in the Inspector.");
                
            }
            else if(isPaused)
            {
                Time.timeScale = 1;
                isPaused = false;
                if(showPopUp && popUp != null)
                    popUp.SetActive(false);
                if(showPopUp && popUp == null)
                    Debug.LogError("No object has been set as the Pop Up in the Inspector.");
            }
        }
        
        /// <summary>
        /// Quits from both the Play Mode in the Unity Editor and the Built Application.
        /// </summary>
        public void QuitGame()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }

    }
}