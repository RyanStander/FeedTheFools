using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GodManager : MonoBehaviour
{
    public static GodManager Instance;
    public List<Caveman> Cavemen;
    [SerializeField] private GameObject _winScreenPopUp;
    [SerializeField] private GameObject _babyPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    private void Start()
    {
        Cavemen = FindObjectsByType<Caveman>(FindObjectsSortMode.None).ToList();
    }

    public void AddCaveman(Caveman caveman)
    {
        Cavemen.Add(caveman);
    }

    public List<Caveman.CavemanState> GetByState(Caveman.CavemanState state)
    {
        return Cavemen.Where(c => c.state == state).Select(c => c.state).ToList();
    }
    
    public void SpawnBaby(Vector3 spawnPosition)
    {
        Instantiate(_babyPrefab, spawnPosition, Quaternion.identity);
    }
}
