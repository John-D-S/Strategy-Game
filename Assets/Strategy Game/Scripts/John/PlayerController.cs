using StrategyGame;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(NavGridAgent))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("The number of actions available to the player at the start of the turn")] private int actionsAtStartOfTurn = 5;
    public int ActionsAtStartOfTurn => actionsAtStartOfTurn;
    public int additionalActions;
    [SerializeField, Tooltip("The button which undoes the actions taken by the player")] private Button undoActionButton;
    [System.NonSerialized] public bool isPlayerTurn = true;
    [SerializeField, Tooltip("The text that displays the number of turns remaining")] private TextMeshProUGUI turnsRemainingDisplay;

    public static PlayerController thePlayerController;
    private NavGridAgent agent;
    public NavGridAgent PlayerAgent => agent;
    public NavGridNode CurrentNode => agent.CurrentNode;
    public List<NavGridNode> NeighboringNodes => CurrentNode.Neighbors;
    public List<Item> collectedItems = new List<Item>();
    public List<PlayerAction> actionsDoneThisTurn = new List<PlayerAction>();

    private NavGridNode selectedNode;
    public NavGridNode SelectedNode => selectedNode;

    /// <summary>
    /// Returns the list of neighboring nodes of the game object of any given node
    /// </summary>
    public Dictionary<GameObject, NavGridNode> NeighboringNodesByGameObject
    {
        get
        {
            Dictionary<GameObject, NavGridNode> returnValue = new Dictionary<GameObject, NavGridNode>();
            foreach(NavGridNode node in NeighboringNodes)
            {
                returnValue[node.gameObject] = node;
            }

            return returnValue;
        }
    }
    private ActionSelector actionSelector;

    [SerializeField] private AudioRunner audioRunner;

    /// <summary>
    /// the list of actions available for the player to do
    /// </summary>
    public List<PlayerAction> AvailableActions(NavGridNode _node)
    {
        List<PlayerAction> returnValue = new List<PlayerAction>();
        returnValue.Add(new Move(_node, this));
        foreach(Item collectedItem in collectedItems)
        {
            returnValue.Add(new PlaceItem(_node, this, collectedItem));
        }
        return returnValue;
    }

    /// <summary>
    /// Updates the materials of neighboring nodes of the player
    /// </summary>
    public void UpdateNeighborNodeMaterials()
    {
        foreach(NavGridNode navGridNode in CurrentNode.navGrid.AllNodes)
        {
            if(CurrentNode.Neighbors.Contains(navGridNode))
            {
                NodeMaterialSwitcher.MaterialSwitchersByNavGridNodes[navGridNode].SwitchToEnableMaterial();
            }
            else
            {
                NodeMaterialSwitcher.MaterialSwitchersByNavGridNodes[navGridNode].SwitchToDefaultMaterial();
            }
        }
    }
    
    /// <summary>
    /// tries to pick up the item on the node the player is currently standing on
    /// </summary>
    public Item TryPickupItem()
    {
        Item item = Item.ItemNearPosition(agent.AgentNavGrid.GridSize * 0.5f, agent.CurrentNode.transform.position);
        if(item)
        {
            audioRunner.effect.Play();
            item.PickUpItem(this);
        }
        return item;
    }

    /// <summary>
    /// undoes the picking up of an item, putting it back to where it started
    /// </summary>
    public void UndoPickupItem(Item _item)
    {
        _item.UndoPickUpItem(this);
    }
    
    private TurnManager turnManager;

    public void DoAction(NavGridNode _node, PlayerAction _playerAction)
    {
        if(actionsDoneThisTurn.Count < actionsAtStartOfTurn + additionalActions)
        {
            actionsDoneThisTurn.Add(_playerAction);
            actionsDoneThisTurn[actionsDoneThisTurn.Count - 1].PlayerExecuteAction();
        }
    }
    
    /// <summary>
    /// undoes the last action
    /// </summary>
    public void UndoAction()
    {
        HideActionSelector();
        if(actionsDoneThisTurn.Count > 0)
        {
            PlayerAction lastPlayerAction = actionsDoneThisTurn[actionsDoneThisTurn.Count - 1];
            lastPlayerAction.PlayerUndoAction();
            actionsDoneThisTurn.RemoveAt(actionsDoneThisTurn.Count - 1);
        }
    }

    /// <summary>
    /// ends the player's turn
    /// </summary>
    public void EndTurn()
    {
        if(isPlayerTurn)
        {
            actionsDoneThisTurn.Clear();
            additionalActions = 0;
            turnManager.StartNextTurn();
        }
    }

    /// <summary>
    /// shows the action selector
    /// </summary>
    public void ShowActionSelector()
    {
        actionSelector.gameObject.SetActive(true);
    }

    /// <summary>
    /// hides the action selector
    /// </summary>
    public void HideActionSelector()
    {
        actionSelector.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// shows the available player actions if it is the player's turn and the player clicks on one of the player character's neighboring nodes.
    /// </summary>
    private void UpdatePlayerActions()
    {
        if(isPlayerTurn)
        {
            if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();
                NavGridNode node = null;
                if(Physics.Raycast(mouseRay, out hit))
                {
                    Dictionary<GameObject, NavGridNode> currentNeighborNodesbyGo = NeighboringNodesByGameObject;
                    GameObject hitObject = hit.collider.gameObject;
                    if(currentNeighborNodesbyGo.ContainsKey(hitObject))
                    {
                        node = currentNeighborNodesbyGo[hitObject];
                    }
                }
                if(node && NeighboringNodes.Contains(node))
                {
                    selectedNode = node;
                    HideActionSelector();
                    ShowActionSelector();
                }
                else
                {
                    HideActionSelector();
                }
            }
        }
        else
        {
            HideActionSelector();
        }
    }

    private void FixedUpdate()
    {
        //update what is shown on the turns remaining display so that it shows the correct number of turns remaining
        if(turnsRemainingDisplay)
        {
            turnsRemainingDisplay.text = $"Actions Remaining: {(actionsAtStartOfTurn + additionalActions) - actionsDoneThisTurn.Count} / {(actionsAtStartOfTurn + additionalActions)}";
        }
    }

    private void Awake()
    {
        //initialize the playerController
        thePlayerController = this;
    }

    private void Start()
    {
        //initialise all the other variables.
        turnManager = TurnManager.theTurnManager;
        agent = GetComponent<NavGridAgent>();
        actionSelector = FindObjectOfType<ActionSelector>(true);
        if(undoActionButton)
        {
            undoActionButton.onClick.AddListener(UndoAction);
        }
        
        Invoke("UpdateNeighborNodeMaterials", 0.1f);
    }
    
    private void Update()
    {
        //updates the player actions
        UpdatePlayerActions();        
    }
}
