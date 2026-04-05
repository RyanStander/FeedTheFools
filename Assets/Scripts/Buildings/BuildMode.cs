using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings
{
    public class BuildMode : MonoBehaviour
    {
        public static BuildMode Instance { get; private set; }

        [SerializeField] private GameObject _pickerPanel;
        [SerializeField] private GameObject _ghost;
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _groundTilemap;

        private SpriteRenderer _ghostRenderer;
        private BuildingData _selected;
        private bool _active;

        private void Start()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            _ghostRenderer = _ghost.GetComponent<SpriteRenderer>();
        }

        private Vector3 _snappedPos;

        void Update()
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
    
            Vector3Int cell = _grid.WorldToCell(mouseWorld);
            _snappedPos = _grid.CellToWorld(cell);
            _snappedPos.y += 0.5f; // tweak this value
            _snappedPos.z = 0;
    
            _ghost.transform.position = _snappedPos;
    
            if (Input.GetMouseButtonDown(0) && !_pickerPanel.activeSelf && _selected != null)
                BuildingManager.Instance.TryPlace(_selected, cell);

            if (Input.GetMouseButtonDown(1))
            {
                Deactivate();
            }
        }

        public void Activate()
        {
            _active = true;
            _pickerPanel.SetActive(true);
            _ghost.SetActive(false);
        }

        public void Deactivate()
        {
            _active = false;
            _pickerPanel.SetActive(false);
            _ghost.SetActive(false);
            _selected = null;
        }

        public void SelectBuilding(BuildingData data)
        {
            _selected = data;
            _ghostRenderer.sprite = data.BuildingSprite;
            _pickerPanel.SetActive(false);
            _ghost.SetActive(true);
        }

        
    }
}
