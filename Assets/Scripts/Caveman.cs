using Navigation;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using UnityEngine;

public class Caveman : MonoBehaviour
{
    public enum CavemanState { Idle, Walking, PerformingTask }
    public enum CavemanTasks { Gathering, Hunting, Building, Hungry, Home, Breeding }
    public enum ResourceType { Wood, Stone, Coal }

    public CavemanState state;
    public CavemanTasks task;

    [Header("Movement")]
    public Transform homePosition;
    [SerializeField] private float arrivalDistance = 0.5f;

    [Header("Carrying")]
    [SerializeField] private int maxCarryCapacity = 3;
    public int woodCount = 0;
    public int stoneCount = 0;
    public int coalCount = 0;

    [Header("Hunger")]
    public float hungerMeter = 100f;
    public bool isHungry = false;

    [Header("Breeding")]
    public GameObject babyPrefab;
    [SerializeField] private float breedCooldown = 30f;
    private float birthTime;

    // Internal
    public CavemanNav nav;
    public bool isPerformingTask;
    private ResourceType currentHarvestResource;
    private Coroutine _activeCoroutine;

    public List<Tree> availableTrees = new List<Tree>();
    public List<Coal> availableCoals = new List<Coal>();
    public List<Stone> availableStones = new List<Stone>();
    private Tree currentTree;
    private Coal currentCoal;
    private Stone currentStone;

    void Start()
    {
        nav = GetComponent<CavemanNav>();
        state = CavemanState.Idle;
        task = CavemanTasks.Home;
        birthTime = Time.time;
        hungerMeter = 100f;

        if (homePosition == null) homePosition = homePosition = HomeManager.Instance.transform;;
    }

    void Update()
    {
        // Tick hunger
        hungerMeter -= Time.deltaTime;
        if (hungerMeter <= 0f)
        {
            hungerMeter = 0f;
            if (!isHungry)
            {
                isHungry = true;
                if (state != CavemanState.PerformingTask)
                    StartTaskCoroutine(HungryTask());
            }
        }
    }

    // ── Task entry points ──

    public void SetGatheringTask(ResourceType resource)
    {
        if (isPerformingTask) return;
        task = CavemanTasks.Gathering;
        currentHarvestResource = resource;
        StartTaskCoroutine(GatheringTask(resource));
    }

    public void SetBreedingTask()
    {
        if (Time.time - birthTime < breedCooldown)
        {
            Debug.Log("Too young to breed");
            return;
        }

        if (!BuildingManager.Instance.BuildingExists(BuildingType.BreedingTent))
        {
            Debug.Log("No breeding tent built yet");
            return;
        }

        task = CavemanTasks.Breeding;
        StartTaskCoroutine(BreedingTask());
    }

    private void StartTaskCoroutine(IEnumerator routine)
    {
        if (_activeCoroutine != null)
            StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(routine);
    }

    // ── Movement helpers ──

    private IEnumerator MoveTo(Vector3 destination)
    {
        nav.GoToPosition(destination);
        state = CavemanState.Walking;

        while (Vector3.Distance(transform.position, destination) > arrivalDistance)
            yield return null;

        state = CavemanState.Idle;
    }

    private IEnumerator MoveToTransform(Transform target)
    {
        // Keep updating destination in case target moves
        nav.GoToPosition(target.position);
        state = CavemanState.Walking;

        while (target != null && Vector3.Distance(transform.position, target.position) > arrivalDistance)
        {
            nav.GoToPosition(target.position);
            yield return null;
        }

        state = CavemanState.Idle;
    }

    // ── Task coroutines ──

    private IEnumerator GatheringTask(ResourceType resource)
    {
        isPerformingTask = true;
        state = CavemanState.PerformingTask;

        while (GetTotalCarry() < maxCarryCapacity)
        {
            // Find nearest resource
            bool found = FindNearestResource(resource);
            if (!found)
            {
                Debug.Log("No resources available for " + resource);
                break;
            }

            // Move to resource
            Vector3 resourcePos = GetCurrentResourcePosition(resource);
            yield return MoveTo(resourcePos);

            // Harvest
            yield return new WaitForSeconds(2f);
            HarvestCurrentResource(resource);

            if (GetTotalCarry() >= maxCarryCapacity) break;
            yield return new WaitForSeconds(0.3f);
        }

        // Return home and deposit
        yield return ReturnHome();
        DepositResources();
        ResetState();
    }

    private IEnumerator HuntingTask()
    {
        isPerformingTask = true;
        state = CavemanState.PerformingTask;
        yield return new WaitForSeconds(2f);
        yield return ReturnHome();
        ResetState();
    }

    private IEnumerator BuildingTask()
    {
        isPerformingTask = true;
        state = CavemanState.PerformingTask;
        yield return new WaitForSeconds(2f);
        yield return ReturnHome();
        ResetState();
    }

