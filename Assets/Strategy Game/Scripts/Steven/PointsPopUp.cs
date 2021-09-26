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