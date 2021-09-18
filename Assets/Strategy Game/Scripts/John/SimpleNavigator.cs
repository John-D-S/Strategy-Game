using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NavGridAgent))]
public class SimpleNavigator : MonoBehaviour
{
    private NavGridAgent navGridAgent;
    
    // Start is called before the first frame update
    void Start()
    {
        navGridAgent = GetComponent<NavGridAgent>();
    }

    // Update is called once per frame
    void Update()
    {
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
