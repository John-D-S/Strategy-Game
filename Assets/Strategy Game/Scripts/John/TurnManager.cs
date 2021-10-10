using StrategyGame.UI;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager theTurnManager;
    private PlayerController player;
    public List<Enemy> allEnemies = new List<Enemy>();
    [SerializeField, Tooltip("The popup UI control thing")] private PopupUIControl popupUIControl;
    [SerializeField, Tooltip("The text that shows that it is the player's turn")] private GameObject yourTurnText;

    public void Awake()
    {
        //sets the turnmanager to this so that other things can reference it
        theTurnManager = this;
    }

    public void Start()
    {
        //initialize the player and enemies
        player = FindObjectOfType<PlayerController>();
        allEnemies = FindObjectsOfType<Enemy>().ToList();
    }

    public void StartNextTurn()
    {
        StartCoroutine(RunNextTurn());
    }

    /// <summary>
    /// runs the next turn
    /// </summary>
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
        YourTurnUIOn();
        Debug.Log("Your turn.");
        player.isPlayerTurn = true;
    }
    
    /// <summary>
    /// Turns on the UI informing the player its now their turn.
    /// </summary>
    private void YourTurnUIOn()
    {
        yourTurnText.SetActive(true);
        Invoke(nameof(YourTurnUIOff), 1f);
    }

    /// <summary>
    /// Turns off the UI informing the player its their turn.
    /// </summary>
    private void YourTurnUIOff() => yourTurnText.SetActive(false);
    

    /// <summary>
    /// Shows the end game popup.
    /// </summary>
    public void LooseGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        popupUIControl.ShowPopup(popupUIControl.losePopup);
        player.isPlayerTurn = false;
    }
}