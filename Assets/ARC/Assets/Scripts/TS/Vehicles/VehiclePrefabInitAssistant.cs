using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace TS.Generics
{
    public class VehiclePrefabInitAssistant : MonoBehaviour
    {
        public bool EnableObject(GameObject obj)
        {
            obj.SetActive(true);
            return true;
        }
        public bool DisableObject(GameObject obj)
        {
            obj.SetActive(false);
            return true;
        }

        public bool UpdateModelLayer(GameObject obj)
        {
            Transform[] children = obj.GetComponentsInChildren<Transform>();

            //-> Change the layer of the 3D models when the player 1 vehicle is displayed on vehicle selection menu page
            if (transform.parent.GetComponent<GarageTagPivot>().ID == 4)
            {
                foreach (Transform child in children)
                {
                    child.gameObject.layer = LayersRef.instance.layersListData.listLayerInfo[5].layerID;    // Layer Triggers
                }
            }

            //-> Change the layer of the 3D models when the player 2 vehicle is displayed on vehicle selection menu page
            if (transform.parent.GetComponent<GarageTagPivot>().ID == 5)
            {
                foreach(Transform child in children)
                {
                    child.gameObject.layer = LayersRef.instance.layersListData.listLayerInfo[8].layerID;    // Layer LimitZone
                }
            }

            return true;
        }

       
    }
}