using UnityEngine;
using UnityEngine.UI;

namespace Buildings
{
    public class BuildingButton : MonoBehaviour
    {
        [SerializeField] private BuildingData _data;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
                BuildMode.Instance.SelectBuilding(_data));
        }
    }
}
