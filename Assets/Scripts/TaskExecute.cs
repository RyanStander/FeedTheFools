using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskExecute : MonoBehaviour
{
    public Caveman caveman;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);
            
            if (hit.collider != null && hit.collider.gameObject.CompareTag("caveman"))
            {
                caveman = hit.collider.gameObject.GetComponent<Caveman>();
                caveman.MoveToTarget(caveman.treeResourceTestPos.position);
            }
        }
    }
}
