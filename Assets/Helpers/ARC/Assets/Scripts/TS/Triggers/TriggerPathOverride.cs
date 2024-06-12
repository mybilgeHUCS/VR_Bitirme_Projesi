// Description: TriggerPathOverride: Override the vehicle position on the path
using UnityEngine;

namespace TS.Generics
{
    public class TriggerPathOverride : MonoBehaviour
    {
        public bool refPosition;
        public Vector3 OverrideTargetPosition = Vector3.zero;
    }
}
