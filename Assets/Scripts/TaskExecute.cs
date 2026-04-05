using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskExecute : MonoBehaviour
{
    public Caveman currentSelectedCaveman;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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

        if (currentSelectedCaveman != null && currentSelectedCaveman.state == Caveman.CavemanState.Idle)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                currentSelectedCaveman.SetGatheringTask(Caveman.ResourceType.Wood);

            if (Input.GetKeyDown(KeyCode.Alpha2))
                currentSelectedCaveman.SetGatheringTask(Caveman.ResourceType.Stone);

            if (Input.GetKeyDown(KeyCode.Alpha3))
                currentSelectedCaveman.SetGatheringTask(Caveman.ResourceType.Coal);

            if (Input.GetKeyDown(KeyCode.Alpha4))
                currentSelectedCaveman.SetBreedingTask();
        }
    }
}
