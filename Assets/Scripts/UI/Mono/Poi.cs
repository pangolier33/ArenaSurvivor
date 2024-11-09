using UnityEngine;

namespace Bones.UI.Presenters
{
    public class Poi : MonoBehaviour
    {
        [SerializeField] private PrefabPresenter _markerPrefab;

        public PrefabPresenter MarkerPrefab => _markerPrefab;
    }
}