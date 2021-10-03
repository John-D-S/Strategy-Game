using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

using Action = System.Action;

public class Item : MonoBehaviour
{
	[SerializeField] private string itemName;
	public string ItemName => itemName;
	[System.NonSerialized] public bool isActivated;
	public static List<Item> allItems = new List<Item>();
	private NavGrid navGrid;

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

	public virtual void PickUpItem(PlayerController _playerController)
	{
		_playerController.collectedItems.Add(this);
		gameObject.SetActive(false);
	}

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
