using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyGame.Player
{
    /// <summary>
    /// Handles the data to picking up and the trap colliding with the enemy and exploding.
    /// </summary>
    public class Trap : MonoBehaviour
    {
        public new ParticleSystem particleSystem;
        [Tooltip("For when the trap is set and not a pickup.")]
        public bool setTrap = false;
        private MeshRenderer rend;
        
        // Start is called before the first frame update
        void Start()
        {
            rend = GetComponent<MeshRenderer>();
        }

        
        /// <summary>
        /// Handles collisons with the trap by both player and enemy.
        /// </summary>
        /// <param name="_other">The other collider in the collision</param>
        private void OnCollisionEnter(Collision _other) //This might work better as a trigger??
        {
            if(_other.collider.tag == "Player" && !setTrap)
            {
                Debug.Log("Player has picked up a trap");
                
                //todo Increase trap count on GameManager.
                
                
                //Destroy object.
                Destroy(this.gameObject);
            }
            
            if(_other.collider.tag == "Enemy" && setTrap)
            {
                Debug.Log("Enemy has hit a set trap");
                // Turn off the renderer
                rend.enabled = false;
                // Instantiate the particle system and set its position.
                Instantiate(particleSystem,this.transform);
                particleSystem.transform.position = this.transform.position;
                particleSystem.Play();
                
                //Destroy object and particle system.
                Destroy(this.gameObject,0.35f);
            }
            
            
        }
    }
}