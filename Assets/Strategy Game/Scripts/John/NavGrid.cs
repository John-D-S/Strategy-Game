using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor.Experimental.GraphView;

using UnityEngine;

public class NavGrid : MonoBehaviour
{
	[SerializeField] private int maxNodes = 2048;
	[SerializeField] private LayerMask allowedNodeLayers;
	public LayerMask AllowedNodeLayers => allowedNodeLayers;
	[SerializeField] private LayerMask pathBlockingLayers;
	public LayerMask PathBlockingLayers => pathBlockingLayers;
	[SerializeField] private float nodeCheckHeight = 2;
	public float NodeCheckHeight => nodeCheckHeight;
	[SerializeField] private float nodeCheckRayDistance = 100;
	public float NodeCheckRayDistance => nodeCheckRayDistance;
	[SerializeField] private float gridSize = 1;
	public float GridSize => gridSize;
	[SerializeField] private GameObject nodePrefab;
	private List<NavGridNode> allNodes = new List<NavGridNode>();
	public Dictionary<GameObject, NavGridNode> NodesByGameObject { get; private set; } = new Dictionary<GameObject, NavGridNode>();

	public NavGridNode ClosestNavGridNodeToPosition(Vector3 _position)
	{
		float SquaredDistance(Vector3 pos1, Vector3 pos2)
		{
			return Mathf.Pow(pos1.x - pos2.x, 2) + Mathf.Pow(pos1.y - pos2.y, 2) + Mathf.Pow(pos1.z - pos2.z, 2);
		}
		NavGridNode currentClosestNode = allNodes[0];
		float smallestDistance = Mathf.Infinity;
		foreach(NavGridNode gridNode in allNodes)
		{
			float distanceToGridNode = SquaredDistance(_position, gridNode.transform.position);
			if(distanceToGridNode < smallestDistance)
			{
				smallestDistance = distanceToGridNode;
				currentClosestNode = gridNode;
			}
		}
		return currentClosestNode;
	}
	
	private NavGridNode PlaceNode(Vector3 _position)
	{
		GameObject instantiatedNode = Instantiate(nodePrefab, _position, Quaternion.identity, transform);
		NavGridNode navGridNodeComponent = instantiatedNode.GetComponent<NavGridNode>();

		NodesByGameObject[instantiatedNode] = navGridNodeComponent;
		navGridNodeComponent.navGrid = this;
		navGridNodeComponent.LinkSurroundingNodes();
		allNodes.Add(navGridNodeComponent);
		return navGridNodeComponent;
	}
	
	private void GenerateNodes()
	{
		int numberOfNodesPlaced = 1;
		Queue<NavGridNode> newlyPlacedNodes = new Queue<NavGridNode>();
		newlyPlacedNodes.Enqueue(PlaceNode(transform.position));
		while(newlyPlacedNodes.Count > 0 && numberOfNodesPlaced < maxNodes)
		{
			List<Vector3> nextNodePositions = newlyPlacedNodes.Dequeue().EmptyNeighborPositions();
			foreach(Vector3 nextNodePosition in nextNodePositions)
			{
				if(numberOfNodesPlaced < maxNodes)
				{
					newlyPlacedNodes.Enqueue(PlaceNode(nextNodePosition));
					numberOfNodesPlaced++;
				}
			}
		}
	}
	
	private void OnValidate()
	{
		if(nodePrefab && !nodePrefab.GetComponent<NavGridNode>())
		{
			nodePrefab = null;
		}
	}

	private void Start()
	{
		allNodes.Clear();
		GenerateNodes();
	}
}
