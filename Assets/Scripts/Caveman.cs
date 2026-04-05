using Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
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
        Hungry,
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

    public bool isMoving = false;
    public bool arrivedToTarget;
    [SerializeField] private float arrivalDistance = 0.5f;
    [SerializeField] private int maxCarryCapacity = 3;

    public int woodCount = 0;
    public int stoneCount = 0;
    public int coalCount = 0;

    public Transform treeResourceTestPos;
    public bool isPerformingTask;
    private ResourceType currentHarvestResource;
    private bool returningHome = false;
    public CavemanNav nav;

    public bool isHungry = false;
    public float hungerMeter = 100;

    void Start()
    {
        hungerMeter = 100;
        nav = GetComponent<CavemanNav>();
        state = CavemanState.Idle;
    }

    void Update()
    {
        hungerMeter -= Time.deltaTime;
        if (hungerMeter <= 0)
        {
            hungerMeter = 0;
            isHungry = true;
        }

        if (isMoving && !arrivedToTarget)
        {
            if (Vector3.Distance(transform.position, targetPosition) < arrivalDistance)
            {
                isMoving = false;
                arrivedToTarget = true;
                
                if (returningHome)
                {
                    DepositResources();
                    ResetState();
                    returningHome = false;
                }
                else
                {
                    ExecuteTask(task);
                }
            }
        }

        if(isHungry && state != CavemanState.PerformingTask)
        {
            task = CavemanTasks.Hungry;
            Debug.Log("Caveman is hungry!");
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

    public void SetGatheringTask(Vector3 targetPos, ResourceType resource)
    {
        task = CavemanTasks.Gathering;
        currentHarvestResource = resource;
        targetPosition = targetPos;
        isMoving = true;
        state = CavemanState.Walking;
        arrivedToTarget = false;
        nav.GoToPosition(targetPos);
    }

    private IEnumerator GatheringTask(ResourceType resource)
    {
        isPerformingTask = true;
        state = CavemanState.PerformingTask;
        yield return HarvestResource(resource, 2f);
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
        targetPosition = homePosition.position;
        isMoving = true;
        state = CavemanState.Walking;
        arrivedToTarget = false;
        returningHome = true;
        nav.GoToPosition(homePosition.position);
    }

    private void ResetState()
    {
        task = CavemanTasks.Gathering;
        state = CavemanState.Idle;
        isMoving = false;
        isPerformingTask = false;
    }

    private int GetTotalCarry()
    {
        return woodCount + stoneCount + coalCount;
    }

    private IEnumerator HarvestResource(ResourceType resourceType, float harvestTime)
    {
        while (GetTotalCarry() < maxCarryCapacity)
        {
            yield return new WaitForSeconds(harvestTime);
            Debug.Log("harvested 1 resource");
            AddResource(resourceType, 1);
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

    public void DepositResources()
    {
        HomeManager.Instance.AddResources(woodCount, stoneCount, coalCount);
        Debug.Log("deposited resources wood: " + woodCount + ", stone: " + stoneCount + ", coal: " + coalCount);
        woodCount = 0;
        stoneCount = 0;
        coalCount = 0;
    }
}
