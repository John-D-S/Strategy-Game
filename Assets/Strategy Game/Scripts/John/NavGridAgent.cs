using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NavGridAgent : MonoBehaviour
{
	[SerializeField, Tooltip("The NavGrid this Agent is using.")] private NavGrid navGrid;
	public NavGrid AgentNavGrid => navGrid;
	/// <summary> How many nodes per second to walk </summary>
	[Tooltip("How many nodes per second to walk")] public float walkSpeed = .25f;
	/// <summary> How fast should the agent lerp to its current node. </summary>
	[Tooltip("How fast should the agent lerp to its current node.")] public float lerpAmount = .25f;
	/// <summary> Should the Agent turn to face the direction last traveled in. </summary>
	[Tooltip("Should the Agent turn to face the direction last traveled in.")]public bool turnInDirectionOfTravel;
	/// <summary> How fast should the agent tourn to face the direction last traveled in, if it is set to do that. </summary>
	[SerializeField, Tooltip("Should the Agent turn to face the direction last traveled in.")] private float turnLerpSpeed = 0.2f;
	
	private NavGridNode currentNode;
	public NavGridNode CurrentNode => currentNode;
	private NavGridNode targetNode;
	private List<NavGridNode> currentPath = new List<NavGridNode>();

	/// <summary>
	/// returns the first node in the path towards the target
	/// </summary>
	public NavGridNode NextNodeTowardTarget(NavGridNode _target, bool _accountForBlocks = false)
	{
		if(_target == currentNode)
		{
			return currentNode;
		}
		if(currentPath != null && currentPath.Count > 0 && _target == currentPath[currentPath.Count - 1])
		{
			return currentPath[0];
		}
		
		List<NavGridNode> calculatedPath = CalculatePathToTarget(currentNode, _target, _accountForBlocks);
		if(calculatedPath == null)
		{
			return null;
		}
		return calculatedPath[0];
	}

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
	/// <param name="_start">The node to start the path from (not Included in the returned path).</param>
	/// <param name="_target">The node for the path to reach.</param>
	/// <param name="_accountForBlocks">if true, the path will not go through blocked nodes</param>
	public List<NavGridNode> CalculatePathToTarget(NavGridNode _start, NavGridNode _target, bool _accountForBlocks = false)
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
		nodeProperties[openNodes[0]] = new TempNodeProperties(0, Vector3.Distance(openNodes[0].transform.position, _target.transform.position), null);
		//if openNodes.count reaches zero, it means that all nodes have been covered and the target node was not found
		while(openNodes.Count > 0)
		{
			NavGridNode current = OpenNodeWithLowestFCost();
			//move the current node from open to closed
			openNodes.Remove(current);
			closedNodes.Add(current);

			//path has been found
			if(current == _target)
			{
				break;
			}
			
			foreach(NavGridNode neighbor in current.Neighbors)
			{
				//do not go over any nodes in closed, because they're closed. Also don't go over blocked nodes if _accountForBlocks is true.
				if(closedNodes.Contains(neighbor) || (neighbor.IsBlocked && _accountForBlocks))
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
					nodeProperties[neighbor] = new TempNodeProperties(newMovementCostToNeighbor, Vector3.Distance(neighbor.transform.position, _target.transform.position), current);
					if(!openNodes.Contains(neighbor))
					{
						openNodes.Add(neighbor);
					}
				}
			}
		}

		if(nodeProperties.ContainsKey(_target))
		{
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
		return null;
	}

	/// <summary>
	/// move the agent one block towards the target
	/// </summary>
	public void MoveTowards(NavGridNode _target, bool _accountForBlocks = false)
	{
		targetNode = _target;
		if(currentNode != _target)
		{
			if(currentPath.Count < 1 || currentPath[currentPath.Count - 1] != _target || _accountForBlocks)
			{
				currentPath = CalculatePathToTarget(currentNode, targetNode, _accountForBlocks);
				if(currentPath == null)
				{
					return;
				}
			}
			if(currentPath.Count > 0)
			{
				currentNode = currentPath[0];
				currentPath.RemoveAt(0);
			}
		}
	}
	
	/// <summary>
	/// step the agent all the way to the given target
	/// </summary>
	private IEnumerator WalkTowardsTarget(NavGridNode _target, bool _accountForBlocks = false)
	{
		targetNode = _target;
		while(currentNode != targetNode)
		{
			MoveTowards(targetNode, _accountForBlocks);
			yield return new WaitForSeconds(walkSpeed);
		}
	}

	/// <summary>
	/// step the agent along to the given target for _maxTilesToMove blocks
	/// </summary>
	private IEnumerator WalkTowardsTarget(NavGridNode _target, int _maxTilesToMove, bool _accountForBlocks = false)
	{
		targetNode = _target;
		int walkedTiles = 0;
		while(currentNode != targetNode && walkedTiles < _maxTilesToMove)
		{
			MoveTowards(targetNode, _accountForBlocks);
			walkedTiles++;
			yield return new WaitForSeconds(walkSpeed);
		}
	}

