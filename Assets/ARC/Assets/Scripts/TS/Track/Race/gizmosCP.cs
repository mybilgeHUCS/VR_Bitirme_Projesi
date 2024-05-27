//Description : gizmosCP.cs : Used to display track Checkpoints
using UnityEngine;

namespace TS.Generics {
	public class gizmosCP : MonoBehaviour
	{

		public Path path;
        public Color gizmoColor = new Color(1, .092F, .016F, .5F);

		void OnDrawGizmos()
		{
			if (!path)
			{
				path = FindObjectOfType<Path>();
			}

            if (path)
            {
				Gizmos.color = gizmoColor;
				Gizmos.DrawSphere(transform.position, path.gizmoCheckpointSize);
			}
			
		}
	}
}
