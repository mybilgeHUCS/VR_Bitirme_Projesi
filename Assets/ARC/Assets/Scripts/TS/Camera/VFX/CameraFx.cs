// Description: CameraFx.cs: Create a sequence of camera Fx.
// Used when the vehicle is destroyed by cameraSystem.cs.
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class CameraFx : MonoBehaviour
    {
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        [System.Serializable]
        public class VFXSequence
        {
            public CameraShake cameraShake;
            public CameraEnableObj cameraEnableObj;
        }


        public List<VFXSequence> listVFXSequence = new List<VFXSequence>();

        public bool b_CameraShakeAvailable = true;
        public bool b_CameraEnableObjAvailable = true;
    }
}
