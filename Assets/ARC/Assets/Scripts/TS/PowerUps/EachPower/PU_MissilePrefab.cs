// Description: PU_MissilePrefab: Attached the missile prefab. Managed the missile behavior
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [System.Serializable]
    public class PU_MissilePrefab : MonoBehaviour
    {
        bool                    bInit;

        public bool             infiniteLifeTime;
        public float            lifeTime = 5;
        public GameObject       Grp_Missile;

        private Rigidbody       rb;

        public float            Force = 300;
        private float           maxSpeedCurrent = 0;
        public float            startSpeed = 0;
        public float            maxSpeedTarget = 100;
        public float            speedToReachMaxSpeed = 100;


        public bool             b_ImpactWithOtherObject = false;

        public GameObject       objExplosion;
        public float            explosionDuration = 3;

        public Transform        objTarget;

        VehicleInfo             vehicleInfo;
        public float            rotSpeed;
        public float            speedToReachMaxRotSpeed = 100;

        private float           step;

        public List<Transform>  impactPosList = new List<Transform>();
        public List<Vector3>    lastPosList = new List<Vector3>();

        private bool            b_LastPosNull = true;
        public Vector3          ImpactPosition = Vector3.zero;


        public List<int>        listLayersUsedBylayerMask = new List<int>();
        public LayerMask        layerMask;

        public GameObject       vehicle;
        public float            startMagnitude;
        public float            parentDuration = .15f;


        public bool             bUnParent = false;

        public GameObject       Grp_Particle;

        //-> Check the amount rotation 
        Vector3 startDirectionRef;
        public float maxAngle = -.2f;

        public int lifeCost = -3;

        public AudioClip    aClipMissileHit;
        public float        aVolumeMissileHit;


        public bool Init(Transform target,int whichCamToCheck,MonoBehaviour mono,int _lifeCost)
        {
            //Debug.Log(target);
            objTarget = target;
            if (objTarget)
            {
                while (objTarget.GetComponent<Rigidbody>() == null)
                {
                    objTarget = objTarget.transform.parent;
                }

                vehicleInfo = objTarget.GetComponent<VehicleInfo>();
                objTarget.GetComponent<PowerUpsSystemAssistant>().pu_Missile.AddToMissileToTheList(this.gameObject);
            }
            else
            {
                rotSpeed = 0;
                speedToReachMaxRotSpeed = 0;
            }

            vehicle = mono.gameObject;
            startMagnitude = vehicle.GetComponent<Rigidbody>().velocity.magnitude;


            rb = GetComponent<Rigidbody>();
            rb.velocity = vehicle.GetComponent<Rigidbody>().velocity;

            startDirectionRef = vehicle.transform.forward;

            lifeCost = _lifeCost;

            bInit = true;
            return true;
        }

        private void Start()
        {
            maxSpeedCurrent = 0;

            StartCoroutine(DestroyMissileRoutine());

            string[] layerUsed = new string[listLayersUsedBylayerMask.Count];
            for (var i = 0; i < listLayersUsedBylayerMask.Count; i++)
                layerUsed[i] = LayerMask.LayerToName(LayersRef.instance.layersListData.listLayerInfo[listLayersUsedBylayerMask[i]].layerID);
            layerMask = LayerMask.GetMask(layerUsed);

            for (var i = 0; i < impactPosList.Count; i++)
                lastPosList[i] = impactPosList[i].position;
        }


        

        void Update()
        {
            if (!PauseManager.instance.Bool_IsGamePaused && bInit && !b_ImpactWithOtherObject)
            {
                if (rotSpeed != 300 && objTarget)
                    rotSpeed = Mathf.MoveTowards(rotSpeed, 300, Time.deltaTime * speedToReachMaxRotSpeed);

                if (maxSpeedCurrent != maxSpeedTarget)
                    maxSpeedCurrent = Mathf.MoveTowards(maxSpeedCurrent, maxSpeedTarget, Time.deltaTime * speedToReachMaxSpeed);

                if (objTarget)
                    step = rotSpeed * Time.deltaTime;

                if (ImpactPosition == Vector3.zero)
                    CheckCollision();

                if (transform.localPosition.z < -1.9f)
                {
                    if (!bUnParent)
                    {
                        startMagnitude = vehicle.GetComponent<Rigidbody>().velocity.magnitude;

                        rb.isKinematic = false;

                        rb.transform.SetParent(null);
                        maxSpeedCurrent = startSpeed;

                        Grp_Particle.SetActive(true);

                        bUnParent = true;
                    }
                   
                }
                else
                {
                   // t += Time.deltaTime;
                }

                CheckRotationAmount();
            }
        }

        void FixedUpdate()
        {
            if (!PauseManager.instance.Bool_IsGamePaused && bInit && !b_ImpactWithOtherObject)
            {
                MoveTheMissile();

                if (vehicleInfo && vehicleInfo.b_IsRespawn && objTarget)
                    objTarget = null;

                if (objTarget)
                   LookAtTheTarget();

                //-> Limit the missile velocity
                if(rb.velocity.magnitude > (startMagnitude + maxSpeedTarget))
                   rb.velocity = rb.velocity.normalized * (startMagnitude + maxSpeedTarget);
            }   
        }

        void MoveTheMissile()
        {
            //-> Apply movement to the missile
            if (rb.transform.parent)
            {
                rb.transform.localPosition -= new Vector3(0, 0, 10 * Time.deltaTime);
            }
            else
            {
                rb.velocity = transform.forward * (startMagnitude + maxSpeedCurrent);
            }
        }

        void CheckCollision()
        {
            // Init the detection after the vehicle has respawned
            if (b_LastPosNull)
            {
                for (var i = 0; i < impactPosList.Count; i++)
                    lastPosList[i] = impactPosList[i].position;

                b_LastPosNull = false;
            }

            //-> Check collision with a part of the stage (wall...)
            for (var i = 0; i < impactPosList.Count; i++)
            {
                RaycastHit hit;
                if (!b_LastPosNull && impactPosList[i].gameObject.activeInHierarchy && lastPosList[i] != impactPosList[i].position)
                {

                    if (Physics.Linecast(lastPosList[i], impactPosList[i].position, out hit, layerMask))
                    {
                        //Debug.Log("hit -> 1: ");
                        if (hit.transform != gameObject.transform &&
                            hit.transform != vehicle.transform &&
                            !hit.transform.GetComponent<PU_MissilePrefab>())
                        {
                            //Debug.Log("hit -> 2: " + hit.transform.name);
                            b_LastPosNull = true;
                            ImpactPosition = hit.point;

                            if (objTarget &&
                                hit.transform == objTarget &&
                                hit.transform.GetComponent<VehicleDamage>() &&
                                !hit.transform.GetComponent<VehicleDamage>().b_Invincibility)
                            {
                                //hit.transform.GetComponent<VehicleDamage>().VehicleExplosionAction.Invoke();
                                hit.transform.GetComponent<VehicleDamage>().LifeUpdate(lifeCost,1, aClipMissileHit,aVolumeMissileHit);
                                objExplosion.transform.position = ImpactPosition;
                                b_ImpactWithOtherObject = true;
                            }
                            else if (hit.transform.GetComponent<VehicleDamage>() && !objTarget)
                            {
                                // Prevent player to be touch if missile is not for him.
                            }
                            else
                            {
                                objExplosion.transform.position = ImpactPosition;
                                b_ImpactWithOtherObject = true;
                            }
                            break;

                        }


                        
                    }
                }

            }

            for (var i = 0; i < impactPosList.Count; i++)
            {
                lastPosList[i] = impactPosList[i].position;
            }
        }


       
        void LookAtTheTarget()
        {
            if (objTarget)
            {
                var targetRotation = Quaternion.LookRotation(objTarget.position - transform.position);
                rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, step));
            }
        }



        IEnumerator DestroyMissileRoutine()
        {
            float t = 0;

            while (t != lifeTime)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t = Mathf.MoveTowards(t, lifeTime, Time.deltaTime);

                    if (b_ImpactWithOtherObject)
                        break;
                }
                yield return null;
            }

            objTarget = null;

            rb.isKinematic = true;

            Grp_Missile.SetActive(false);
            //objExplosion.transform.position = ImpactPosition;
            objExplosion.SetActive(true);

            t = 0;

            while (t != explosionDuration)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                    t = Mathf.MoveTowards(t, explosionDuration, Time.deltaTime);
                yield return null;

            }

            objExplosion.SetActive(false);
            DestroyMissile();

            yield return null;
        }

        public void DestroyMissile()
        {
            Destroy(gameObject);
        }



        
        public void CheckRotationAmount()
        {

            float value = Vector3.Dot(transform.forward, startDirectionRef);// (dir02.position - dir01.position).normalized);

            // If value between .1f and 1 the vehicle is in the good way
            // if value > maxAngle the vehicle is in the wrong way
            if (value > maxAngle)
            {
                //Debug.Log("Wrong way -> " + value);
                objExplosion.transform.position = transform.position;
                b_ImpactWithOtherObject = true;
            }
        }

    }
}