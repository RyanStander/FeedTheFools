using System;
using System.Collections;
using System.Collections.Generic;
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
}
