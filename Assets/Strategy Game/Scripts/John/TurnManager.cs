using StrategyGame.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public static TurnManager theTurnManager;
    private PlayerController player;
    public List<Enemy> allEnemies = new List<Enemy>();
    [SerializeField] private PopupUIControl popupUIControl;

    public void Awake()
    {
        theTurnManager = this;
    }

    public void Start()
    {
        player = FindObjectOfType<PlayerController>();
        allEnemies = FindObjectsOfType<Enemy>().ToList();
    }

    public void StartNextTurn()
    {
        if(player.isPlayerTurn)
        {
            player.actionsDoneThisTurn.Clear();
            StartCoroutine(RunNextTurn());
        }
    }

    private IEnumerator RunNextTurn()
    {
        player.isPlayerTurn = false;
        foreach(var enemy in allEnemies)
        {
            enemy.TakeTurn();
            yield return new WaitForSeconds(0.1f);
            while(!enemy.HasFinishedTurn)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        player.isPlayerTurn = true;
    }

    /// <summary>
    /// for now this just reloads the game
    /// </summary>
    public void LooseGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        popupUIControl.ShowPopup(popupUIControl.losePopup);
        player.isPlayerTurn = false;
    }
}