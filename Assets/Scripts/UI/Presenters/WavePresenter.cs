using Bones.Gameplay.Startup;
using Bones.UI.Bindings.Base;
using UniRx;
using UnityEngine;
using Zenject;

namespace Bones.UI.Presenters
{
    public class WavePresenter : BasePresenter, IInitializable
    {
        [SerializeReference] private IBinding _completenessBinding;
        [SerializeReference] private IBinding _typeBinding;
        [SerializeReference] private IBinding _indexBinding;
        
        private IWaveProvider _waveProvider;

        public void Initialize()
        {
            Subscribe(_waveProvider.Completeness, _completenessBinding);
            Subscribe(_waveProvider.Wave.Select(x => (int)x.Type), _typeBinding);
            Subscribe(_waveProvider.Index, _indexBinding);
        }
        
        [Inject]
        private void Inject(IWaveProvider waveProvider)
        {
            _waveProvider = waveProvider;
        }
    }
}