#region MoveToTargetOverrides
	/// <summary>
	/// step the agent all the way to the given target.
	/// </summary>
	public void MoveToTarget(NavGridNode _target)
	{
		StopAllCoroutines();
		StartCoroutine(WalkTowardsTarget(_target));
	}

	/// <summary>
	/// step the agent along to the given target for _maxTilesToMove blocks
	/// </summary>
	public void MoveToTarget(NavGridNode _target, int _maxTilesToMove)
	{
		StopAllCoroutines();
        StartCoroutine(WalkTowardsTarget(_target, _maxTilesToMove));
	}
	
	/// <summary>
	/// step the agent all the way to node closest to the _target GameObject.
	/// </summary>
	public void MoveToTarget(GameObject _target)
	{
		if(navGrid.NodesByGameObject.ContainsKey(_target))
		{
			NavGridNode nodeTarget = navGrid.NodesByGameObject[_target];
			StopAllCoroutines();
			StartCoroutine(WalkTowardsTarget(nodeTarget));
		}
	}

	/// <summary>
	/// step the agent along the path to the given _target GameObject for _maxTilesToMove blocks
	/// </summary>
	public void MoveToTarget(GameObject _target, int _maxTilesToMove)
	{
		if(navGrid.NodesByGameObject.ContainsKey(_target))
		{
			NavGridNode nodeTarget = navGrid.NodesByGameObject[_target];
			StopAllCoroutines();
			StartCoroutine(WalkTowardsTarget(nodeTarget, _maxTilesToMove));
		}
	}
	
	/// <summary>
	/// Step the agent towards the closest node to the given _target position
	/// </summary>
	public void MoveToTarget(Vector3 _target)
	{
		NavGridNode nodeTarget = navGrid.ClosestNavGridNodeToPosition(_target);
		StopAllCoroutines();
		StartCoroutine(WalkTowardsTarget(nodeTarget));
	}
	
	/// <summary>
	/// move the agent along the path to the closest node to the given _target position for _maxTilesToMove nodes
	/// </summary>
	/// <param name="_target"></param>
	/// <param name="_maxTilesToMove"></param>
	public void MoveToTarget(Vector3 _target, int _maxTilesToMove)
	{
		NavGridNode nodeTarget = navGrid.ClosestNavGridNodeToPosition(_target);
		StopAllCoroutines();
		StartCoroutine(WalkTowardsTarget(nodeTarget, _maxTilesToMove));
	}
	
#endregion

#region NumberOfNodesToTargetOverrides

	/// <summary>
	/// returns the number of nodes to the given navgrid node
	/// </summary>
	public int NumberOfNodesToTarget(NavGridNode _target)
	{
		return CalculatePathToTarget(currentNode, _target).Count;
	}
	
	/// <summary>
	/// returns the number of nodes to the node closest to the _target gameobject
	/// </summary>
	public int NumberOfNodesToTarget(GameObject _target)
	{
		if(navGrid.NodesByGameObject.ContainsKey(_target))
		{
			NavGridNode nodeTarget = navGrid.NodesByGameObject[_target];
			return CalculatePathToTarget(currentNode, nodeTarget).Count;
		}
		return 0;
	}

	/// <summary>
	/// returns the number of nodes to the node closest to the _target position
	/// </summary>
	public int NumberOfNodesToTarget(Vector3 _target)
	{
		NavGridNode nodeTarget = navGrid.ClosestNavGridNodeToPosition(_target);
		return CalculatePathToTarget(currentNode, nodeTarget).Count;
	}

#endregion
	
	private void FixedUpdate()
	{
		//lerp the position towards the current node's position
		transform.position = Vector3.Lerp(transform.position, currentNode.transform.position, lerpAmount);

		//if turnInDirecitonOfTravel is true, turn the gameobject so that its local z axis alligns with its direciton of travel
		if(turnInDirectionOfTravel)
		{
			Vector3 moveDirection = (currentNode.transform.position - transform.position).normalized;
			transform.LookAt(Vector3.Lerp(transform.position + transform.forward, transform.position + new Vector3(moveDirection.x, 0, moveDirection.z), turnLerpSpeed), Vector3.up);
		}
	}

	private void Start()
	{
		//if the navgrid exists, set the current node to the closest node to the agent's position, and set the position of the gameObject to position of the current node
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
