// Description: PowerUpsAIDetectVehicle: 
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TS.Generics
{
    public class PowerUpsAIDetectVehicle : MonoBehaviour
    {
        public bool             b_VehicleLocked;
        public bool             b_VehicleEnemyDetected;
        public GameObject       currentPlaneDetected;

        public Vector3          increaseColliderSize = new Vector3(2,2,0);
        private Vector3         refColliderSize;
        private BoxCollider     boxCollider;
        public float            increaseSizeSpeed = 2;

        public class objDetected
        {
            public GameObject obj;
            public float distance;
            public objDetected(GameObject newObj, float newDistance)
            {
                obj = newObj;
                distance = newDistance;
            }
        }
        public List<objDetected> listObjDectected = new List<objDetected>();

        public GameObject       objForRefDistance;

        public List<int>        listLayersUsedBym_LayerMask = new List<int>();

        public LayerMask        m_LayerMask;

        public bool             b_AI = true;

    
        private void Start()
        {
            boxCollider = GetComponent<BoxCollider>();
            refColliderSize = transform.localScale;
        }

        void FixedUpdate()
        {
           if(objForRefDistance) MyCollisions();
        }

        void MyCollisions()
        {
            //Use the OverlapBox to detect if there are any other colliders within this box area.
            //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, transform.rotation, m_LayerMask);
            int i = 0;

            int howManyVehicleDetected = 0;
            listObjDectected.Clear();
            b_VehicleLocked = false;
            //Check when there is a new collider coming into contact with the box
            while (i < hitColliders.Length)
            {
                //Output all of the collider names
                if (hitColliders[i].tag == "VehicleCapsuleCol")
                {
                    // Vehicle is locked keep this vehicle the target
                    if (hitColliders[i].gameObject == currentPlaneDetected)
                    {
                        b_VehicleLocked = true;
                        howManyVehicleDetected = 1;
                        break;
                    }

                    //Debug.Log("Hit : " + hitColliders[i].name + i);
                    howManyVehicleDetected++;
                    float distance = (float)Vector3.Distance(hitColliders[i].transform.position, objForRefDistance.transform.position);
                    listObjDectected.Add(new objDetected(hitColliders[i].gameObject, distance));
                }

                //Increase the number of Colliders in the array
                i++;
            }

            if (b_VehicleLocked)
            {
                if (refColliderSize + increaseColliderSize != boxCollider.size)
                    transform.localScale = Vector3.MoveTowards(transform.localScale, refColliderSize + increaseColliderSize, Time.deltaTime * increaseSizeSpeed);
            }

            // A new target is selected
            if(listObjDectected.Count > 0 &&
                !b_VehicleLocked)
            {
                var reOrder = listObjDectected.OrderByDescending(g => g.distance).Reverse();

                //-> Check if the detected vehicle is already use as a target by an other vehicle || If it is a Player the target can be same as other vehicles
                if (!AlreadyDetected(reOrder.First().obj)
                    ||
                    !b_AI)
                {
                    currentPlaneDetected = reOrder.First().obj;
                    b_VehicleLocked = true;
                    b_VehicleEnemyDetected = true;
                    //Debug.Log(listObjDectected.Count + ": Closest: " + reOrder.First().obj.name);
                }
            }
           

            if (howManyVehicleDetected == 0)
            {
                ResetDetector();
            }
        }

        private void ResetDetector()
        {
            //Debug.Log("No Collision : ");
            currentPlaneDetected = null;
            b_VehicleEnemyDetected = false;
            b_VehicleLocked = false;
            transform.localScale = refColliderSize;
        }


        //-> Use with Missile power up
        public List<Transform> ReturnListTransformsDectected()
        {
            //Use the OverlapBox to detect if there are any other colliders within this box area.
            //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, transform.rotation, m_LayerMask);
            int i = 0;
            List<Transform> listTransformsDectected = new List<Transform>();

            //Check when there is a new collider coming into contact with the box
            while (i < hitColliders.Length)
            {
                //Output all of the collider names
                if (hitColliders[i].tag == "VehicleCapsuleCol")
                {
                    listTransformsDectected.Add(hitColliders[i].transform);
                }

                //Increase the number of Colliders in the array
                i++;
            }

            return listTransformsDectected;
        }


        public bool Bool_Init()
        {
            //-> AI case

            //-> P1/P2 Case


            return true;
        }

        public bool AlreadyDetected(GameObject objRef)
        {
            for (var i = 0; i < VehiclesRef.instance.listVehicles.Count; i++)
            {
                if (VehiclesRef.instance.listVehicles[i].GetComponent<PowerUpsSystemAssistant>().puMachineGun.DetectVehicleFront.currentPlaneDetected == objRef.gameObject)
                    return true;
            }
            return false;
        }
    }
}
