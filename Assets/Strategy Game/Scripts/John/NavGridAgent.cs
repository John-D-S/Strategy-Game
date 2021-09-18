using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NavGridAgent : MonoBehaviour
{
	[SerializeField] private NavGrid navGrid;
	/// <summary> How many nodes per second to walk </summary>
	public float walkSpeed = .25f;
	/// <summary> How fast should the agent lerp to its current node? </summary>
	public float lerpAmount = .25f;
	private NavGridNode currentNode;
	private NavGridNode targetNode;
	private List<NavGridNode> currentPath = new List<NavGridNode>();

	private class TempNodeProperties
	{
		public TempNodeProperties(float _gCost, float _hCost, NavGridNode _parent)
		{
			gCost = _gCost;
			hCost = _hCost;
			parent = _parent;
		}
		
		public float gCost;
		public float hCost;
		public float FCost => gCost + hCost;
		public NavGridNode parent;
	}
	
	public List<NavGridNode> CalculatePathToTarget(NavGridNode _start, NavGridNode _target)
	{
		Dictionary<NavGridNode, TempNodeProperties> nodeProperties = new Dictionary<NavGridNode, TempNodeProperties>();

		List<NavGridNode> openNodes = new List<NavGridNode>(); //the set of nodes to be evaluated
		List<NavGridNode> closedNodes = new List<NavGridNode>(); //the set of nodes already evaluated

		NavGridNode OpenNodeWithLowestFCost()
		{
			NavGridNode currentLowestFCostNode = openNodes[0];
			for(int i = 1; i < openNodes.Count; i++)
			{
				NavGridNode openNode = openNodes[i];
				bool conditionOne = nodeProperties[openNode].FCost < nodeProperties[currentLowestFCostNode].FCost;
				if (conditionOne || Math.Abs(nodeProperties[openNode].FCost - nodeProperties[currentLowestFCostNode].FCost) < 0.5f && nodeProperties[openNode].hCost < nodeProperties[currentLowestFCostNode].hCost)
				{
					currentLowestFCostNode = openNode;
				}
			}
			return currentLowestFCostNode;
		}

		openNodes.Add(_start);
		nodeProperties[openNodes[0]] = new TempNodeProperties(0, Vector3.Distance(openNodes[0].transform.position, targetNode.transform.position), null);
		while(openNodes.Count > 0)
		{
			NavGridNode current = OpenNodeWithLowestFCost();
			openNodes.Remove(current);
			closedNodes.Add(current);

			//path has been found
			if(current == targetNode)
			{
				break;
			}

			foreach(NavGridNode neighbor in current.Neighbors)
			{
				if(closedNodes.Contains(neighbor))
				{
					continue;
				}

				float newMovementCostToNeighbor = nodeProperties[current].gCost + Vector3.Distance(current.transform.position, neighbor.transform.position);
				if(!openNodes.Contains(neighbor) || newMovementCostToNeighbor < nodeProperties[neighbor].gCost)
				{
					nodeProperties[neighbor] = new TempNodeProperties(newMovementCostToNeighbor, Vector3.Distance(neighbor.transform.position, targetNode.transform.position), current);
					if(!openNodes.Contains(neighbor))
					{
						openNodes.Add(neighbor);
					}
				}
			}
		}

		List<NavGridNode> path = new List<NavGridNode>();
		NavGridNode currentPathNode = _target;
		while(currentPathNode != _start)
		{
			path.Add(currentPathNode);
			currentPathNode = nodeProperties[currentPathNode].parent;
		}
		path.Reverse();
		return path;
	}

	public void MoveTowards(NavGridNode _target)
	{
		if(currentNode != _target)
		{
			if(!(currentPath.Count > 0 && currentPath[currentPath.Count - 1] == _target))
			{
				currentPath = CalculatePathToTarget(currentNode, targetNode);
			}
			if(currentPath.Count > 0)
			{
				currentNode = currentPath[0];
				currentPath.RemoveAt(0);
			}
		}
	}
	
	private IEnumerator WalkTowardsTarget(NavGridNode _target)
	{
		targetNode = _target;
		while(currentNode != targetNode)
		{
			MoveTowards(targetNode);
			yield return new WaitForSeconds(walkSpeed);
		}
	}

	private IEnumerator WalkTowardsTarget(NavGridNode _target, int _maxTilesToMove)
	{
		targetNode = _target;
		int walkedTiles = 0;
		while(currentNode != targetNode && walkedTiles < _maxTilesToMove)
		{
			MoveTowards(targetNode);
			walkedTiles++;
			yield return new WaitForSeconds(walkSpeed);
		}
	}
	
	public void MoveToTarget(NavGridNode _target)
	{
		StopAllCoroutines();
		StartCoroutine(WalkTowardsTarget(_target));
	}

	public void MoveToTarget(NavGridNode _target, int _maxTilesToMove)
	{
		StopAllCoroutines();
        StartCoroutine(WalkTowardsTarget(_target, _maxTilesToMove));
	}
	
	public void MoveToTarget(GameObject _target)
	{
		if(navGrid.NodesByGameObject.ContainsKey(_target))
		{
			NavGridNode targetNode = navGrid.NodesByGameObject[_target];
			StopAllCoroutines();
			StartCoroutine(WalkTowardsTarget(targetNode));
		}
	}

	public void MoveToTarget(GameObject _target, int _maxTilesToMove)
	{
		if(navGrid.NodesByGameObject.ContainsKey(_target))
		{
			NavGridNode targetNode = navGrid.NodesByGameObject[_target];
			StopAllCoroutines();
			StartCoroutine(WalkTowardsTarget(targetNode, _maxTilesToMove));
		}
	}
	
	public void MoveToTarget(Vector3 _target)
	{
		NavGridNode targetNode = navGrid.ClosestNavGridNodeToPosition(_target);
		StopAllCoroutines();
		StartCoroutine(WalkTowardsTarget(targetNode));
	}
	
	public void MoveToTarget(Vector3 _target, int _maxTilesToMove)
	{
		NavGridNode targetNode = navGrid.ClosestNavGridNodeToPosition(_target);
		StopAllCoroutines();
		StartCoroutine(WalkTowardsTarget(targetNode, _maxTilesToMove));
	}
	
	private void FixedUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, currentNode.transform.position, lerpAmount);
	}

	private void Start()
	{
		if(navGrid)
		{
			currentNode = navGrid.ClosestNavGridNodeToPosition(transform.position);
			transform.position = currentNode.transform.position;
		}
	}
	
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		foreach(NavGridNode node in currentPath)
		{
			Gizmos.DrawSphere(node.transform.position, 0.25f);
		}
	}
}
