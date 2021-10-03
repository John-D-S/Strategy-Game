using StrategyGame.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace StrategyGame.Player
{
    /// <summary>
    /// This class handles the extra move pickup and adding more action points for the player to use.
    /// </summary>
    public class ExtraMovePickup : Item
    {
        [SerializeField] private int extraMoves = 3;

        public override void PickUpItem(PlayerController _playerController)
        {
            _playerController.additionalActions += extraMoves;
            gameObject.SetActive(false);
        }

        public override void UndoPickUpItem(PlayerController _playerController)
        {
            gameObject.SetActive(true);
            _playerController.additionalActions -= extraMoves;
        }
    }
}