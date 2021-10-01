using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.WSA;

using Cursor = UnityEngine.Cursor;
using Object = UnityEngine.Object;

[RequireComponent(typeof(NavGridAgent))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private int actionsAtStartOfTurn = 5;
    private List<PlayerAction> availableActions = new List<PlayerAction>();
    
    private NavGridAgent agent;
    public NavGridAgent PlayerAgent => agent;
    public NavGridNode CurrentNode => agent.CurrentNode;
    public List<NavGridNode> NeighboringNodes => CurrentNode.Neighbors;
    public List<Item> collectedItems = new List<Item>();
    public List<PlayerAction> actionsDoneThisTurn;

    private NavGridNode selectedNode;
    public NavGridNode SelectedNode => selectedNode;

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

    public Item TryPickupItem()
    {
        Item item = Item.ItemNearPosition(agent.AgentNavGrid.GridSize * 0.5f, agent.CurrentNode.transform.position);
        if(item)
        {
            collectedItems.Add(item);
            item.gameObject.SetActive(false);
        }
        return item;
    }

    public void UndoPickupItem(ref Item _item)
    {
        _item.gameObject.SetActive(true);
        collectedItems.Remove(_item);
    }
    
    private TurnManager turnManager;

    public void DoAction(NavGridNode _node, PlayerAction _playerAction)
    {
        if(actionsDoneThisTurn.Count < actionsAtStartOfTurn)
        {
            actionsDoneThisTurn.Add(_playerAction);
            actionsDoneThisTurn[actionsDoneThisTurn.Count - 1].PlayerExecuteAction();
        }
    }

    public void UndoAction()
    {
        if(actionsDoneThisTurn.Count > 0)
        {
            PlayerAction lastPlayerAction = actionsDoneThisTurn[actionsDoneThisTurn.Count];
            lastPlayerAction.PlayerUndoAction();
            actionsDoneThisTurn.Remove(lastPlayerAction);
        }
    }

    public void EndTurn()
    {
        turnManager.StartNextTurn();
    }

    public void ShowActionSelector()
    {
        actionSelector.gameObject.SetActive(true);
    }

    public void HideActionSelector()
    {
        actionSelector.gameObject.SetActive(false);
    }
    
    private void UpdatePlayerActions()
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
                ShowActionSelector();
            }
            else
            {
                HideActionSelector();
            }
        }
    }
    
    private void Start()
    {
        turnManager = TurnManager.theTurnManager;
        agent = GetComponent<NavGridAgent>();
        actionSelector = FindObjectOfType<ActionSelector>(true);
    }
    
    private void Update()
    {
        UpdatePlayerActions();        
    }
}
