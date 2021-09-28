using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public static TurnManager theTurnManager;
    public List<Enemy> allEnemies = new List<Enemy>();

    public void Awake()
    {
        theTurnManager = this;
    }

    public void Start()
    {
        allEnemies = FindObjectsOfType<Enemy>().ToList();
    }

    public void StartNextTurn()
    {
        StartCoroutine(RunNextTurn());
    }

    private IEnumerator RunNextTurn()
    {
        foreach(var enemy in allEnemies)
        {
            enemy.TakeTurn();
            yield return new WaitForSeconds(0.1f);
            while(!enemy.HasFinishedTurn)
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }

    /// <summary>
    /// for now this just reloads the game
    /// </summary>
    public void LooseGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
