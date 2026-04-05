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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddResources(int woodCount, int stoneCount, int coalCount)
    {
        homeWoodCount += woodCount;
        homeStoneCount += stoneCount;
        homeCoalCount += coalCount;

        Debug.Log("depo'd: wood: " + this.homeWoodCount + ", Total stone: " + this.homeStoneCount + ", Total coal: " + this.homeCoalCount);
    }
}
