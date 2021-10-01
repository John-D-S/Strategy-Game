using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAction
{
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
    
    public void PlayerExecuteAction()
    {
        if(player.NeighboringNodes.Contains(node))
        {
            ExecuteAction();
            player.HideActionSelector();
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

public class Move : PlayerAction
{
    private NavGridNode lastNode;
    private Item pickedUpItem;
    
    public Move(NavGridNode _node, PlayerController _player) : base(_node, _player)
    {
        actionName = "Move Here";
    }
    
    protected override void ExecuteAction()
    {
        lastNode = player.CurrentNode;
        player.PlayerAgent.MoveToTarget(node);
        pickedUpItem = player.TryPickupItem();
    }

    protected override void UndoAction()
    {
        player.UndoPickupItem(ref pickedUpItem);
        player.PlayerAgent.MoveToTarget(lastNode);
    }
}

public class PlaceItem : PlayerAction
{
    private Item itemToPlace;
    private GameObject ObjectToPlace => itemToPlace.gameObject;
    private GameObject placedObject;

    public PlaceItem(NavGridNode _node, PlayerController _player, Item _itemToPlace) : base(_node, _player)
    {
        itemToPlace = _itemToPlace;
        actionName = $"Place {itemToPlace.ItemName} here";
    }
    
    protected override void ExecuteAction()
    {
        player.collectedItems.Remove(itemToPlace);
        placedObject = Object.Instantiate(ObjectToPlace,  node.transform.position, Quaternion.identity);
        placedObject.SetActive(true);
        placedObject.GetComponent<Item>().isActivated = true;
    }

    protected override void UndoAction()
    {
        Object.Destroy(placedObject);
        player.collectedItems.Add(itemToPlace);
    }
}
