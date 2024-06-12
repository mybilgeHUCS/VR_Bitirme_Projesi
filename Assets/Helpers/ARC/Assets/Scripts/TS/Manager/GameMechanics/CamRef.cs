// Description: CamRef: Access form anywhere to P1 | P2 cam and Post process profile
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class CamRef : MonoBehaviour
    {
        public static CamRef instance = null;

        public List<Camera>             listCameras = new List<Camera>();
        public List<TS_PostProcess>     listPostFxVolumeProfile = new List<TS_PostProcess>();

        public bool                     b_InitDone;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }


        public bool initCamerasList()
        {
            b_InitDone = true;
            return b_InitDone;
        }
    }

}
