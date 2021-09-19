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
    public class ExtraMovePickup : MonoBehaviour
    {
        [SerializeField] private int extraMoves = 0;
        [SerializeField] private int min = 1;
        [SerializeField] private int max = 5;
        
        // Start is called before the first frame update
        void Start()
        {
            extraMoves = Random.Range(min, max);
        }

        /// <summary>
        /// Handles the collisions with objects in the scene
        /// </summary>
        /// <param name="_other">The other object colliding with this</param>
        private void OnCollisionEnter(Collision _other)
        {
            if(_other.collider.tag == "Player")
            {
                Debug.Log($"Player now has an extra {extraMoves} moves");
                
                //todo increase action points/moves in gamemanager
                
                Destroy(this.gameObject);
            }
        }
    }
}