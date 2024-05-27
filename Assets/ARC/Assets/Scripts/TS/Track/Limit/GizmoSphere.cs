//Description : gizmosCP.cs : Use to display track Checkpoints
using UnityEngine;

namespace TS.Generics
{
	public class GizmoSphere : MonoBehaviour
	{
		public Color gizmoColor = new Color(1, .092F, .016F, .5F);
		public float radius = 10;

		void OnDrawGizmos()
		{
			Gizmos.color = gizmoColor;
			Gizmos.DrawSphere(transform.position, radius);
		}
	}
}
