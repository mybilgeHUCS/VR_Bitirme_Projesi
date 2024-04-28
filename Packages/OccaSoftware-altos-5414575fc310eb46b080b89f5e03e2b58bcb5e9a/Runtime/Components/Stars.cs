using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OccaSoftware.Altos.Runtime
{
    [ExecuteAlways]
    [AddComponentMenu("OccaSoftware/Altos/Stars")]
    public class Stars : MonoBehaviour
    {
        public StarDefinition starDefinition;
        internal StarRenderPass renderPass;

        private void OnEnable()
        {
            renderPass = new StarRenderPass();
        }

        private void OnDisable()
        {
            renderPass?.Dispose();
            renderPass = null;
        }
    }
}
