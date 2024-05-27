// Description: PowerUpsItemsGlobalParams: Rotate ref Item if needed. Allows to generate a new Power-up group
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class PowerUpsItemsGlobalParams : MonoBehaviour
    {
        public static PowerUpsItemsGlobalParams instance = null;

        public bool         b_EnableRotation = true;
        public float        rotationSpeed = 100;
        public Transform    refRotation;

        public GameObject   refPowerUpPrefab;

        [HideInInspector]
        public bool         SeeInspector;
        [HideInInspector]
        public bool         moreOptions;
        [HideInInspector]
        public bool         helpBox = true;

        public List<GameObject> listPowerUpPrefab = new List<GameObject>();
        [HideInInspector]
        public int              currentPowerUpSelection =0;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        void Update()
        {
            if(refRotation && b_EnableRotation)
                refRotation.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}