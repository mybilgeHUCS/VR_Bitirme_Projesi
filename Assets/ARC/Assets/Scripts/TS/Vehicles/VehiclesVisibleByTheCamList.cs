// Description: VehiclesVisibleByTheCamList: Allows any script to know of vehicles are visible by the camera P1 | P2
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TS.Generics;

namespace TS.Generics {
    public class VehiclesVisibleByTheCamList : MonoBehaviour
    {
        public static VehiclesVisibleByTheCamList instance = null;

        public List<VehicleInfo> listVehicles = new List<VehicleInfo>();

        public List<int> listLayersUsedByLayerMask01 = new List<int>();

        public bool b_InitDone;
        private bool b_InitInProgress;

        [System.Serializable]
        public class VehiclesVisibleByCamera
        {
            public int vehicleToIgnore = 0;
            public Camera cam;
            public List<bool> listVehiclesVisible = new List<bool>();
            public LayerMask layerMask01;

            public VehiclesVisibleByCamera(Camera newCam, List<bool> listBool)
            {
                cam = newCam;
                listVehiclesVisible = listBool;
            }
        }

        public List<VehiclesVisibleByCamera> listVehiclesVisibleByCamera = new List<VehiclesVisibleByCamera>();


        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        //-> Initialisation
        public bool bInitVehiclesVisibleByCamera()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(InitRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

      
        private IEnumerator InitRoutine()
        {
            //-> Init LayerMask
            string[] layerUsed = new string[listLayersUsedByLayerMask01.Count];
            for (var i = 0; i < listLayersUsedByLayerMask01.Count; i++)
                layerUsed[i] = LayerMask.LayerToName(LayersRef.instance.layersListData.listLayerInfo[listLayersUsedByLayerMask01[i]].layerID);

            for (var i = 0; i < listVehiclesVisibleByCamera.Count; i++)
                listVehiclesVisibleByCamera[i].layerMask01 = LayerMask.GetMask(layerUsed);


            yield return new WaitUntil(() => VehiclesRef.instance.b_InitDone == true);

            for (var i = 0;i< VehiclesRef.instance.listVehicles.Count; i++)
            {
                listVehicles.Add(VehiclesRef.instance.listVehicles[i]);
                
            }

            for (var j = 0; j < listVehiclesVisibleByCamera.Count; j++)
            {
                for (var i = 0; i < VehiclesRef.instance.listVehicles.Count; i++)
                {
                    listVehiclesVisibleByCamera[j].listVehiclesVisible.Add(false);
                }
            }

            b_InitDone = true;
            //Debug.Log("Time Trial Step 1:  -> Init Game Modules Done");
            yield return null;
        }

        void Update()
        {
            if (b_InitDone)
            {
                for (var i = 0; i < listVehiclesVisibleByCamera.Count; i++)
                {
                    for (var j = 0; j < listVehicles.Count; j++)
                    {
                        Camera tmpCam = listVehiclesVisibleByCamera[i].cam;
                        Transform target = listVehicles[j].transform;



                        RaycastHit hit;

                        bool ObstacleDetacted = true;
                        if (Physics.Linecast(tmpCam.transform.position, target.position, out hit, listVehiclesVisibleByCamera[i].layerMask01))
                        {
                            int vehicleLayer = LayersRef.instance.layersListData.listLayerInfo[9].layerID;
                            if (hit.transform.gameObject.layer == vehicleLayer)
                                ObstacleDetacted = false;
                        }

                        if (IsTargetVisible(tmpCam, target.gameObject) &&
                            !ObstacleDetacted &&
                            j != listVehiclesVisibleByCamera[i].vehicleToIgnore &&
                            !listVehicles[j].b_IsRespawn
                            )
                        {
                            if (!listVehiclesVisibleByCamera[i].listVehiclesVisible[j])
                            {
                                listVehiclesVisibleByCamera[i].listVehiclesVisible[j] = true;
                            }     
                        }
                        else
                        {
                            if (listVehiclesVisibleByCamera[i].listVehiclesVisible[j])
                            {
                                listVehiclesVisibleByCamera[i].listVehiclesVisible[j] = false;
                            }
                            
                        }
                    }
                }
                //Debug.Log("Bla");

            }

        }

        bool IsTargetVisible(Camera c, GameObject go)
        {
            #region
            var planes = GeometryUtility.CalculateFrustumPlanes(c);
            var point = go.transform.position;
            foreach (var plane in planes)
            {
                if (plane.GetDistanceToPoint(point) < 0)
                    return false;
            }
            return true;
            #endregion
        }
    }
}