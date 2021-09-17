using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Node : MonoBehaviour
{
	[System.NonSerialized] public NavGrid navGrid;
	private List<Node> neighbors = new List<Node>();

	private bool IsInLayerMask(GameObject _gameObject, LayerMask _layerMask)
	{
		return (_layerMask.value & (1 << _gameObject.layer)) > 0;
	}
	
	public void TryLinkNodes(Node _nodeToLink)
	{
		if(!neighbors.Contains(_nodeToLink) && _nodeToLink != this)
		{
			neighbors.Add(_nodeToLink);
			_nodeToLink.TryLinkNodes(this);
		}
	}

	public void LinkSurroundingNodes()
	{
		for(int x = -1; x < 2; x++)
		{
			for(int z = -1; z < 2; z++)
			{
				RaycastHit hit = new RaycastHit();
				Ray ray = new Ray(transform.position + new Vector3(x * navGrid.GridSize, navGrid.NodeCheckHeight, z * navGrid.GridSize), Vector3.down);
				if(Physics.Raycast(ray, out hit, navGrid.NodeCheckRayDistance))
				{
					GameObject hitObject = hit.collider.gameObject;
					Node hitNode = hitObject.GetComponent<Node>();
					if(hitNode)
					{
						TryLinkNodes(hitNode);
					}
				}
			}			
		}
	}

	public List<Vector3> EmptyNeighborPositions()
	{
		List<Vector3> returnValue = new List<Vector3>();
		for(int x = -1; x < 2; x++)
		{
			for(int z = -1; z < 2; z++)
			{
				RaycastHit hit = new RaycastHit();
				Ray ray = new Ray(transform.position + new Vector3(x * navGrid.GridSize, navGrid.NodeCheckHeight, z * navGrid.GridSize), Vector3.down);
				if(Physics.Raycast(ray, out hit, navGrid.NodeCheckRayDistance))
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
		Gizmos.color = Color.green;
		foreach(Node neighbor in neighbors)
		{
			Gizmos.DrawLine(transform.position, neighbor.transform.position);
		}
	}
}
