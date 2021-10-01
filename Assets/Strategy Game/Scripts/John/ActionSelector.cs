using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEditorInternal.VersionControl;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ActionSelector : MonoBehaviour
{
    private RectTransform rectTransform;
    
    [SerializeField] private RectTransform buttonPanel;
    private PlayerController player;
    public GameObject ActionButtonPrefab;
    private List<PlayerAction> availablePlayerActions = new List<PlayerAction>();
    public delegate void PlayerActionDelegate();
    
    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        rectTransform.anchoredPosition = Input.mousePosition;
        availablePlayerActions = player.AvailableActions(player.SelectedNode);
        for(int i = 0; i < availablePlayerActions.Count; i++)
        {
            GameObject instantiatedButtonObject = Instantiate(ActionButtonPrefab, buttonPanel);
            Button instantiatedButton = instantiatedButtonObject.GetComponent<Button>();
            instantiatedButton.onClick.AddListener(availablePlayerActions[i].PlayerExecuteAction);
            TextMeshProUGUI buttonText = instantiatedButtonObject.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = availablePlayerActions[i].ActionName;
        }
    }

    private void OnDisable()
    {
        foreach(RectTransform childTransform in buttonPanel)
        {
            Destroy(childTransform);
        }
    }
}
