using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// a component given to an object that can be picked up and placed by the player
/// </summary>
public class Item : MonoBehaviour
{
	[SerializeField, Tooltip("The name of the item.")] private string itemName;
	/// <summary> the name of the item </summary>
	public string ItemName => itemName;
	[System.NonSerialized] public bool isActivated;
	public static List<Item> allItems = new List<Item>();
	private NavGrid navGrid;

	/// <summary>
	/// Returns the closest item to the given position
	/// </summary>
	/// <param name="_maxDistance">This funciton will not return an item further away from this distance.</param>
	/// <param name="_position">The position to get the closest item to.</param>
	public static Item ItemNearPosition(float _maxDistance, Vector3 _position)
	{
		foreach(Item item in allItems)
		{
			if(Vector3.Distance(_position, item.transform.position) < _maxDistance)
			{
				if(!item.isActivated)
				{
					return item;
				}
			}			
		}
		return null;
	}
	
	/// <summary>
	/// add the item to the players list of picked up items and set this item to inactive
	/// </summary>
	public virtual void PickUpItem(PlayerController _playerController)
	{
		_playerController.collectedItems.Add(this);
		gameObject.SetActive(false);
	}

	/// <summary>
	/// set this item back to active and remove it from the players list of collected items.
	/// </summary>
	public virtual void UndoPickUpItem(PlayerController _playerController)
	{
		gameObject.SetActive(true);
		_playerController.collectedItems.Remove(this);
	}
	
	public void Start()
	{
		allItems.Add(this);
		navGrid = FindObjectOfType<NavGrid>();
		transform.position = navGrid.ClosestNavGridNodeToPosition(transform.position).transform.position;
	}

	private void OnDestroy()
	{
		if(allItems.Contains(this))
		{
			allItems.Remove(this);
		}
	}
}
