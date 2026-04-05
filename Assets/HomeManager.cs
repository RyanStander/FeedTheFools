using System;
using System.Collections;
using System.Collections.Generic;
using Buildings;
using UnityEngine;

public class HomeManager : MonoBehaviour
{
    public static HomeManager Instance { get; private set; }

    public int homeWoodCount = 0;
    public int homeStoneCount = 0;
    public int homeCoalCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void AddResources(int woodCount, int stoneCount, int coalCount)
    {
        homeWoodCount += woodCount;
        homeStoneCount += stoneCount;
        homeCoalCount += coalCount;

        Debug.Log("depo'd: wood: " + this.homeWoodCount + ", Total stone: " + this.homeStoneCount + ", Total coal: " + this.homeCoalCount);
    }
    
    public bool Has(Caveman.ResourceType type, int amount)
    {
        if (type == Caveman.ResourceType.Wood)
            return homeWoodCount >= amount;
        if (type == Caveman.ResourceType.Stone)
            return homeStoneCount >= amount;
        if (type == Caveman.ResourceType.Coal)
            return homeCoalCount >= amount;
        return false;
    }
    
    public bool Has(List<ResourceCost> costs)
    {
        foreach (var cost in costs)
        {
            if (cost.Type == Caveman.ResourceType.Wood && homeWoodCount < cost.Amount)
                return false;
            if (cost.Type == Caveman.ResourceType.Stone && homeStoneCount < cost.Amount)
                return false;
            if (cost.Type == Caveman.ResourceType.Coal && homeCoalCount < cost.Amount)
                return false;
        }
        return true;
    }

    public void Spend(List<ResourceCost> costs)
    {
        foreach (var cost in costs)
        {
            if (cost.Type == Caveman.ResourceType.Wood)
                homeWoodCount -= cost.Amount;
            if (cost.Type == Caveman.ResourceType.Stone)
                homeStoneCount -= cost.Amount;
            if (cost.Type == Caveman.ResourceType.Coal)
                homeCoalCount -= cost.Amount;
        }
    }
    
    public void Spend(Caveman.ResourceType type, int amount)
    {
        if (type == Caveman.ResourceType.Wood)
            homeWoodCount -= amount;
        if (type == Caveman.ResourceType.Stone)
            homeStoneCount -= amount;
        if (type == Caveman.ResourceType.Coal)
            homeCoalCount -= amount;
    }
}