    private IEnumerator BreedingTask()
    {
        isPerformingTask = true;
        state = CavemanState.PerformingTask;

        // Find the breeding tent
        BreedingTent tent = FindObjectOfType<BreedingTent>();
        if (tent == null)
        {
            Debug.LogError("No breeding tent found");
            ResetState();
            yield break;
        }

        // Walk to tent
        yield return MoveTo(tent.transform.position);

        int breedTime = UnityEngine.Random.Range(3, 10);
        yield return new WaitForSeconds(breedTime);

        if (babyPrefab != null)
            Instantiate(babyPrefab,
                tent.transform.position + new Vector3(1f, 0f, 0f),
                Quaternion.identity);
        else
            Debug.LogError("babyPrefab not assigned on " + gameObject.name);

        birthTime = Time.time;

        // Return home after breeding
        yield return ReturnHome();
        ResetState();
    }

    private IEnumerator HungryTask()
    {
        isPerformingTask = true;
        state = CavemanState.PerformingTask;

        yield return ReturnHome();

        yield return new WaitForSeconds(2f);
        hungerMeter = 100f;
        isHungry = false;

        ResetState();
    }

    // ── Return home ──

    private IEnumerator ReturnHome()
    {
        if (homePosition == null)
        {
            Debug.LogError("homePosition is null on " + gameObject.name);
            yield break;
        }

        yield return MoveTo(homePosition.position);
    }

    // ── Resource helpers ──

    private bool FindNearestResource(ResourceType resource)
    {
        switch (resource)
        {
            case ResourceType.Wood:
                RefreshTreeList();
                currentTree = GetNearest(availableTrees, t => t.transform.position);
                return currentTree != null;
            case ResourceType.Stone:
                RefreshStoneList();
                currentStone = GetNearest(availableStones, s => s.transform.position);
                return currentStone != null;
            case ResourceType.Coal:
                RefreshCoalList();
                currentCoal = GetNearest(availableCoals, c => c.transform.position);
                return currentCoal != null;
        }
        return false;
    }

    private Vector3 GetCurrentResourcePosition(ResourceType resource)
    {
        switch (resource)
        {
            case ResourceType.Wood:  return currentTree  != null ? currentTree.transform.position  : transform.position;
            case ResourceType.Stone: return currentStone != null ? currentStone.transform.position : transform.position;
            case ResourceType.Coal:  return currentCoal  != null ? currentCoal.transform.position  : transform.position;
        }
        return transform.position;
    }

    private void HarvestCurrentResource(ResourceType resource)
    {
        switch (resource)
        {
            case ResourceType.Wood:
                if (currentTree != null)
                {
                    AddResource(ResourceType.Wood, 1);
                    currentTree.HarvestResource();
                    currentTree = null;
                }
                break;
            case ResourceType.Stone:
                if (currentStone != null)
                {
                    AddResource(ResourceType.Stone, 1);
                    currentStone.HarvestResource();
                    currentStone = null;
                }
                break;
            case ResourceType.Coal:
                if (currentCoal != null)
                {
                    AddResource(ResourceType.Coal, 1);
                    currentCoal.HarvestResource();
                    currentCoal = null;
                }
                break;
        }
    }

    private T GetNearest<T>(List<T> list, System.Func<T, Vector3> positionGetter) where T : UnityEngine.Object
    {
        T nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var item in list)
        {
            if (item == null) continue;
            float dist = Vector3.Distance(transform.position, positionGetter(item));
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = item;
            }
        }
        return nearest;
    }

    private void RefreshTreeList()
    {
        availableTrees.Clear();
        availableTrees.AddRange(FindObjectsOfType<Tree>());
    }

    private void RefreshStoneList()
    {
        availableStones.Clear();
        availableStones.AddRange(FindObjectsOfType<Stone>());
    }

    private void RefreshCoalList()
    {
        availableCoals.Clear();
        availableCoals.AddRange(FindObjectsOfType<Coal>());
    }

    private void AddResource(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.Wood:  woodCount  += amount; break;
            case ResourceType.Stone: stoneCount += amount; break;
            case ResourceType.Coal:  coalCount  += amount; break;
        }
    }

    private int GetTotalCarry() => woodCount + stoneCount + coalCount;

    public void DepositResources()
    {
        HomeManager.Instance.AddResources(woodCount, stoneCount, coalCount);
        Debug.Log($"Depositing — wood: {woodCount}, stone: {stoneCount}, coal: {coalCount}");
        woodCount = 0;
        stoneCount = 0;
        coalCount = 0;
    }

    private void ResetState()
    {
        task = CavemanTasks.Home;
        state = CavemanState.Idle;
        isPerformingTask = false;
        _activeCoroutine = null;
    }
}
