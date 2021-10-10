using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAction
{
    /// <summary>
    /// the initialiser sets the node that this acion acts upon
    /// </summary>
    public PlayerAction(NavGridNode _node, PlayerController _player)
    {
        node = _node;
        player = _player;
    }
    
    protected string actionName = "default Action Name";
    public string ActionName => actionName;
    protected NavGridNode node;
    protected PlayerController player;
    private bool hasBeenExexuted;
    
    /// <summary>
    /// makes the player do the action
    /// </summary>
    public void PlayerExecuteAction()
    {
        if(player.NeighboringNodes.Contains(node))
        {
            player.actionsDoneThisTurn.Add(this);
            ExecuteAction();
            player.HideActionSelector();
            player.UpdateNeighborNodeMaterials();
            hasBeenExexuted = true;
        }
    }

    /// <summary>
    /// Undoes the action once it has been done
    /// </summary>
    public void PlayerUndoAction()
    {
        if(hasBeenExexuted)
        {
            UndoAction();
            player.UpdateNeighborNodeMaterials();
            hasBeenExexuted = false;
        }
        
    }
    
    /// <summary>
    /// the code that is run when this action is executed
    /// </summary>
    protected abstract void ExecuteAction();
    /// <summary>
    /// the code that is run to undo whatever this action did
    /// </summary>
    protected abstract void UndoAction();
}

/// <summary>
/// The action that makes the player move to a tile
/// </summary>
public class Move : PlayerAction
{
    /// <summary>
    /// the tile that the player moved from
    /// </summary>
    private NavGridNode lastNode;
    /// <summary>
    /// the item that the player picked up when it moved, if anything
    /// </summary>
    private Item pickedUpItem;
    
    /// <summary>
    /// the initialiser derives from the base action class
    /// </summary>
    public Move(NavGridNode _node, PlayerController _player) : base(_node, _player)
    {
        actionName = "Move Here";
    }
    
    /// <summary>
    /// move the player to the node and try to pick up any item on that node
    /// </summary>
    protected override void ExecuteAction()
    {
        lastNode = player.CurrentNode;
        player.PlayerAgent.MoveToTarget(node);
        pickedUpItem = player.TryPickupItem();
    }

    /// <summary>
    /// puts any item back and moves the player back to the lastNode
    /// </summary>
    protected override void UndoAction()
    {
        if(pickedUpItem)
        {
            player.UndoPickupItem(pickedUpItem);
        }
        player.PlayerAgent.MoveToTarget(lastNode);
    }
}

public class PlaceItem : PlayerAction
{
    private Item itemToPlace;
    private GameObject ObjectToPlace => itemToPlace.gameObject;
    private GameObject placedObject;
    
    /// <summary>
    /// initializes the name of the action and the item to place
    /// </summary>
    public PlaceItem(NavGridNode _node, PlayerController _player, Item _itemToPlace) : base(_node, _player)
    {
        itemToPlace = _itemToPlace;
        actionName = $"Place {itemToPlace.ItemName} here";
    }
    
    /// <summary>
    /// Removes the item from the player's list of placeable items.
    /// </summary>
    protected override void ExecuteAction()
    {
        player.collectedItems.Remove(itemToPlace);
        placedObject = Object.Instantiate(ObjectToPlace,  node.transform.position, Quaternion.identity);
        placedObject.SetActive(true);
        placedObject.GetComponent<Item>().isActivated = true;
    }

    /// <summary>
    /// destroys the placed item and returns the action to the player's list of avialable actions.
    /// </summary>
    protected override void UndoAction()
    {
        Object.Destroy(placedObject);
        player.collectedItems.Add(itemToPlace);
    }
}
