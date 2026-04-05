using Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        Home,
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

    public GameObject cavemanPrefab;
    private float birthTime;
    [SerializeField] private float breedCooldown = 30f;

    public List<Tree> availableTrees = new List<Tree>();
    public List<Coal> availableCoals = new List<Coal>();
    public List<Stone> availableStones = new List<Stone>();
    private Tree currentTree;
    private Coal currentCoal;
    private Stone currentStone;

    void Start()
    {
        hungerMeter = 100;
        nav = GetComponent<CavemanNav>();
        state = CavemanState.Idle;
        birthTime = Time.time;
        RefreshTreeList();
        RefreshCoalList();
        RefreshStoneList();
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
                    if (task != CavemanTasks.Breeding && task != CavemanTasks.Hungry)
                    {
                        DepositResources();
                    }
                    
                    if (task != CavemanTasks.Breeding && task != CavemanTasks.Hungry)
                    {
                        ResetState();
                    }
                    else
                    {
                        ExecuteTask(task);
                    }
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
            case CavemanTasks.Hungry:
                StartCoroutine(HungryTask());
                break;
        }
    }

    public void SetGatheringTask(Vector3 targetPos, ResourceType resource)
    {
        task = CavemanTasks.Gathering;
        currentHarvestResource = resource;
        RefreshTreeList();
        FindAndGoToNearestTree();
    }

    private void RefreshTreeList()
    {
        availableTrees.Clear();
        Tree[] allTrees = FindObjectsOfType<Tree>();
        availableTrees.AddRange(allTrees);
    }

    private void RefreshStoneList()
    {
        availableStones.Clear();
        Stone[] allStones = FindObjectsOfType<Stone>();
        availableStones.AddRange(allStones);
    }

    private void RefreshCoalList()
    {
        availableCoals.Clear();
        Coal[] allCoals = FindObjectsOfType<Coal>();
        availableCoals.AddRange(allCoals);
    }

    private void FindAndGoToNearestTree()
    {
        RefreshTreeList();
        
        if (availableTrees.Count == 0)
        {
            Debug.Log("No trees available!");
            return;
        }

        currentTree = availableTrees[0];
        float nearestDistance = Vector3.Distance(transform.position, currentTree.transform.position);

        foreach (var tree in availableTrees)
        {
            if (tree == null) continue;
            
            float distance = Vector3.Distance(transform.position, tree.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                currentTree = tree;
            }
        }

        if (currentTree != null)
        {
            targetPosition = currentTree.transform.position;
            isMoving = true;
            state = CavemanState.Walking;
            arrivedToTarget = false;
            nav.GoToPosition(targetPosition);
            Debug.Log("Going to nearest tree at " + nearestDistance);
        }
    }

    private void FindAndGoToNearestStone()
    {
        RefreshStoneList();

        if (availableStones.Count == 0)
        {
            Debug.Log("No stones available");
            return;
        }

        currentStone = availableStones[0];
        float nearestDistance = Vector3.Distance(transform.position, currentStone.transform.position);

        foreach (var stone in availableStones)
        {
            if (stone == null) continue;

            float distance = Vector3.Distance(transform.position, stone.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                currentStone = stone;
            }
        }

        if (currentStone != null)
        {
            targetPosition = currentStone.transform.position;
            isMoving = true;
            state = CavemanState.Walking;
            arrivedToTarget = false;
            nav.GoToPosition(targetPosition);
            Debug.Log("Going to nearest stone at " + nearestDistance);
        }
    }

    private void FindAndGoToNearestCoal()
    {
        RefreshCoalList();

        if (availableCoals.Count == 0)
        {
            Debug.Log("No coals available");
            return;
        }

        currentCoal = availableCoals[0];
        float nearestDistance = Vector3.Distance(transform.position, currentCoal.transform.position);

        foreach (var coal in availableCoals)
        {
            if (coal == null) continue;

            float distance = Vector3.Distance(transform.position, coal.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                currentCoal = coal;
            }
        }

        if (currentCoal != null)
        {
            targetPosition = currentCoal.transform.position;
            isMoving = true;
            state = CavemanState.Walking;
            arrivedToTarget = false;
            nav.GoToPosition(targetPosition);
            Debug.Log("Going to nearest coal at distance" + nearestDistance);
        }
    }

    public void SetBreedingTask()
    {
        float timeSinceBirth = Time.time - birthTime;
        if (timeSinceBirth < breedCooldown)
        {
            Debug.Log("currentSelectedCaveman is too young to breed");
            return;
        }

        task = CavemanTasks.Breeding;
        ReturnHome();
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
        Debug.Log("BREEEEEDING");
        isPerformingTask = true;
        state = CavemanState.PerformingTask;
        int breedTime = UnityEngine.Random.Range(3, 10);
        yield return new WaitForSeconds(breedTime);
        var newCaveman = Instantiate(cavemanPrefab, transform.position, Quaternion.identity);
        newCaveman.transform.position = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);
        birthTime = Time.time;
        isPerformingTask = false;
        ReturnHome();
    }

    private IEnumerator HungryTask()
    {
        isPerformingTask = true;
        state = CavemanState.PerformingTask;
        yield return new WaitForSeconds(2f);
        hungerMeter = 100;
        isHungry = false;
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
        task = CavemanTasks.Home;
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
        int initialCarry = GetTotalCarry();

        while (GetTotalCarry() < maxCarryCapacity)
        {
            switch (resourceType)
            {
                case ResourceType.Wood:
                    FindAndGoToNearestTree();
                    while (currentTree != null && Vector3.Distance(transform.position, currentTree.transform.position) > 0.7f)
                        yield return null;
                    break;
                case ResourceType.Stone:
                    FindAndGoToNearestStone();
                    while (currentStone != null && Vector3.Distance(transform.position, currentStone.transform.position) > 0.7f)
                        yield return null;
                    break;
                case ResourceType.Coal:
                    FindAndGoToNearestCoal();
                    while (currentCoal != null && Vector3.Distance(transform.position, currentCoal.transform.position) > 0.7f)
                        yield return null;
                    break;
            }

            yield return new WaitForSeconds(harvestTime);

            switch (resourceType)
            {
                case ResourceType.Wood:
                    if (currentTree != null)
                    {
                        AddResource(ResourceType.Wood, 1);
                        currentTree.HarvestResource();
                    }
                    break;
                case ResourceType.Stone:
                    if (currentStone != null)
                    {
                        AddResource(ResourceType.Stone, 1);
                        currentStone.HarvestResource();
                    }
                    break;
                case ResourceType.Coal:
                    if (currentCoal != null)
                    {
                        AddResource(ResourceType.Coal, 1);
                        currentCoal.HarvestResource();
                    }
                    break;
            }

            if (GetTotalCarry() >= maxCarryCapacity)
                break;

            yield return new WaitForSeconds(0.5f);
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
        Debug.Log("transfering resources wood: " + woodCount + ", stone: " + stoneCount + ", coal: " + coalCount);
        woodCount = 0;
        stoneCount = 0;
        coalCount = 0;
    }
}
