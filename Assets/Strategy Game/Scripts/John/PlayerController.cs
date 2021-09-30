using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.WSA;

using Object = UnityEngine.Object;

public abstract class Action
{
    public Action(NavGridNode _node, PlayerController _player)
    {
        node = _node;
        player = _player;
    }
    protected NavGridNode node;
    protected PlayerController player;
    private bool hasBeenExexuted;
    
    public void PlayerExecuteAction()
    {
        if(player.NeighboringNodes.Contains(node))
        {
            ExecuteAction();
            hasBeenExexuted = true;
        }
    }

    public void PlayerUndoAction()
    {
        if(hasBeenExexuted)
        {
            UndoAction();
            hasBeenExexuted = false;
        }
        
    }
    
    protected abstract void ExecuteAction();
    protected abstract void UndoAction();
}

public class Move : Action
{
    private NavGridNode lastNode;
    
    public Move(NavGridNode _node, PlayerController _player) : base(_node, _player) { }
    
    protected override void ExecuteAction()
    {
        lastNode = player.CurrentNode;
        player.PlayerAgent.MoveToTarget(node);
    }

    protected override void UndoAction()
    {
        player.PlayerAgent.MoveToTarget(lastNode);
    }
}

public class PlaceGameObject : Action
{
    private GameObject objectToPlace;
    private GameObject placedObject;

    public PlaceGameObject(NavGridNode _node, PlayerController _player, GameObject _objectToPlace) : base(_node, _player)
    {
        objectToPlace = _objectToPlace;
    }
    
    protected override void ExecuteAction()
    {
        placedObject = Object.Instantiate(objectToPlace);
    }

    protected override void UndoAction()
    {
        Object.Destroy(placedObject);
    }
}

[RequireComponent(typeof(NavGridAgent))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private int actionsAtStartOfTurn = 5;
    private List<Action> availableActions = new List<Action>();
    
    private NavGridAgent agent;
    public NavGridAgent PlayerAgent => agent;
    public NavGridNode CurrentNode => agent.CurrentNode;
    public List<NavGridNode> NeighboringNodes => CurrentNode.Neighbors;
    
    private TurnManager turnManager;

    public List<Action> actionsDoneThisTurn;
    
    public void DoAction(NavGridNode _node, Action _action)
    {
        if(actionsDoneThisTurn.Count < actionsAtStartOfTurn)
        {
            actionsDoneThisTurn.Add(_action);
            actionsDoneThisTurn[actionsDoneThisTurn.Count - 1].PlayerExecuteAction();
        }
    }

    public void UndoAction()
    {
        if(actionsDoneThisTurn.Count > 0)
        {
            Action lastAction = actionsDoneThisTurn[actionsDoneThisTurn.Count];
            lastAction.PlayerUndoAction();
            actionsDoneThisTurn.Remove(lastAction);
        }
    }


    public void StartNewTurn()
    {
        actionsRemaining = actionsAtStartOfTurn;
    }

    public void EndTurn()
    {
        turnManager.StartNextTurn();
    }

    private void UpdatePlayerActions()
    {
        
    }
    
    private void Start()
    {
        turnManager = TurnManager.theTurnManager;
        agent = GetComponent<NavGridAgent>();
    }
    
    private void Update()
    {
        UpdatePlayerActions();        
    }
}
