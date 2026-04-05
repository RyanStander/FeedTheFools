using System;
using UnityEngine;
using UnityEngine.AI;

namespace Navigation
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CavemanNav : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _navMeshAgent;

        private void OnValidate()
        {
            if (_navMeshAgent == null)
                _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            _navMeshAgent.updateRotation = false;
            _navMeshAgent.updateUpAxis = false;
        }

        public void GoToPosition(Vector2 target)
        {
            _navMeshAgent.SetDestination(target);
        }
    }
}
