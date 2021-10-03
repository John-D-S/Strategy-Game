using StrategyGame.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyGame
{
    /// <summary>
    /// This class handles the exit of the level and the win condition.
    /// </summary>
    public class Exit : MonoBehaviour
    {
        private Ray exitRay;
        [SerializeField] private float radius = 5;
        public Collider[] hits = new Collider[0];
        [SerializeField] private PopupUIControl popupUIControl;
        [SerializeField] private AudioRunner audioRunner;
        
        
        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating(nameof(CheckForPlayer), 1f, 1f);
        }

        private void CheckForPlayer()
        {
            hits = Physics.OverlapSphere(transform.position, radius);
            foreach(Collider _collider in hits)
            {
                if(_collider.tag == "Player")
                {
                    popupUIControl.ShowPopup(popupUIControl.winPopup);
                    audioRunner.PlayAudio(audioRunner.effect);
                }
            }
            
        }

        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}