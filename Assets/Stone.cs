using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stone : MonoBehaviour
{
    [SerializeField] private Tilemap stoneTilemap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HarvestResource()
    {
        Debug.Log("Stone harvested!");
        HarvestStoneAtWorldPosition(this.transform.position);
    }

    public void HarvestStoneAtWorldPosition(Vector3 worldPos)
    {
        Vector3Int cell = stoneTilemap.WorldToCell(worldPos);
        stoneTilemap.SetTile(cell, null);
        Debug.Log("Stone tile destroyed at " + cell);
    }
}
