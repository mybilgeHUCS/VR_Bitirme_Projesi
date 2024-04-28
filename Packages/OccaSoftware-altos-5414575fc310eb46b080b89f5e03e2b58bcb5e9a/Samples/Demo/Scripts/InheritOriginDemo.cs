using OccaSoftware.Altos.Runtime;

using UnityEngine;

namespace OccaSoftware.Altos.Demo
{
    public class InheritOriginDemo : MonoBehaviour
    {
        private void OnEnable()
        {
            AltosSkyDirector.Instance.onOriginShift += Shift;
        }

        private void OnDisable()
        {
            if (AltosSkyDirector.Instance)
            {
                AltosSkyDirector.Instance.onOriginShift -= Shift;
            }
        }

        void Shift(Vector3 origin)
        {
            transform.position = origin;
        }
    }
}
