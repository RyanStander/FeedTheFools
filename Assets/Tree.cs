using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tree : MonoBehaviour
{
    [SerializeField] private Tilemap treeTilemap;

    // Start is called before the first frame update
    void Start()
    {
        if (treeTilemap == null)
            treeTilemap = FindObjectOfType<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HarvestResource()
    {
        Debug.Log("Tree harvested!");
        HarvestTreeAtWorldPosition(this.transform.position);
    }

    public void HarvestTreeAtWorldPosition(Vector3 worldPos)
    {
        Vector3Int cell = treeTilemap.WorldToCell(worldPos);
        treeTilemap.SetTile(cell, null);
        Debug.Log("Tree tile destroyed at " + cell);
    }
}
