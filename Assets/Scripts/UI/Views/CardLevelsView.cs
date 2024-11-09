using System;
using System.Linq;
using Bones.UI.Bindings.Base;
using UnityEngine;

namespace Bones.UI.Views
{
    public class CardLevelsView : MonoBehaviour
    {
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private GameObject _lastCellPrefab;
        [SerializeField] private GameObject _spherePrefab;
        [SerializeField] private Transform _parent;
        
        private int _level;
        private int _maxLevel;

        private GameObject[] _cells = Array.Empty<GameObject>();
        
        public void SetLevel(int value)
        {
            _level = value;
            Rewind();
        }

        public void SetMaxLevel(int value)
        {
            _maxLevel = value;
            Rewind();
        }

        private void Rewind()
        {
            foreach (var cell in _cells)
                Destroy(cell);
            
            _cells = new GameObject[_maxLevel];
            for (var i = 0; i < _maxLevel; i++)
            {
                if (i < _level)
                    _cells[i] = Instantiate(_spherePrefab, _parent);
                else if (i == _maxLevel - 1)
                    _cells[i] = Instantiate(_lastCellPrefab, _parent);
                else
                    _cells[i] = Instantiate(_cellPrefab, _parent);
            }
        }

        [Serializable]
        public class LevelBinding : BaseBinding<int>
        {
            [SerializeField] private CardLevelsView _view;
            
            public override void OnNext(int value)
            {
                _view.SetLevel(value);    
            }
        }
        [Serializable]
        public class MaxLevelBinding : BaseBinding<int>
        {
            [SerializeField] private CardLevelsView _view;
            
            public override void OnNext(int value)
            {
                _view.SetMaxLevel(value);    
            }
        }
    }
}