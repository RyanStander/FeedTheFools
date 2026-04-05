using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Coal : MonoBehaviour
{
    [SerializeField] private Tilemap coalTilemap;

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
        Debug.Log("Coal harvested!");
        HarvestCoalAtWorldPosition(this.transform.position);
    }

    public void HarvestCoalAtWorldPosition(Vector3 worldPos)
    {
        Vector3Int cell = coalTilemap.WorldToCell(worldPos);
        coalTilemap.SetTile(cell, null);
        Debug.Log("Coal tile destroyed at " + cell);
    }
}
