//Description : gizmosCube.cs : Used to display a Cube on gameObject 
using UnityEngine;

namespace TS.generics
{
	public class gizmosCube : MonoBehaviour
	{

		public Color GizmoColor = new Color(0, .9f, 1f, .5f);

		public Transform overrideTransform;

		void OnDrawGizmos()
		{
			Gizmos.color = GizmoColor;

			Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);      // Allow the gizmo to fit the position, rotation and scale of the gameObject

            if(overrideTransform) cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, overrideTransform.localScale);

			Gizmos.matrix = cubeTransform;

			Gizmos.DrawCube(Vector3.zero, Vector3.one);
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}
	}
}
