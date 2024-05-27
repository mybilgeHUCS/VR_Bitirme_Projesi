using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class TVSystemTrigger : MonoBehaviour
    {
        public bool bNewCam;
        public int newCamID;

        public bool bNewZoom;
        public float zoom;
        public float zoomDuration;
        public AnimationCurve zoomCurve;


        public bool bNewShake;
        public float shakeForceX;
        public float shakeForceY;
        public float shakeDuration;
        public AnimationCurve shakeCurveX;
        public AnimationCurve shakeCurveY;


        private void OnTriggerEnter(Collider other)
        {
            if (TVSystem.instance.bInit)
            {
                int currentTargetID = TVSystem.instance.currentTargetID;
                if (other.transform.parent.GetComponent<VehicleInfo>() && other.transform.parent.GetComponent<VehicleInfo>().playerNumber == currentTargetID)
                {
                    if(bNewCam)
                        TVSystem.instance.InitNewCam(newCamID);

                    if (bNewZoom)
                        TVSystem.instance.NewZoom(newCamID,zoom, zoomDuration, zoomCurve);

                    if (bNewShake)
                        TVSystem.instance.NewShake(newCamID, shakeForceX, shakeForceY, shakeDuration, shakeCurveX, shakeCurveY);
                }
            } 
        }
    }

}
