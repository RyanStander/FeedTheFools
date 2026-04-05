using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Caveman : MonoBehaviour
{
    public enum CavemanState
    {
        Idle,
        Walking,
        PerformingTask
    }
    public CavemanState state;

    public enum CavemanTasks
    {
        Gathering,
        Hunting,
        Building,
        Breeding
    }
    public CavemanTasks task;

    public enum ResourceType
    {
        Wood,
        Stone,
        Coal
    }

    public Vector3 currentPosition;
    public Vector3 targetPosition;
    public Transform homePosition;

    private bool isMoving = false;
    public bool arrivedToTarget;
    [SerializeField] private float moveSpeed = 2;

    public int woodCount = 0;
    public int stoneCount = 0;
    public int coalCount = 0;

    public Transform treeResourceTestPos;
    public bool isPerformingTask;
    private ResourceType currentHarvestResource;

    void Start()
    {
        
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                isMoving = false;
                arrivedToTarget = true;
                ExecuteTask(task);
            }
        }
    }

    public void ExecuteTask(CavemanTasks task)
    {
        switch(task)
        {
            case CavemanTasks.Gathering:
                StartCoroutine(GatheringTask(currentHarvestResource));
                break;
            case CavemanTasks.Hunting:
                StartCoroutine(HuntingTask());
                break;
            case CavemanTasks.Building:
                StartCoroutine(BuildingTask());
                break;
            case CavemanTasks.Breeding:
                StartCoroutine(BreedingTask());
                break;
        }
    }

    public void MoveToTarget(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
        isMoving = true;
        state = CavemanState.Walking;
        arrivedToTarget = false;
    }

    public void SetGatheringTask(Vector3 targetPos, ResourceType resource) //i really wanted this to be dynamic and flexible for the resources we end up using, call this on button press perhaps and pass in the resource type and target position for that resource
    {
        task = CavemanTasks.Gathering;
        currentHarvestResource = resource;
        MoveToTarget(targetPos);
    }

    private IEnumerator GatheringTask(ResourceType resource)
    {
        isPerformingTask = true;
        state= CavemanState.PerformingTask;
        yield return HarvestResource(resource, 3, 2f);
        isPerformingTask = false;
        ReturnHome();
    }

    private IEnumerator HuntingTask()
    {
        isPerformingTask = true;
        yield return new WaitForSeconds(2f);
        isPerformingTask = false;
        ReturnHome();
    }

    private IEnumerator BuildingTask()
    {
        isPerformingTask = true;
        yield return new WaitForSeconds(2f);
        isPerformingTask = false;
        ReturnHome();
    }

    private IEnumerator BreedingTask()
    {
        isPerformingTask = true;
        yield return new WaitForSeconds(2f);
        isPerformingTask = false;
        ReturnHome();
    }

    public void ReturnHome()
    {
        MoveToTarget(homePosition.position);
        state = CavemanState.Walking;
        if (this.transform.position == homePosition.position)
        {
            //reset task and state
            task = CavemanTasks.Gathering; //default to gathering for now we can change this later when we have more tasks
            state = CavemanState.Idle;
            isMoving = false;
            isPerformingTask = false;
        }
    }

    private IEnumerator HarvestResource(ResourceType resourceType, int limit, float harvestTime)
    {
        while(GetResourceCount(resourceType) < limit) 
        {
            yield return new WaitForSeconds(harvestTime);
            AddResource(resourceType, 1);
        }
    }

    private int GetResourceCount(ResourceType resourceType)
    {
        switch(resourceType)
        {
            case ResourceType.Wood:
                return woodCount;
            case ResourceType.Stone:
                return stoneCount;
            case ResourceType.Coal:
                return coalCount;
            default:
                return 0;
        }
    }

    private void AddResource(ResourceType resourceType, int amount)
    {
        switch(resourceType)
        {
            case ResourceType.Wood:
                woodCount += amount;
                break;
            case ResourceType.Stone:
                stoneCount += amount;
                break;
            case ResourceType.Coal:
                coalCount += amount;
                break;
        }
    }
}
