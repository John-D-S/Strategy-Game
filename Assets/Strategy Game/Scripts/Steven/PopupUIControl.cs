using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace StrategyGame.UI
{
    public class PopupUIControl : MonoBehaviour
    {
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
        // maybe a start button for begining of the game instructions?
    
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

        public void StartButton()
        {
            popupPanel.SetActive(false);
            Time.timeScale = 1;
        }

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