using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action
{
    public abstract void PerformAction();
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int actionsAtStartOfTurn = 10;
    private int actionsRemaining;
    private TurnManager turnManager;

    private void Start()
    {
        turnManager = TurnManager.theTurnManager;
    }

    public void StartNewTurn()
    {
        actionsRemaining = actionsAtStartOfTurn;
    }

    public void EndTurn()
    {
        turnManager.StartNextTurn();
    }

    private void FixedUpdate()
    {
        if(actionsRemaining < 1)
        {
            //throw new NotImplementedException();
        }
    }
}
