using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace Bones.Gameplay.Experience
{
    public class ExperienceBank : IExperienceBank
    {
        private readonly Subject<IStarModel> _requested = new();
        private readonly Subject<IStarModel> _released = new();

        private readonly int[] _levelsMap;
        private readonly List<Request> _pendingRequests = new();
        private readonly ReactiveProperty<int> _level = new();

        public IObservable<IStarModel> Requested => _requested;
        public IObservable<IStarModel> Released => _released;
        public IReadOnlyReactiveProperty<int> Level => _level;

        public int StarsCount => _pendingRequests.Count;
        public bool IsAllFull => _pendingRequests.All(x => x.IsFull);
        private Request AvailableRequest => _pendingRequests.FirstOrDefault(x => !x.IsFull);
        
        public ExperienceBank(int[] levelsMap)
        {
            _levelsMap = levelsMap;
            _level.Value = 0;
        }

        public void Obtain(float amount)
        {
            CreateNewRequestIfNeeded();
            TryObtain(amount);
        }

        public void ObtainPercent(float percent)
        {
            CreateNewRequestIfNeeded();
            var lastRequiredAmount = AvailableRequest.RequiredAmount;
            var obtainAmount = lastRequiredAmount * percent;
            TryObtain(obtainAmount);
        }

        public void Release()
        {
            var completedCount = 0;
            for (var i = 0; i < _pendingRequests.Count; i++)
            {
                var request = _pendingRequests[i];
                if (!request.IsFull)
                    continue;
                
                _level.Value++;
                _released.OnNext(request);
                completedCount++;
                
                _pendingRequests.RemoveAt(i);
                i--;
            }
        }
        
        private void CreateNewRequestIfNeeded()
        {
            if (_level.Value >= _levelsMap.Length)
                return;
            
            if (AvailableRequest != null)
                return;
            
            var index = _pendingRequests.FirstOrDefault()?.Index + 1 ?? _level.Value;
            var request = new Request(_levelsMap[index], index);
            _pendingRequests.Add(request);
            _requested.OnNext(request);
        }

        private void TryObtain(float amount)
        {
            AvailableRequest.TryObtain(amount, out amount);
        }

        private class Request : IStarModel
        {
            private readonly ReactiveProperty<float> _amount = new();

            public Request(float requiredAmount, int index)
            {
                RequiredAmount = requiredAmount;
                Index = index;
            }

            public int Index { get; }
            public bool IsFull => _amount.Value >= RequiredAmount;
            public IObservable<float> Completeness => _amount.Select(x => x / RequiredAmount);
            public float RequiredAmount { get; }

            public bool TryObtain(float addition, out float remainder)
            {
                remainder = _amount.Value - RequiredAmount + addition;
                if (remainder < 0)
                {
                    _amount.Value += addition;
                    return true;
                }
                else
                {
                    _amount.Value = RequiredAmount;
                    return false;
                }
            }
        }
    }
}