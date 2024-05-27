using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class TVSystem : MonoBehaviour
    {
        public static TVSystem instance = null;
        public List<Camera> camList = new List<Camera>();
        public List<GameObject> objsToDiabledDuringInit = new List<GameObject>();

        public bool bInit;

        public Transform target;
        public Transform objPivot;

        public int currentTargetID;

        int currentActivatedCam;

        void Awake()
        {
            if (instance == null)
                instance = this;
        }

        // Start is called before the first frame update
        public void bInitTVSystem(int targetID)
        {
            bInit = false;
            for (var i = 0; i < camList.Count; i++)
            {
                camList[i].transform.parent.gameObject.SetActive(false);
            }

            camList[0].transform.parent.gameObject.SetActive(true);
            target = VehiclesRef.instance.listVehicles[targetID].transform;
            objPivot = camList[0].transform.parent;
            currentActivatedCam = 0;

            for (var i = 0; i < objsToDiabledDuringInit.Count; i++)
                objsToDiabledDuringInit[i].SetActive(false);

            Cam_Follow[] cam_Follows = FindObjectsOfType<Cam_Follow>();
            foreach(Cam_Follow obj in cam_Follows)
                obj.transform.gameObject.SetActive(false);

            bInit = true;
        }

        private void LateUpdate()
        {
            if(target && objPivot)
                objPivot.LookAt(target, Vector3.up);
        }


        public void InitNewCam(int newCamID)
        {
            camList[newCamID].transform.parent.gameObject.SetActive(true);
            target = VehiclesRef.instance.listVehicles[currentTargetID].transform;
            objPivot = camList[newCamID].transform.parent;

            camList[currentActivatedCam].transform.parent.gameObject.SetActive(false);
            currentActivatedCam = newCamID;
        }


        public void NewZoom(int newCamID, float newZoom, float zoomDuration, AnimationCurve zoomCurve)
        {
            StopCoroutine("ZoomRoutine");
            StartCoroutine(ZoomRoutine(newCamID,newZoom, zoomDuration, zoomCurve));
        }

        public IEnumerator ZoomRoutine(int newCamID, float newZoom, float zoomDuration, AnimationCurve zoomCurve)
        {
            if (!bInit)
                bInitTVSystem(newCamID);

            yield return new WaitUntil(() => bInit);



            float t = 0;
            float duration = zoomDuration;

            float currentZoom = camList[currentActivatedCam].fieldOfView;

            while (t < duration)
            {
                t += Time.deltaTime / duration;
                camList[currentActivatedCam].fieldOfView = Mathf.Lerp(currentZoom, newZoom, zoomCurve.Evaluate(t));
                yield return null;
            }

            yield return null;
        }

        public void NewShake(int newCamID, float shakeForceX,float shakeForceY, float shakeDuration, AnimationCurve shakeCurveX, AnimationCurve shakeCurveY)
        {
            StopCoroutine("ShakeRoutine");
            StartCoroutine(ShakeRoutine(newCamID, shakeForceX, shakeForceY, shakeDuration, shakeCurveX, shakeCurveY));
        }

        public IEnumerator ShakeRoutine(int newCamID, float shakeForceX, float shakeForceY, float shakeDuration, AnimationCurve shakeCurveX, AnimationCurve shakeCurveY)
        {
            float t = 0;

            while(t < shakeDuration)
            {
                float x = shakeCurveX.Evaluate(t) * shakeForceX;
                float y = shakeCurveY.Evaluate(t) * shakeForceY;

                camList[currentActivatedCam].transform.localPosition = new Vector3(x, y, 0);

                t += Time.deltaTime;

                yield return null;
            }

            camList[currentActivatedCam].transform.localPosition = Vector3.zero;

            yield return null;
        }
       


    }

}
