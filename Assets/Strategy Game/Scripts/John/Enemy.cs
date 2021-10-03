using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	private NavGridAgent agent;
	[SerializeField] private int movesPerTurn;
	private int movesRemaining;
	public bool HasFinishedTurn { get; private set; } = true;
	[SerializeField] private NavGridAgent playerAgent;

	[SerializeField] private ParticleSystem electricEffect;
	[SerializeField] private AudioSource soundEffect;

	private void Start()
	{
		agent = GetComponent<NavGridAgent>();
	}

	public void PerformAction()
	{
		NavGridNode targetNode = agent.AgentNavGrid.ClosestNavGridNodeToPosition(playerAgent.transform.position);
		NavGridNode nextNodeTowardTarget = agent.NextNodeTowardTarget(targetNode, true);
		if(nextNodeTowardTarget == null)
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
			movesRemaining--;
		}
		HasFinishedTurn = true;
	}
}
