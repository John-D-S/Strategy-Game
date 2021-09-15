using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StrategyGame.UI
{
    /// <summary>
    /// Handles the enabling of the UI popup for user feedback and information.
    /// </summary>
    public class PopupUIControl : MonoBehaviour
    {
        /// <summary>
        /// Struct for the popup display data
        /// </summary>
        [Serializable]
        public struct PopUpStruct
        {
            public string title;
            [TextArea] public string description;
        }
        
        [Header("Popup Panel Elements")]
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Button startButton;
        [SerializeField] private Button retryButton;
        
    
        [Header("Messages")]
        [SerializeField] private PopUpStruct winPopup;
        [SerializeField] private PopUpStruct losePopup;
        [SerializeField] private PopUpStruct startPopup;
    
    
    
    
        // Start is called before the first frame update
        void Start()
        {
            if(startButton != null)
                startButton.onClick.AddListener(StartButton);
            ShowPopup(startPopup);
            startButton.enabled = true;
            retryButton.enabled = false;
        }

        /// <summary>
        /// Function for the start game button on the popup UI panel
        /// </summary>
        public void StartButton()
        {
            popupPanel.SetActive(false);
            Time.timeScale = 1;
        }
        
        /// <summary>
        /// Shows the popup on the cnavas with the passed data and pauses the timescale.
        /// </summary>
        /// <param name="_popUpStruct">The struct values to be displayed</param>
        public void ShowPopup(PopUpStruct _popUpStruct)
        {
            // Set the popup active and set the fields
            popupPanel.SetActive(true);
            startButton.enabled = false;
            retryButton.enabled = true;
            title.text = _popUpStruct.title;
            description.text = _popUpStruct.description;
            // Pause time
            Time.timeScale = 0;
        }
    }
}