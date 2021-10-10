using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NavGridAgent))]
public class SimpleNavigator : MonoBehaviour
{
    private NavGridAgent navGridAgent;
    
    void Start()
    {
        //initialise the navGridAgent
        navGridAgent = GetComponent<NavGridAgent>();
    }

    void Update()
    {
        //if the mouse button is pressed, move the agent to the node under the cursor if there is one.
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();
            if(Physics.Raycast(ray, out hit, 1000))
            {
                navGridAgent.MoveToTarget(hit.collider.gameObject);
            }
        }
    }
}
