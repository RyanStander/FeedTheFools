using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    [System.Serializable]
    public class ResourceCost
    {
        public Caveman.ResourceType Type;
        public int Amount;
    }

    public enum BuildingType
    {
        AxeHut,
        ChiselHut,
        PickaxeHut,
        Toolsmith,
        SleepingHut,
        BreedingTent,
        Farmland,
        Smelter
    }

    [CreateAssetMenu(fileName = "BuildingData", menuName = "FeedTheFools/BuildingData")]
    public class BuildingData : ScriptableObject
    {
        [Header("Info")]
        public BuildingType Type;
        public string BuildingName;
        public Sprite Icon;
        public Sprite BuildingSprite;

        [Header("Construction")]
        public List<ResourceCost> BuildCost;
        public float ConstructionTime = 10f;
        public int AssignSlots = 1;

        [Header("Production")]
        public Caveman.ResourceType InputResource;
        public int InputAmount;
        public Caveman.ResourceType OutputResource;
        public int OutputAmount;
        public float ProductionTime = 8f;
    }
}
