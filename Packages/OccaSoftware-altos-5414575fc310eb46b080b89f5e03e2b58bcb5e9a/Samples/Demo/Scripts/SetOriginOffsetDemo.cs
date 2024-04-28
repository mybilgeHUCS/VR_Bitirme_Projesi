using OccaSoftware.Altos.Runtime;

using UnityEngine;

namespace OccaSoftware.Altos.Demo
{
    [AddComponentMenu("OccaSoftware/Altos/Demo/Set Origin Offset")]
    [ExecuteAlways]
    public class SetOriginOffsetDemo : MonoBehaviour
    {
        public Vector3 origin;

        private void OnValidate()
        {
            SetOrigin();
        }

        private void SetOrigin()
        {
            AltosSkyDirector.Instance.SetOrigin(origin);
        }
    }
}
