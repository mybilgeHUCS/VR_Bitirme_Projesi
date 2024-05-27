// Description: Attached to the player to init the camera when the scene starts.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TS.Generics
{
    public class CamInitParams : MonoBehaviour
    {
        public List<int> cullingMaskList = new List<int>();

        public List<int> volumeMaskList = new List<int>();

        private UniversalAdditionalCameraData cam;
        private Camera cam02;

        // Start is called before the first frame update
        void Start()
        {
            //-> Init Camera Culling Mask
            if (cullingMaskList.Count > 0)
            {
                string[] layerUsed = new string[cullingMaskList.Count];
                for (var i = 0; i < cullingMaskList.Count; i++)
                    layerUsed[i] = LayerMask.LayerToName(LayersRef.instance.layersListData.listLayerInfo[cullingMaskList[i]].layerID);

                cam02 = GetComponent<Camera>();
                cam02.cullingMask = LayerMask.GetMask(layerUsed);
            }


            //-> Init Camera Volume Mask
            if (volumeMaskList.Count > 0)
            {
                string[] layerUsed = new string[volumeMaskList.Count];
                for (var i = 0; i < volumeMaskList.Count; i++)
                    layerUsed[i] = LayerMask.LayerToName(LayersRef.instance.layersListData.listLayerInfo[volumeMaskList[i]].layerID);

                cam = GetComponent<UniversalAdditionalCameraData>();
                cam.volumeLayerMask = LayerMask.GetMask(layerUsed);
            }
            
        }

    }

}
