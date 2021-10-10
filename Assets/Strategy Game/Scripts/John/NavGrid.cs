
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NavGrid : MonoBehaviour
{
	[SerializeField, Tooltip("The maximum number of nodes that will be generated")] private int maxNodes = 2048;
	[SerializeField, Tooltip("The layers that the nodes are allowed to be generated on top of")] private LayerMask allowedNodeLayers;
	public LayerMask AllowedNodeLayers => allowedNodeLayers;
	[SerializeField, Tooltip("Nodes that are near GameObjects with this tag will be marked as blocked")] private LayerMask pathBlockingLayers;
	public LayerMask PathBlockingLayers => pathBlockingLayers;
	[SerializeField, Tooltip("The height to raycast down from to generate nodes")] private float nodeCheckHeight = 2;
	public float NodeCheckHeight => nodeCheckHeight;
	[SerializeField, Tooltip("the distance the raycast used for detecting floor will go")] private float nodeCheckRayDistance = 100;
	public float NodeCheckRayDistance => nodeCheckRayDistance;
	[SerializeField, Tooltip("how far apart the grid nodes are from each other")] private float gridSize = 1;
	public float GridSize => gridSize;
	[SerializeField, Tooltip("The node prefab")] private GameObject nodePrefab;
	private List<NavGridNode> allNodes = new List<NavGridNode>();
	public List<NavGridNode> AllNodes => allNodes;
	public Dictionary<GameObject, NavGridNode> NodesByGameObject { get; private set; } = new Dictionary<GameObject, NavGridNode>();
	[SerializeField, Tooltip("Turn this off before entering play mode")] private bool showNodesInEditMode;
	
	/// <summary>
	/// returns the closest navgrid node to the given position
	/// </summary>
	public NavGridNode ClosestNavGridNodeToPosition(Vector3 _position)
	{
		//a simple funciton that returns the squared position from pos1 to pos2
		float SquaredDistance(Vector3 pos1, Vector3 pos2)
		{
			return Mathf.Pow(pos1.x - pos2.x, 2) + Mathf.Pow(pos1.y - pos2.y, 2) + Mathf.Pow(pos1.z - pos2.z, 2);
		}
		//set the current closest node to the first node in the list of all nodes, and set the closest distance ot infinity.
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
	
	/// <summary>
	/// place a navgrid node in the given position
	/// </summary>
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
	
	/// <summary>
	/// generate all nodes in the level by placing all the nodes neighbors recursively, similarly to how a flood fill works in paint.
	/// </summary>
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

	/// <summary>
	/// destroys all the nodes in the world. This should be used only in edit modes
	/// </summary>
	IEnumerator DestroyAllNodesInEditMode()
	{
		yield return null;
		if(!Application.isPlaying)
		{
			foreach(NavGridNode navGridNode in allNodes)
			{
				if(navGridNode)
				{
					DestroyImmediate(navGridNode.gameObject);
				}
			}
			while(transform.childCount > 0)
			{
				foreach(Transform child in transform)
				{
					DestroyImmediate(child.gameObject);
				}
			}
			allNodes.Clear();
		}
	}
	
	/// <summary>
	/// regenerates all the nodes in the world. this should only be used in edit mode
	/// </summary>
	/// <returns></returns>
	IEnumerator RegenerateAllNodesInEditMode()
	{
		//wait until the next frame
		yield return null;
		//if the applicaiton isn't playing (it is in edit mode), destroy all nodes and regenerate them.
		if(!Application.isPlaying)
		{
			foreach(NavGridNode navGridNode in allNodes)
			{
				if(navGridNode)
				{
					DestroyImmediate(navGridNode.gameObject);
				}
			}
			allNodes.Clear();
			while(transform.childCount > 0)
			{
				foreach(Transform child in transform)
				{
					DestroyImmediate(child.gameObject);
				}
			}
			GenerateNodes();
		}
	}
	
	private void OnValidate()
	{
		if(nodePrefab && !nodePrefab.GetComponent<NavGridNode>())
		{
			nodePrefab = null;
		}
		if(!Application.isPlaying)
		{
			if(showNodesInEditMode)
			{
				StartCoroutine(RegenerateAllNodesInEditMode());
			}
			else
			{
				StartCoroutine(DestroyAllNodesInEditMode());
			}
		}
	}

	private void Start()
	{
		//generate all the nodes when the game starts.
		GenerateNodes();
	}
}
