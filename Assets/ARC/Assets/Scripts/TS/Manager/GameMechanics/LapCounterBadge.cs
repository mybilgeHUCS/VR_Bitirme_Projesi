// Description: LapCounterBadge. Find on vehicle.
// Check the start line
// Detect wrong way

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TS.Generics
{
    public class LapCounterBadge : MonoBehaviour
    {
     
        public bool         b_InitDone;
        private bool        b_InitInProgress;
        public int          vehicleID;
        public List<int>    listLayersUsedBylayerMask = new List<int>();
        public LayerMask    layerMaskLapCounter;


        public bool        bDetected;

        public float       angle;

        public bool         bIsPlayerIntoBufferZone;

        public Transform objStartLine;
        public Transform objBufferOutOut;

        public bool enterBuffersStart;

        private VehicleDamage vehicleDamage;
        private VehicleInfo vehicleInfo;

        public int lastLap = 0;

        public float value;
        public bool lastVal;
        public bool Lock;

        public UnityEvent DoSomethingAfterALap;

        public float distanceDetection = 250;

       
        //-> Initialisation
        public bool bInitLapCounterBadge()
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

        IEnumerator InitRoutine()
        {
            //-> Init LayerMask
            string[] layerUsed = new string[listLayersUsedBylayerMask.Count];
            for (var i = 0; i < listLayersUsedBylayerMask.Count; i++)
                layerUsed[i] = LayerMask.LayerToName(LayersRef.instance.layersListData.listLayerInfo[listLayersUsedBylayerMask[i]].layerID);
            layerMaskLapCounter = LayerMask.GetMask(layerUsed);

            Transform grpStartLine = FindObjectOfType<StartLine>().transform;
            objStartLine = grpStartLine.GetChild(0);//.GetChild(0);
            objBufferOutOut = grpStartLine.GetChild(1);//.GetChild(1);

            for (var i = 0; i < impactPosList.Count; i++)
                lastPosList[i] = impactPosList[i].position;


            vehicleDamage = transform.parent.GetComponent<VehicleDamage>();
            vehicleInfo = transform.parent.GetComponent<VehicleInfo>();

            vehicleDamage.VehicleRespawnPart1 += VehicleRespawnPart1;
            vehicleDamage.VehicleRespawnPart2 += VehicleRespawnPart2;

            // The player starts befor the start line
            bIsPlayerIntoBufferZone = true;


            b_InitDone = true;
            //Debug.Log("Init: VehicleDamage -> Done");
            yield return null;
        }

        public void OnDestroy()
        {
            transform.parent.GetComponent<VehicleDamage>().VehicleRespawnPart1 -= VehicleRespawnPart1;
            transform.parent.GetComponent<VehicleDamage>().VehicleRespawnPart2 -= VehicleRespawnPart2;
        }

        private void Update()
        {
            if (b_InitDone &&
                LapCounterAndPosition.instance.posList.Count > vehicleInfo.playerNumber)
            {
                CheckStartLine();
                CheckCollision();
            } 
        }


        public List<Transform> impactPosList = new List<Transform>();
        public List<Vector3> lastPosList = new List<Vector3>();

        public bool b_LastPosNull = true;
        public Vector3 ImpactPosition = Vector3.zero;
        void CheckCollision()
        {
            // Init the detection after the vehicle has respawned
            if (b_LastPosNull)
            {
                for (var i = 0; i < impactPosList.Count; i++)
                    lastPosList[i] = impactPosList[i].position;

                b_LastPosNull = false;
            }

            //-> Check collision with start line
            for (var i = 0; i < impactPosList.Count; i++)
            {
                RaycastHit hit;
                if (!b_LastPosNull && impactPosList[i].gameObject.activeInHierarchy && lastPosList[i] != impactPosList[i].position)
                {
                    if (Physics.Linecast(lastPosList[i], impactPosList[i].position, out hit, layerMaskLapCounter))
                    {
                        if (hit.transform.GetComponent<LapCounterStartLine>()/* &&
                            VehiclesRef.instance.listVehicles[vehicleID].GetComponent<VehicleInfo>().b_IsVehicleAvailableToMove*/)
                        {
                            LapCounterStartLine lapCounterStartLine = hit.transform.GetComponent<LapCounterStartLine>();

                            if (lapCounterStartLine.State == 0 && !vehicleInfo.b_IsRespawn)
                            {
                                //Debug.Log("Start Line Detected -> " + hit.transform.name);

                                bIsPlayerIntoBufferZone = false;

                                if (CheckWrongDirection(impactPosList[i], objBufferOutOut))
                                {
                                    //-> Destroy the vehicle when it collides with the Start line
                                    VehicleDamage vehicleDamage = VehiclesRef.instance.listVehicles[vehicleID].GetComponent<VehicleDamage>();
                                    vehicleDamage.ImpactPosition = hit.point;
                                    vehicleDamage.VehicleExplosionAction.Invoke();
                                    if (CanvasInGameUIRef.instance.listPlayerUIElements[vehicleID].listRectTransform[2].gameObject.activeSelf)
                                        CanvasInGameUIRef.instance.listPlayerUIElements[vehicleID].listRectTransform[2].gameObject.SetActive(false);
                                }
                                else
                                {
                                    //howManyLap ++;
                                    //Debug.Log("New Lap");
                                    LapCounterAndPosition.instance.updateLapValueList.Add(vehicleID);
                                    DoSomethingAfterALap?.Invoke();
                                }
                                StartCoroutine(LapDoneRoutine());
                            }

                            if (lapCounterStartLine.State == 1)
                            {
                                bIsPlayerIntoBufferZone = true;
                            }

                        }
                    }
                }
            }

            for (var i = 0; i < impactPosList.Count; i++)
            {
                lastPosList[i] = impactPosList[i].position;
            }
        }



      
        
        public bool CheckWrongDirection(Transform dir01= null, Transform dir02=null)
        {
            if (!dir01) dir01 = impactPosList[0];
            if (!dir02) dir02 = objBufferOutOut;


             value = Vector3.Dot(dir01.forward, -dir02.forward);// (dir02.position - dir01.position).normalized);

            // If value between .1f and 1 the vehicle is in the good way
            // if value < .1f the vehicle is in the wrong way
            if (value > 0.1f)
            {
                //Debug.Log("Wrong way -> " + value);
                return true;
            }
            //Debug.Log("Good way -> " + value);
            return false;
        }

        
        void VehicleRespawnPart1()
        {
            b_LastPosNull = true;
            bIsPlayerIntoBufferZone = false;
            Lock = true;
        }
        void VehicleRespawnPart2()
        {
            b_LastPosNull = true;
            bIsPlayerIntoBufferZone = false;
            Lock = false;
        }


        public IEnumerator LapDoneRoutine()
        {
            Lock = true;
            bIsPlayerIntoBufferZone = false;
            float t = 0;

            while (t < 3)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime;
                    if (!Lock)
                        t = 3;
                }
                yield return null;
            }

            Lock = false;
            yield return null;
        }

        public void PlayAudioAfterLap(int clipID =0)
        {
            if (!vehicleInfo.GetComponent<VehicleAI>().enabled &&
                Countdown.instance.b_IsCountdownEnded &&
                LapCounterAndPosition.instance.posList[vehicleInfo.playerNumber].howLapDone > 1 &&
                LapCounterAndPosition.instance.posList[vehicleInfo.playerNumber].howLapDone <= LapCounterAndPosition.instance.howManyLapsInTheCurrentRace &&
                !LapCounterAndPosition.instance.posList[vehicleInfo.playerNumber].IsRaceComplete)
                SoundFxManager.instance.Play(SfxList.instance.listAudioClip[clipID], 1);
        }


        public void CheckStartLine()
        {
            if (objBufferOutOut &&
                InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer > vehicleID)
            {
                 GameObject objWrongWay = CanvasInGameUIRef.instance.listPlayerUIElements[vehicleID].listRectTransform[2].gameObject;
                 if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, distanceDetection, layerMaskLapCounter))
                 {
                     if (hit.transform.GetComponent<LapCounterStartLine>() &&
                         VehiclesRef.instance.listVehicles.Count > vehicleID && 
                         VehiclesRef.instance.listVehicles[vehicleID].GetComponent<VehicleInfo>().b_IsVehicleAvailableToMove)
                     {
                          LapCounterStartLine lapCounterStartLine = hit.transform.GetComponent<LapCounterStartLine>();

                          if (lapCounterStartLine.State == 0)
                          {
                              if (CheckWrongDirection(transform, objBufferOutOut))
                              {
                                 if (!objWrongWay.activeSelf) objWrongWay.SetActive(true);
                              }
                         }
                         else if (objWrongWay.activeSelf) objWrongWay.SetActive(false);
                     }
                     else if (objWrongWay.activeSelf) objWrongWay.SetActive(false);
                 }
                 else
                     if (objWrongWay.activeSelf) objWrongWay.SetActive(false);
                     
            }

        }
    }
}

