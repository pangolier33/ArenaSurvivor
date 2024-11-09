using UnityEngine;

namespace Plugins.mitaywalle.ACV.Runtime
{
	public class ACV_LineGameObject : ACV_Line<GameObject>
	{
		protected override GameObject GetTarget(Transform target) => target.gameObject;
		protected override bool GetState(GameObject target) => target.activeSelf;
		protected override void SetState(GameObject target, bool state) => target.SetActive(state);
	}
}