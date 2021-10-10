using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NavGridNode : MonoBehaviour
{
	[System.NonSerialized] public NavGrid navGrid;
	public List<NavGridNode> Neighbors { get; } = new List<NavGridNode>();

	/// <summary>
	/// returns whether or not the node is near a collider with a layere in the PathBlockingLayers LayerMask
	/// </summary>
	public bool IsBlocked => Physics.CheckSphere(transform.position, navGrid.GridSize * 0.25f, navGrid.PathBlockingLayers);

	/// <summary>
	/// returns whether or not a gameobject is in the given layermask
	/// </summary>
	private bool IsInLayerMask(GameObject _gameObject, LayerMask _layerMask)
	{
		return (_layerMask.value & (1 << _gameObject.layer)) > 0;
	}
	
	/// <summary>
	/// links this node to the given node both ways.
	/// </summary>
	public void TryLinkNodes(NavGridNode _navGridNodeToLink)
	{
		if(!Neighbors.Contains(_navGridNodeToLink) && _navGridNodeToLink != this)
		{
			Neighbors.Add(_navGridNodeToLink);
			_navGridNodeToLink.TryLinkNodes(this);
		}
	}
	
	/// <summary>
	/// link all neighboring nodes in a square around this node
	/// </summary>
	public void LinkSurroundingNodes()
	{
		//cycle through all the node positions in a 3x3 square around this one
		for(int x = -1; x < 2; x++)
		{
			for(int z = -1; z < 2; z++)
			{
				//raycast to get the node at that position.
				RaycastHit hit = new RaycastHit();
				Ray ray = new Ray(transform.position + new Vector3(x * navGrid.GridSize, navGrid.NodeCheckHeight, z * navGrid.GridSize), Vector3.down);
				if(Physics.Raycast(ray, out hit, navGrid.NodeCheckRayDistance, ~navGrid.PathBlockingLayers))
				{
					//try to link this node and the node hit by the raycast
					GameObject hitObject = hit.collider.gameObject;
					NavGridNode hitNavGridNode = hitObject.GetComponent<NavGridNode>();
					if(hitNavGridNode)
					{
						TryLinkNodes(hitNavGridNode);
					}
				}
			}			
		}
	}

	/// <summary>
	/// returns a list of the positions in a 3x3 grid around this node's position which have not yet been taken up by nodes.
	/// </summary>
	public List<Vector3> EmptyNeighborPositions()
	{
		List<Vector3> returnValue = new List<Vector3>();
		for(int x = -1; x < 2; x++)
		{
			for(int z = -1; z < 2; z++)
			{
				RaycastHit hit = new RaycastHit();
				Ray ray = new Ray(transform.position + new Vector3(x * navGrid.GridSize, navGrid.NodeCheckHeight, z * navGrid.GridSize), Vector3.down);
				if(Physics.Raycast(ray, out hit, navGrid.NodeCheckRayDistance, ~navGrid.PathBlockingLayers))
				{
					if(IsInLayerMask( hit.collider.gameObject, navGrid.AllowedNodeLayers))
					{
						returnValue.Add(hit.point);
					}
				}
			}			
		}
		return returnValue;
	}

	private void OnDrawGizmos()
	{
		//show th links between the nodes.
		Gizmos.color = new Color(0, 1, 0, 0.1f);
		foreach(NavGridNode neighbor in Neighbors)
		{
			if(neighbor)
			{
				Gizmos.DrawLine(transform.position, neighbor.transform.position);
			}
		}

		//show which tiles are blocked
		if( Application.isPlaying && IsBlocked)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position, 1f);
		}
	}
}
