using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEditorInternal.VersionControl;

using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

[RequireComponent(typeof(RectTransform))]
public class ActionSelector : MonoBehaviour
{
    private RectTransform rectTransform;
    
    [SerializeField] private RectTransform buttonPanel;
    private PlayerController player;
    [Tooltip("The button used to select the action")] public GameObject ActionButtonPrefab;
    private List<PlayerAction> availablePlayerActions = new List<PlayerAction>();
    public delegate void PlayerActionDelegate();

    private Canvas rootCanvas;
    
    private void Start()
    {
        //initializes all the variables
        rootCanvas = GetComponentInParent<Canvas>();
        player = FindObjectOfType<PlayerController>();
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// moves the recttransform to the cursor's position when enabled and adds a button for each action
    /// </summary>
    private void OnEnable()
    {
        if(!player || !rectTransform)
        {
            player = FindObjectOfType<PlayerController>();
            rectTransform = GetComponent<RectTransform>();
            rootCanvas = GetComponentInParent<Canvas>();
        }
        rectTransform.anchoredPosition = Input.mousePosition / rootCanvas.scaleFactor;
        availablePlayerActions = player.AvailableActions(player.SelectedNode);
        if(player.actionsDoneThisTurn.Count < player.ActionsAtStartOfTurn + player.additionalActions)
        {
            for(int i = 0; i < availablePlayerActions.Count; i++)
            {
                GameObject instantiatedButtonObject = Instantiate(ActionButtonPrefab, buttonPanel);
                Button instantiatedButton = instantiatedButtonObject.GetComponent<Button>();
                instantiatedButton.onClick.AddListener(availablePlayerActions[i].PlayerExecuteAction);
                TextMeshProUGUI buttonText = instantiatedButtonObject.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = availablePlayerActions[i].ActionName;
            }
        }
    }

    /// <summary>
    /// destroys all the buttons when disabled
    /// </summary>
    private void OnDisable()
    {
        foreach(RectTransform childTransform in buttonPanel)
        {
            Destroy(childTransform.gameObject);
        }
    }
}
