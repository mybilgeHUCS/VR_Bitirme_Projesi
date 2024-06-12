// Description: MinimapManager: Allows to display vehicles position on map.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class MinimapManager : MonoBehaviour
    {
        public static MinimapManager instance = null;
        public Transform objLineRenderer;
        public List<Transform> vehicleList    = new List<Transform>();
        public List<Transform> spotList     = new List<Transform>();

        public GameObject refSpot;
        public bool b_InitDone;
        private bool b_InitInProgress;
        public VehicleUIColorsDatas vehicleUIColorsData;

        public int spotSize = 124;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        //-> Init Lap counter
        public bool bInitMiniMap()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(bInitRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;


            return b_InitDone;
            #endregion
        }

        IEnumerator bInitRoutine()
        {
            #region
            b_InitDone = false;

            //-> First Init the vehicle List
            vehicleList.Clear();
            spotList.Clear();
            for (var i = 0; i < VehiclesRef.instance.listVehicles.Count; i++)
            {
                vehicleList.Add(VehiclesRef.instance.listVehicles[i].transform);

                GameObject newSpot = Instantiate(refSpot, objLineRenderer);
                if (vehicleUIColorsData)
                {
                    newSpot.transform.GetChild(0).transform.GetChild(1).GetComponent<SpriteRenderer>().color
                        = vehicleUIColorsData.listVehicleUIColorsDatas[i % vehicleUIColorsData.listVehicleUIColorsDatas.Count];
                }

                newSpot.name = i.ToString();


                float ratio = GetComponent<CreateMinimap>().cam.orthographicSize * 100 / 700 * .01f ;

                newSpot.transform.GetChild(0).localScale = new Vector3(spotSize * ratio, spotSize * ratio, 1);

                spotList.Add(newSpot.transform);

                newSpot.transform.GetChild(0).transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = -i;
                newSpot.transform.GetChild(0).transform.GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = -i;
            }

            int counter = 0;
            for (var i = spotList.Count - 1; i >= 0; i--)
            {
                if (spotList[i])
                {
                    spotList[i].transform.GetChild(0).localPosition = new Vector3(0, counter, 0);
                }
                counter++;
            }

            b_InitDone = true;
            yield return null;
            #endregion
        }

        // Update is called once per frame
        void Update()
        {
            if(vehicleList.Count > 0 && b_InitDone)
            {
                for(var i = 0;i< vehicleList.Count; i++)
                {
                    if (vehicleList[i])
                    {
                        if (spotList[i])
                        {
                            if (!spotList[i].gameObject.activeSelf)
                                spotList[i].gameObject.SetActive(true);

                            spotList[i].position = new Vector3(vehicleList[i].position.x, objLineRenderer.position.y + 50, vehicleList[i].position.z);
                        }
                    }
                    else
                    {
                        spotList[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
