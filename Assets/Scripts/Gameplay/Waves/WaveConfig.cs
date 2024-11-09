using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bones.Gameplay.Waves
{
    [CreateAssetMenu(menuName = "Waves/Config", fileName = "New Wave")]
    public class WaveConfig : ScriptableObject
    {
        [SerializeField] private WaveType _type;
        
        [SerializeField]
        private float _durationInMinutes;
            
        [SerializeField]
        [ListDrawerSettings(ListElementLabelName = "Name", ShowPaging = true)]
        private SpawnerConfig[] _spawners;

        public WaveType Type => _type;
        public float Duration => _durationInMinutes * 60;
        public IEnumerable<SpawnerConfig> Spawners => _spawners.Where(x => x.Enabled);
        
        #if UNITY_EDITOR
	    //[Button]
	    private void MigrateValues()
	    {
		    foreach (SpawnerConfig spawner in _spawners)
		    {
			    spawner.MigrateSharedValues();
		    }
	    }
        #endif
    }
}