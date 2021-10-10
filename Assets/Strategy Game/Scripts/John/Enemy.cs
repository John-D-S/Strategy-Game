using StrategyGame.Player;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavGridAgent))]
public class Enemy : MonoBehaviour
{
	private NavGridAgent agent;
	[SerializeField, Tooltip("The number of actions a player can take per turn.")] private int movesPerTurn;
	[HideInInspector] public int movesRemaining;
	public bool HasFinishedTurn { get; private set; } = true;
	private NavGridAgent playerAgent;

	[SerializeField, Tooltip("The particle effect that plays when the enemy zaps the player.")] private ParticleSystem electricEffect;
	[SerializeField, Tooltip("The audio source that plays the sound effect.")] private AudioSource electricSoundEffect;
	[SerializeField] private AudioSource explosionEffect;

	private void Start()
	{
		//move the layer to the enemy layer
		gameObject.layer = LayerMask.NameToLayer("Enemy");
		//initialize the navgridAgent
		agent = GetComponent<NavGridAgent>();
	}

	/// <summary>
	/// tries to step on a trap on the current tile
	/// </summary>
	private void TryStepOnTrap()
	{
		Trap steppedOnTrap = Trap.ActivatedTrapNearPosition(agent.AgentNavGrid.GridSize * 0.2f, agent.CurrentNode.transform.position);
		if(steppedOnTrap)
		{
			steppedOnTrap.StepOnTrap(this);
			explosionEffect.Play();
		}
	}
	
	/// <summary>
	/// moves the enemy toward the player and end the game if it catches the player
	/// </summary>
	public void PerformAction()
	{
		NavGridNode targetNode = PlayerController.thePlayerController.PlayerAgent.CurrentNode;
		NavGridNode nextNodeTowardTarget = agent.NextNodeTowardTarget(targetNode, true);
		if(nextNodeTowardTarget == null || nextNodeTowardTarget.IsBlocked)
		{
			nextNodeTowardTarget = agent.NextNodeTowardTarget(targetNode, false);
			if(nextNodeTowardTarget.IsBlocked)
			{
				return;
			}
		}
		if(nextNodeTowardTarget == targetNode)
		{
			electricEffect.Play();
			electricSoundEffect.Play();
			Invoke(nameof(LoseGame), 0.75f);
		}
		agent.MoveTowards(nextNodeTowardTarget);
	}

	/// <summary>
	/// points toward LooseGame() in the turn manager
	/// </summary>
	private void LoseGame() => TurnManager.theTurnManager.LooseGame();

	/// <summary>
	/// sets hasfinishedturn to true
	/// </summary>
	public void TakeTurn()
	{
		HasFinishedTurn = false;
		StartCoroutine(RunTurn());
	}

	/// <summary>
	/// runs the player's turn
	/// </summary>
	private IEnumerator RunTurn()
	{
		HasFinishedTurn = false;
		movesRemaining = movesPerTurn;
		while(movesRemaining > 0)
		{
			yield return new WaitForSeconds(0.5f);
			PerformAction();
			TryStepOnTrap();
			movesRemaining--;
		}
		HasFinishedTurn = true;
	}
}
