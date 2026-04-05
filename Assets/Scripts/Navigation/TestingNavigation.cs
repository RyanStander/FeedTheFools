using System;
using System.Collections.Generic;
using UnityEngine;

namespace Navigation
{
    public class TestingNavigation : MonoBehaviour
    {
        [SerializeField] private CavemanNav _cavemanNav;
        [SerializeField] private List<Transform> _targetsToLoopThrough;
        [SerializeField] private float _distanceFromTarget = 0.5f;
        private int _index;

        private void Start()
        {
            _cavemanNav.GoToPosition(_targetsToLoopThrough[_index].position);
        }

        private void Update()
        {
            if(Vector2.Distance(_targetsToLoopThrough[_index].position,transform.position)>_distanceFromTarget)
                return;

            _index++;
            if (_index >= _targetsToLoopThrough.Count)
                _index = 0;
            _cavemanNav.GoToPosition(_targetsToLoopThrough[_index].position);
        }
    }
}
