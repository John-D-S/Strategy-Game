using StrategyGame.Player;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavGridAgent))]
public class Enemy : MonoBehaviour
{
	private NavGridAgent agent;
	[SerializeField] private int movesPerTurn;
	[HideInInspector] public int movesRemaining;
	public bool HasFinishedTurn { get; private set; } = true;
	private NavGridAgent playerAgent;

	[SerializeField] private ParticleSystem electricEffect;
	[SerializeField] private AudioSource soundEffect;

	private void Start()
	{
		gameObject.layer = LayerMask.NameToLayer("Enemy");
		agent = GetComponent<NavGridAgent>();
	}

	private void TryStepOnTrap()
	{
		Trap steppedOnTrap = Trap.ActivatedTrapNearPosition(agent.AgentNavGrid.GridSize * 0.2f, agent.CurrentNode.transform.position);
		Debug.Log("tried to step on trap");
		if(steppedOnTrap)
		{
			steppedOnTrap.StepOnTrap(this);
		}
	}
	
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
			soundEffect.Play();
			Invoke(nameof(LoseGame), 0.75f);
		}
		agent.MoveTowards(nextNodeTowardTarget);
	}

	private void LoseGame() => TurnManager.theTurnManager.LooseGame();

	public void TakeTurn()
	{
		HasFinishedTurn = false;
		StartCoroutine(RunTurn());
	}

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
