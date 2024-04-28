using OccaSoftware.Altos.Runtime;

using UnityEngine;
using UnityEngine.Rendering;

namespace OccaSoftware.Altos.Demo
{
    // We use the DefaultExecutionOrder attribute to force this Component to run before the Sky Director.
    [DefaultExecutionOrder(-1)]
    public class FloatingPointOffsetDemo : MonoBehaviour
    {
        public float distance = 512;
        private Vector3 origin = Vector3.zero;

        private void OnEnable()
        {
            AltosSkyDirector.Instance.SetOrigin(origin);
        }

        private void OnDisable()
        {
            if (AltosSkyDirector.Instance)
            {
                AltosSkyDirector.Instance.SetOrigin(origin);
            }
        }

        private void Update()
        {
            if (transform.position.magnitude > distance)
            {
                origin -= transform.position;
                AltosSkyDirector.Instance.SetOrigin(origin);
                transform.position = Vector3.zero;
            }
        }
    }
}
