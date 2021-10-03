using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace StrategyGame.UI
{
    /// <summary>
    /// This class handles assigning the value of extra points to the UI popup.
    /// </summary>
    public class PointsPopUp : MonoBehaviour
    {
        private TMP_Text pointsText;

        private void Start()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// To be called from GameManager to assign the points to the popup and turn it on so that is animates
        /// then turn off again ready for the next call.
        /// </summary>
        /// <param name="_points">The points to display on the HUD</param>
        public void RunPopUp(int _points)
        {
            AssignPoints(_points);
            gameObject.SetActive(true);
            
            Invoke(nameof(TurnOff), 1.5f);
        }
        
        /// <summary>
        /// Turns off the game object ready for the next call.
        /// </summary>
        private void TurnOff() => gameObject.SetActive(false);
        
        /// <summary>
        /// Assigns the passed value to the UI text.
        /// </summary>
        /// <param name="_points">the value of extra points to be shown.</param>
        public void AssignPoints(int _points)
        {
            pointsText = GetComponent<TMP_Text>();
            if(pointsText != null)
                pointsText.text = $"+ {_points}";
            else
                Debug.Log("No text component found.");
        }
        
        
    }
}