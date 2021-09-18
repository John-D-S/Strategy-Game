using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NavGridAgent : MonoBehaviour
{
	[SerializeField, Tooltip("The NavGrid this Agent is using.")] private NavGrid navGrid;
	/// <summary> How many nodes per second to walk </summary>
	[Tooltip("How many nodes per second to walk")] public float walkSpeed = .25f;
	/// <summary> How fast should the agent lerp to its current node. </summary>
	[Tooltip("How fast should the agent lerp to its current node.")] public float lerpAmount = .25f;
	/// <summary> Should the Agent turn to face the direction last traveled in. </summary>
	[Tooltip("Should the Agent turn to face the direction last traveled in.")]public bool turnInDirectionOfTravel;
	private NavGridNode currentNode;
	private NavGridNode targetNode;
	private List<NavGridNode> currentPath = new List<NavGridNode>();

	/// <summary>
	/// used to store information about a node during pathfinding
	/// </summary>
	private class TempNodeProperties
	{
		public TempNodeProperties(float _gCost, float _hCost, NavGridNode _parent)
		{
			gCost = _gCost;
			hCost = _hCost;
			parent = _parent;
		}
		
		/// <summary> the distance from the start node </summary>
		public float gCost;
		/// <summary> the distance to the end node </summary>
		public float hCost;
		/// <summary> used to choose the shortest path </summary>
		public float FCost => gCost + hCost;
		/// <summary> these will link in a chain back to the start node </summary>
		public NavGridNode parent;
	}
	
	/// <summary>
	/// Calculates and returns the list of NavGridNodes which lead from the start to the target.
	/// </summary>
	/// <param name="_start">The node to start the path from.</param>
	/// <param name="_target">The node for the path to reach.</param>
	public List<NavGridNode> CalculatePathToTarget(NavGridNode _start, NavGridNode _target)
	{
		//an easily accessable dictionary of properties for finding the shortest path
		Dictionary<NavGridNode, TempNodeProperties> nodeProperties = new Dictionary<NavGridNode, TempNodeProperties>();

		List<NavGridNode> openNodes = new List<NavGridNode>(); //the set of nodes to be evaluated
		List<NavGridNode> closedNodes = new List<NavGridNode>(); //the set of nodes already evaluated

		//used to find the node in openNodes with the lowest f cost
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

		//add the start node to openNodes
		openNodes.Add(_start);
		//set the node properties of the first node, so as to not cause an error. parent can be null because the parent of the start node will not be referenced
		nodeProperties[openNodes[0]] = new TempNodeProperties(0, Vector3.Distance(openNodes[0].transform.position, targetNode.transform.position), null);
		//if openNodes.count reaches zero, it means that all nodes have been covered and the target node was not found
		while(openNodes.Count > 0)
		{
			NavGridNode current = OpenNodeWithLowestFCost();
			//move the current node from open to closed
			openNodes.Remove(current);
			closedNodes.Add(current);

			//path has been found
			if(current == targetNode)
			{
				break;
			}
			
			foreach(NavGridNode neighbor in current.Neighbors)
			{
				//do not go over any nodes in closed, because they're closed
				if(closedNodes.Contains(neighbor))
				{
					continue;
				}
				
				//this will be the neighbor's gCost, and it is the length of the path that traces back to the start node.
				//it is the length of the path to current, plus the distance from current to neighbor
				float newMovementCostToNeighbor = nodeProperties[current].gCost + Vector3.Distance(current.transform.position, neighbor.transform.position);	
				//if the neighbor is not already in open nodes, or the new gCost is less than its current gCost,
				//update its node properties and add it to open nodes if it is not already there 
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

		//the return value
		List<NavGridNode> path = new List<NavGridNode>();
		//start at the _target node and work back through each node's parent
		//until the start node is reached, adding each one to the return value
		NavGridNode currentPathNode = _target;
		while(currentPathNode != _start)
		{
			path.Add(currentPathNode);
			currentPathNode = nodeProperties[currentPathNode].parent;
		}
		//reverse the path so that it goes from start to target
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

#region MoveToTargetOverrides
	
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
			NavGridNode nodeTarget = navGrid.NodesByGameObject[_target];
			StopAllCoroutines();
			StartCoroutine(WalkTowardsTarget(nodeTarget));
		}
	}

	public void MoveToTarget(GameObject _target, int _maxTilesToMove)
	{
		if(navGrid.NodesByGameObject.ContainsKey(_target))
		{
			NavGridNode nodeTarget = navGrid.NodesByGameObject[_target];
			StopAllCoroutines();
			StartCoroutine(WalkTowardsTarget(nodeTarget, _maxTilesToMove));
		}
	}
	
	public void MoveToTarget(Vector3 _target)
	{
		NavGridNode nodeTarget = navGrid.ClosestNavGridNodeToPosition(_target);
		StopAllCoroutines();
		StartCoroutine(WalkTowardsTarget(nodeTarget));
	}
	
	public void MoveToTarget(Vector3 _target, int _maxTilesToMove)
	{
		NavGridNode nodeTarget = navGrid.ClosestNavGridNodeToPosition(_target);
		StopAllCoroutines();
		StartCoroutine(WalkTowardsTarget(nodeTarget, _maxTilesToMove));
	}
	
#endregion

#region NumberOfNodesToTargetOverrides

	public int NumberOfNodesToTarget(NavGridNode _target)
	{
		return CalculatePathToTarget(currentNode, _target).Count;
	}
	
	public int NumberOfNodesToTarget(GameObject _target)
	{
		if(navGrid.NodesByGameObject.ContainsKey(_target))
		{
			NavGridNode nodeTarget = navGrid.NodesByGameObject[_target];
			return CalculatePathToTarget(currentNode, nodeTarget).Count;
		}
		return 0;
	}

	public int NumberOfNodesToTarget(Vector3 _target)
	{
		NavGridNode nodeTarget = navGrid.ClosestNavGridNodeToPosition(_target);
		return CalculatePathToTarget(currentNode, nodeTarget).Count;
	}

#endregion
	
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
