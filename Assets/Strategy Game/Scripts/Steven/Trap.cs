using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyGame.Player
{
    /// <summary>
    /// Handles the data to picking up and the trap colliding with the enemy and exploding.
    /// </summary>
    [RequireComponent(typeof(Item))]
    public class Trap : MonoBehaviour
    {
        [SerializeField, Tooltip("The amount of moves to take away from the enemy when they step on the trap.")] private int movesToTakeFromEnemy = 100;
        public new ParticleSystem particleSystem;
        //For when the trap is set and not a pickup.
        public bool TrapIsActivated => item.isActivated;
        private Item item;
        private MeshRenderer rend;
        public static List<Trap> allTraps = new List<Trap>();

        private void Awake()
        {
            allTraps.Clear();
        }

        // Start is called before the first frame update
        void Start()
        {
            item = GetComponent<Item>();
            rend = GetComponent<MeshRenderer>();
            allTraps.Add(this);
        }

        public static Trap ActivatedTrapNearPosition(float _maxDistance, Vector3 _position)
        {
            foreach(Trap trap in allTraps)
            {
                if(trap != null)
                {
                    if(Vector3.Distance(_position, trap.transform.position) < _maxDistance)
                    {
                        if(trap.TrapIsActivated)
                        {
                            return trap;
                        }
                    }			
                }
            }
            return null;
        }

        public void StepOnTrap(Enemy _enemy)
        {
            Debug.Log("Enemy has hit a set trap");
            // Turn off the renderer
            rend.enabled = false;
            // Play the particle system.
            particleSystem.Play();
                    
            //todo play explosion sound
            _enemy.movesRemaining = Mathf.Clamp(_enemy.movesRemaining - movesToTakeFromEnemy, 0, _enemy.movesRemaining);
                    
            //Destroy object and particle system.
            Destroy(this.gameObject,0.35f);
            allTraps.Remove(this);
        }
    }
}