using System.Collections;
using System.Collections.Generic;
using Buildings;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;
    [SerializeField] private Grid _grid;

    [SerializeField] private List<BuildingData> _allBuildingTypes;
    [SerializeField] private GameObject _buildingPrefab;
    
    private List<Building> _placedBuildings = new();
    private HashSet<Vector3Int> _occupiedCells = new();
    
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    #region Placement

    public bool CanPlace(Vector3Int cell)
    {
        return !_occupiedCells.Contains(cell);
    }

    public bool TryPlace(BuildingData data, Vector3Int cell)
    {
        if (!CanPlace(cell)) return false;
        //TODO: Connect to resources
        if (!HomeManager.Instance.Has(data.BuildCost)) return false;

        HomeManager.Instance.Spend(data.BuildCost);

        Vector3 worldPos = _grid.CellToWorld(cell);
        worldPos.x +=0;
        worldPos.y += _grid.cellSize.y;
        worldPos.z = 0;
        
        var go = Instantiate(_buildingPrefab, worldPos, Quaternion.identity);
        var building = go.GetComponent<Building>();
        building.Init(data);

        _placedBuildings.Add(building);
        _occupiedCells.Add(cell);

        return true;
    }
    
    #endregion
    
    public void RegisterBuilding(Building building)
    {
        if (!_placedBuildings.Contains(building))
            _placedBuildings.Add(building);
    }
    
    public Vector3Int WorldToCell(Vector3 worldPos) => _grid.WorldToCell(worldPos);
    public Vector3 CellToWorld(Vector3Int cell) => _grid.CellToWorld(cell) + _grid.cellSize / 2f;
}
