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
            
            #region Colouring

            bool cellFree = BuildingManager.Instance.CanPlace(cell);
            bool hasResources = _selected != null && HomeManager.Instance.Has(_selected.BuildCost);
            bool valid = cellFree && hasResources;

            if (!_active || _selected == null)
            {
                // no tint
            }
            else if (!hasResources)
            {
                _ghostRenderer.color = new Color(1f, 0f, 0f, 0.5f);
            }
            else if (!cellFree)
            {
                _ghostRenderer.color = new Color(1f, 0.5f, 0f, 0.5f);
            }
            else
            {
                _ghostRenderer.color = new Color(0f, 1f, 0f, 0.5f);
            }

            #endregion
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
