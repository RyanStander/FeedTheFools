using System;
using System.Collections;
using UnityEngine;

namespace Buildings
{
    public class Building : MonoBehaviour
    {
        public BuildingData Data { get; private set; }
        public Caveman AssignedCaveman { get; private set; }
        public bool IsUnderConstruction { get; private set; } = true;
        public float ConstructionProgress { get; private set; }
        public Vector3Int Cell { get; private set; }

        private Coroutine _activeCoroutine;
        [SerializeField] private SpriteRenderer _renderer;

        private void OnValidate()
        {
            if (_renderer == null)
                _renderer = GetComponent<SpriteRenderer>();
        }

        public void Init(BuildingData data)
        {
            Data = data;
            Cell = BuildingManager.Instance.WorldToCell(transform.position);
            IsUnderConstruction = true;
            ConstructionProgress = 0f;

            if (data.BuildingSprite != null)
                _renderer.sprite = data.BuildingSprite;

            StartCoroutine(AutoConstructRoutine());
        }

        private IEnumerator AutoConstructRoutine()
        {
            float elapsed = 0f;
            while (elapsed < Data.ConstructionTime)
            {
                elapsed += Time.deltaTime;
                ConstructionProgress = elapsed / Data.ConstructionTime;
                yield return null;
            }

            ConstructionProgress = 1f;
            IsUnderConstruction = false;
            BuildingManager.Instance.RegisterBuilding(this);
            Debug.Log($"{Data.BuildingName} construction complete");
        }

        #region Assignment

        public bool IsAvailable() => AssignedCaveman == null;

        public void AssignCaveman(Caveman caveman)
        {
            AssignedCaveman = caveman;

            if (IsUnderConstruction)
                _activeCoroutine = StartCoroutine(ConstructionRoutine());
            else
                _activeCoroutine = StartCoroutine(ProductionRoutine());
        }

        public void UnassignCaveman()
        {
            if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
            AssignedCaveman = null;
        }

        #endregion

        #region Construction

        private IEnumerator ConstructionRoutine()
        {
            ConstructionProgress = 0f;
            float elapsed = 0f;

            while (elapsed < Data.ConstructionTime)
            {
                elapsed += Time.deltaTime;
                ConstructionProgress = elapsed / Data.ConstructionTime;
                yield return null;
            }

            ConstructionProgress = 1f;
            IsUnderConstruction = false;
            BuildingManager.Instance.RegisterBuilding(this);
            _activeCoroutine = StartCoroutine(ProductionRoutine());
        }

        #endregion

        #region Production

        private IEnumerator ProductionRoutine()
        {
            while (AssignedCaveman != null)
            {
                if (Data.InputAmount > 0)
                {
                    // Wait until inputs available
                    yield return new WaitUntil(() =>
                        HomeManager.Instance.Has(Data.InputResource, Data.InputAmount));

                    HomeManager.Instance.Spend(Data.InputResource, Data.InputAmount);
                }

                yield return new WaitForSeconds(Data.ProductionTime);

                /*if (Data.OutputAmount > 0)
                    ResourceManager.Instance.Add(Data.OutputResource, Data.OutputAmount);*/

                OnProductionComplete();
            }
        }
        
        protected virtual void OnProductionComplete()
        {
        }

        #endregion
    }
}
