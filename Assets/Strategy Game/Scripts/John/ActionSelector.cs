using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Button = UnityEngine.UIElements.Button;

[RequireComponent(typeof(RectTransform))]
public class ActionSelector : MonoBehaviour
{
    private RectTransform rectTransform;
    
    [SerializeField] private RectTransform buttonPanel;
    private PlayerController player;
    public GameObject ActionButtonPrefab;
    private List<PlayerAction> availablePlayerActions = new List<PlayerAction>();

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        rectTransform.anchoredPosition = Input.mousePosition;
        GameObject instantiatedButtonObject = Instantiate(ActionButtonPrefab, buttonPanel);
        Button instantiatedButton = instantiatedButtonObject.GetComponent<Button>();
        TextMeshProUGUI buttonText = instantiatedButtonObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnDisable()
    {
        foreach(RectTransform childTransform in buttonPanel)
        {
            Destroy(childTransform);
        }
    }
}
