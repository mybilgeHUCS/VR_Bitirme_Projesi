// Description: GaugeScale. Create Gauge scale. Used in the garage and in the vehicle selection
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace TS.Generics
{
    public class GaugeScale : MonoBehaviour
    {
        public float scaleSpeed = .5f;
        public float delay;
        public AnimationCurve scaleCurve;
        private float t;

        public CurrentText txtInfo;
        public float multiplierRangeTextValue = 5; // Use to display a value bigger than 1;

        public bool b_Init;

        public float minValue = 20;
        public float minRatio = .7f;
        public float maxValue = 30;

        public int vehicleToSelect = 0;     // 0: Gararge P1 | 1: Vehicle Selected P1 | 2: Vehicle Selected P2

        private RectTransform rect;
        private Vector2 refSizeDelta;

        public bool bInvert;

        public void NewScale(float value = 1)
        {
            StartCoroutine(scaleRectTransformRoutine(value));
        }


        IEnumerator scaleRectTransformRoutine(float value = 1, float paramID = 0)
        {
            if (!b_Init) StartCoroutine(InitGaugeRoutine(paramID));
            yield return new WaitUntil(() => b_Init);

            rect.sizeDelta = new Vector2(0, 1* refSizeDelta.y); 

            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;

            t = 0;
            while (t < delay)
            {
                if(!PauseManager.instance || PauseManager.instance && !PauseManager.instance.Bool_IsGamePaused)
                    t += Time.deltaTime / delay;
                yield return null;
            }

           
            float scaledValue = (value - minValue) / (maxValue - minValue);


            if (bInvert)
            {
                scaledValue = (1/value - minValue) / (maxValue - minValue);
            }

            t = 0;
            while (t < scaleSpeed)
            {
                if (!PauseManager.instance  || PauseManager.instance && !PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime / scaleSpeed;
                    float localZEvaluate = scaleCurve.Evaluate(t / scaleSpeed);

                    //transform.localScale = new Vector3(localZEvaluate * scaledValue, 1, 1);
                    rect.sizeDelta = new Vector2(localZEvaluate * scaledValue * refSizeDelta.x, 1 * refSizeDelta.y); 


                    // Display a text (slider value)
                    if (txtInfo)
                    {

                        txtInfo.DisplayTextComponent(txtInfo.gameObject, Math.Round((localZEvaluate * scaledValue * multiplierRangeTextValue),1).ToString());
                    }
                }
                yield return null;
            }
            yield return null;
        }

        public void InitScale(float value = 0)
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            StopAllCoroutines();
            //transform.localScale = new Vector3(value, 1, 1);
            rect.sizeDelta = new Vector2(value * refSizeDelta.x, 1 * refSizeDelta.y);;
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

        public void StopScale()
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            StopAllCoroutines();
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

        public void NewScaleUsingCarParams(float selectedParam = 0)
        {
            if(gameObject.activeInHierarchy)
                StartCoroutine(scaleRectTransformRoutine(ReturnVehicleValue(selectedParam), selectedParam));
        }

        float ReturnVehicleValue(float selectedParam = 0)
        {
            int currentVehicle = 0;
            
            //-> Case: Garage 
            if(vehicleToSelect == 0)currentVehicle = InfoVehicle.instance.currentVehicleDisplayedInTheGarage;

            //-> Case: Garage 
            if (vehicleToSelect == 1) currentVehicle = InfoVehicle.instance.listSelectedVehicles[0];

            //-> Case: Garage 
            if (vehicleToSelect == 2) currentVehicle = InfoVehicle.instance.listSelectedVehicles[1];

           return ReturnParamValue(currentVehicle, selectedParam);

            //return 0;
        }



        IEnumerator InitGaugeRoutine( float selectedParam = 0)
        {
            yield return new WaitUntil(() =>
            DataRef.instance.vehicleGlobalData.carParametersList.Count == InfoVehicle.instance.vehicleParametersInGameList.Count);

            int howManyVehicle = DataRef.instance.vehicleGlobalData.carParametersList.Count;

            List<float> valuesList = new List<float>();

            for (var i = 0;i < howManyVehicle; i++)
            {
                if(bInvert)
                    valuesList.Add(1/ReturnParamValue(i, selectedParam));
                else
                    valuesList.Add(ReturnParamValue(i, selectedParam));
                //if (selectedParam == 1) Debug.Log(selectedParam + " | " + i + " | " +ReturnParamValue(i, selectedParam));
            }

            rect = transform.parent.GetComponent<RectTransform>();
            refSizeDelta = rect.sizeDelta;

            // Maximum gauge value
            maxValue = valuesList.Max();
            // Minimum gauge
            minValue = valuesList.Min() * minRatio;



            b_Init = true;
        }


        float ReturnParamValue(int vehicleID,float selectedParam)
        {
            switch (selectedParam)
            {
                case 0:
                    return InfoVehicle.instance.vehicleParametersInGameList[vehicleID].vehicleCategory;
                case 1:
                    return InfoVehicle.instance.vehicleParametersInGameList[vehicleID].speed;
                case 2:
                    return InfoVehicle.instance.vehicleParametersInGameList[vehicleID].damageResistance;
                case 3:
                    return InfoVehicle.instance.vehicleParametersInGameList[vehicleID].boosterDuration;
                case 4:
                    return InfoVehicle.instance.vehicleParametersInGameList[vehicleID].boosterCooldown;
                case 5:
                    return InfoVehicle.instance.vehicleParametersInGameList[vehicleID].coinMultiplier;
                case 6:
                    return InfoVehicle.instance.vehicleParametersInGameList[vehicleID].cost;
                case 7:
                    return InfoVehicle.instance.vehicleParametersInGameList[vehicleID].boosterPower;
            }
            return 0;
        }

    }

}
