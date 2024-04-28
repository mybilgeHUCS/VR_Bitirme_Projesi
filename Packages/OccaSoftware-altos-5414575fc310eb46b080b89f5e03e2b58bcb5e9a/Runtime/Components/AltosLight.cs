using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace OccaSoftware.Altos.Runtime
{
    [ExecuteAlways]
    [RequireComponent(typeof(Light))]
    public class AltosLight : MonoBehaviour
    {
        public Light _light;

        public static HashSet<AltosLight> altosLightMap = new HashSet<AltosLight>();
        public const int MAX_ALTOS_LIGHT_COUNT = 8;
        public float Intensity
        {
            get => _light.intensity;
        }

        public Color Color => _light.color;

        public Vector3 Position
        {
            get => transform.position;
        }

        private void Reset()
        {
            _light = GetComponent<Light>();
        }

        private void OnEnable()
        {
            if (_light == null)
            {
                _light = GetComponent<Light>();
            }

            altosLightMap.Add(this);
        }

        private void OnDisable()
        {
            altosLightMap.Remove(this);
        }
    }
}
