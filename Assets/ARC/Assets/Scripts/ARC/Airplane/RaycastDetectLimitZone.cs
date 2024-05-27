// Description: RaycastDetectLimitZone. (attached to the vehicle).
// Use to detect if the player go outside the game area.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TS.ARC;
using TS.Generics;

public class RaycastDetectLimitZone : MonoBehaviour
{
    public bool                     b_InitDone;
    private bool                    b_InitInProgress;
    private VehiclePrefabInit       vehiclePrefabInit;

    public WarningSignal            warningSignal;
    public GameObject               objWarningSignal;
    public float                    distanceDetection = 20;

    public float                    currentHitDistance;
    public float                    alpha = 0;

    public List<int>                listLayersUsedBylayerMaskWarning = new List<int>();
    public LayerMask                layerMaskWarning;

    public bool                     b_LimitDetected;


    //-> Initialisation
    public bool bInitRaycastDetectLimitZone()
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
        vehiclePrefabInit = transform.parent.GetComponent<VehiclePrefabInit>();

        int playerNumber = GetComponent<VehicleInfo>().playerNumber;

        WarningSignal[] allWarningSignal = FindObjectsOfType<WarningSignal>(true);

        foreach (WarningSignal ws in allWarningSignal)
        {
            if (ws.ID == playerNumber)
            {
                warningSignal = ws;
                objWarningSignal = ws.transform.GetChild(0).gameObject;
            }
        }

        //-> Init LayerMask
        string[] layerUsed = new string[listLayersUsedBylayerMaskWarning.Count];
        for (var i = 0; i < listLayersUsedBylayerMaskWarning.Count; i++)
            layerUsed[i] = LayerMask.LayerToName(LayersRef.instance.layersListData.listLayerInfo[listLayersUsedBylayerMaskWarning[i]].layerID);
        layerMaskWarning = LayerMask.GetMask(layerUsed);

        b_InitDone = true;
        //Debug.Log("Init: RaycastDetectLimitZone -> Done");
        yield return null;
    }

    void Update()
    {
        if (b_InitDone && vehiclePrefabInit && vehiclePrefabInit.b_InitDone && warningSignal)
        {
            if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, distanceDetection, layerMaskWarning))
            {


                int limitZoneLayer = LayersRef.instance.layersListData.listLayerInfo[8].layerID;
                if (hit.transform.gameObject.layer == limitZoneLayer)
                {
                    Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
                    currentHitDistance = hit.distance;
                    alpha = 1 - currentHitDistance / distanceDetection;

                    if (objWarningSignal && !objWarningSignal.activeSelf)
                    {
                        warningSignal.warningSpeed = 1;
                        objWarningSignal.SetActive(true);
                    }

                    warningSignal.warningSpeed = 1 - 1 * ((currentHitDistance - 0) / (distanceDetection - 0));
                    b_LimitDetected = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, transform.forward * distanceDetection, Color.green);

                    currentHitDistance = hit.distance;
                    if (objWarningSignal && objWarningSignal.activeSelf) objWarningSignal.SetActive(false);

                    b_LimitDetected = false;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, transform.forward * distanceDetection, Color.green);

                currentHitDistance = hit.distance;
                if (objWarningSignal && objWarningSignal.activeSelf) objWarningSignal.SetActive(false);

                b_LimitDetected = false;
            }
        }
    }

    public void VDVehicleExplosion()
    {
        warningSignal.gameObject.SetActive(false);
        b_LimitDetected = false;
    }
}
