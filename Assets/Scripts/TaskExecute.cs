using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskExecute : MonoBehaviour
{
    public Caveman currentSelectedCaveman;
    public Transform woodResourcePos;
    public Transform stoneResourcePos;
    public Transform coalResourcePos;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);
            
            if (hit.collider != null && hit.collider.gameObject.CompareTag("caveman"))
            {
                currentSelectedCaveman = hit.collider.gameObject.GetComponent<Caveman>();
                Debug.Log("Caveman selected. Press: 1=Wood, 2=Stone, 3=Coal, 4=Breed");
            }
        }

        if (currentSelectedCaveman != null)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1) && !currentSelectedCaveman.isMoving && currentSelectedCaveman.state == Caveman.CavemanState.Idle)
            {
                currentSelectedCaveman.SetGatheringTask(woodResourcePos.position, Caveman.ResourceType.Wood);
            }
            if(Input.GetKeyDown(KeyCode.Alpha2) && !currentSelectedCaveman.isMoving && currentSelectedCaveman.state == Caveman.CavemanState.Idle)
            {
                currentSelectedCaveman.SetGatheringTask(stoneResourcePos.position, Caveman.ResourceType.Stone);
            }
            if(Input.GetKeyDown(KeyCode.Alpha3) && !currentSelectedCaveman.isMoving && currentSelectedCaveman.state == Caveman.CavemanState.Idle)
            {
                currentSelectedCaveman.SetGatheringTask(coalResourcePos.position, Caveman.ResourceType.Coal);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && !currentSelectedCaveman.isMoving && currentSelectedCaveman.state == Caveman.CavemanState.Idle)
            {
                currentSelectedCaveman.SetBreedingTask();
            }
        }
    }
}
