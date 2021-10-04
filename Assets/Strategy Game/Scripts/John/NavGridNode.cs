using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NavGridNode : MonoBehaviour
{
	[System.NonSerialized] public NavGrid navGrid;
	public List<NavGridNode> Neighbors { get; } = new List<NavGridNode>();

	public bool IsBlocked => Physics.CheckSphere(transform.position, navGrid.GridSize * 0.25f, navGrid.PathBlockingLayers);

	private bool IsInLayerMask(GameObject _gameObject, LayerMask _layerMask)
	{
		return (_layerMask.value & (1 << _gameObject.layer)) > 0;
	}
	
	public void TryLinkNodes(NavGridNode _navGridNodeToLink)
	{
		if(!Neighbors.Contains(_navGridNodeToLink) && _navGridNodeToLink != this)
		{
			Neighbors.Add(_navGridNodeToLink);
			_navGridNodeToLink.TryLinkNodes(this);
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
					NavGridNode hitNavGridNode = hitObject.GetComponent<NavGridNode>();
					if(hitNavGridNode)
					{
						TryLinkNodes(hitNavGridNode);
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
		Gizmos.color = new Color(0, 1, 0, 0.1f);
		foreach(NavGridNode neighbor in Neighbors)
		{
			if(neighbor)
			{
				Gizmos.DrawLine(transform.position, neighbor.transform.position);
			}
		}

		if(IsBlocked)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position, 1f);
		}
	}
}
