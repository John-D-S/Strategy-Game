using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavGrid : MonoBehaviour
{
	[SerializeField] private int maxNodes = 2048;
	[SerializeField] private LayerMask allowedNodeLayers;
	public LayerMask AllowedNodeLayers => allowedNodeLayers;
	[SerializeField] private float nodeCheckHeight = 2;
	public float NodeCheckHeight => nodeCheckHeight;
	[SerializeField] private float nodeCheckRayDistance = 100;
	public float NodeCheckRayDistance => nodeCheckRayDistance;
	[SerializeField] private float gridSize = 1;
	public float GridSize => gridSize;
	[SerializeField] private GameObject nodePrefab;
	private List<Node> allNodes = new List<Node>();

	private void OnValidate()
	{
		if(nodePrefab && !nodePrefab.GetComponent<Node>())
		{
			nodePrefab = null;
		}
	}

	private Node PlaceNode(Vector3 _position)
	{
		GameObject instantiatedNode = Instantiate(nodePrefab, _position, Quaternion.identity, transform);
		Node nodeComponent = instantiatedNode.GetComponent<Node>();

		nodeComponent.navGrid = this;
		nodeComponent.LinkSurroundingNodes();
		allNodes.Add(nodeComponent);
		return nodeComponent;
	}
	
	private void GenerateNodes()
	{
		int numberOfNodesPlaced = 1;
		Queue<Node> newlyPlacedNodes = new Queue<Node>();
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

	private void Start()
	{
		allNodes.Clear();
		GenerateNodes();
	}
}
