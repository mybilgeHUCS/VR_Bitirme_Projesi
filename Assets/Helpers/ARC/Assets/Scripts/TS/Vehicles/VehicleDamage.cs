using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TS.Generics;
using TS.ARC;
using System;
using UnityEngine.Events;

namespace TS.Generics
{
    public class VehicleDamage : MonoBehaviour
    {
        public bool b_InitDone;
        private bool b_InitInProgress;
        private VehiclePrefabInit vehiclePrefabInit;

        private VehicleInfo vehicleInfo;
        private VehicleAI vehicleAI;
        private VehiclePathFollow vehiclePathFollow;
        private VehicleDamage vehicleDamage;
        public Action VehicleExplosionAction;               // Vehicle is destroyed. Life = 0
        public Action VehicleRespawnPart1;                  // Transition between the position where vehicle is destroyed and the position where the vehicle is respawned
        public Action VehicleRespawnPart2;                  // Actions when the vehicle is respawned (init) 

        public Action<int> VehicleLoseLife;
        public Action<int> VehicleWinLife;

        public UnityEvent CustomInit;

        [HideInInspector]
        public Vector3 respawnPoint;
        [HideInInspector]
        public Quaternion respawnRotation;
        [HideInInspector]
        public int checkPointID = 0;


        [Header("Damage and Explosion")]
        public int lifePoints = 10;
        [HideInInspector]
        public int refLifePoints;

        public ParticleSystem damageParticle;
        private ParticleSystem.EmissionModule damageParticleEmission;

        public GameObject objExplosion;
        public AudioSource aSourceExplosion;                       // AudioSource used to play vehicle explosion sound
        public AudioSource aSourceHit;                       // AudioSource used to play vehicle hit sound

        //public bool b_IsShieldEnabled;


        // Detect wall collisions
        public float sphereRadius = .5f;

        public List<Transform> impactPosList = new List<Transform>();
        public List<Vector3> lastPosList = new List<Vector3>();

        private bool b_LastPosNull = true;
        public Vector3 ImpactPosition = Vector3.zero;

        public List<int> listLayersUsedBylayerMask = new List<int>();
        public LayerMask layerMask;

        public float offsetDistanceMax = 50;
        public float offsetDistanceMin = -200;

        public bool b_Invincibility = false;
        public float invincibilityDuration = 2;


        //public bool b_StartLineInvincibility;


        //-> UI life bar
        private RectTransform rectLifeBar;
        private float refWidthLifeBar;

        private void Start()
        {
            //-> Init LayerMask
            string[] layerUsed = new string[listLayersUsedBylayerMask.Count];
            for (var i = 0; i < listLayersUsedBylayerMask.Count; i++)
                layerUsed[i] = LayerMask.LayerToName(LayersRef.instance.layersListData.listLayerInfo[listLayersUsedBylayerMask[i]].layerID);
            layerMask = LayerMask.GetMask(layerUsed);

            vehicleInfo = GetComponent<VehicleInfo>();
            vehiclePathFollow = GetComponent<VehiclePathFollow>();
            vehicleDamage = GetComponent<VehicleDamage>();
            vehicleAI = GetComponent<VehicleAI>();
            VehicleExplosionAction += VDVehicleExplosion;
            VehicleRespawnPart2 += VDVehicleRespawn;

            for (var i = 0; i < impactPosList.Count; i++)
                lastPosList[i] = impactPosList[i].position;

            UpdateDamageParticleFx(0);
        }

        public void OnDestroy()
        {
            VehicleExplosionAction -= VDVehicleExplosion;
            VehicleRespawnPart2 -= VDVehicleRespawn;
        }

        //-> Initialisation
        public bool bInitVehicleDamage()
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


            if (DataRef.instance.vehicleGlobalData)
            {
                int vehicleDataID = vehiclePrefabInit.vehicleDataID;

                //-> Init vehicle booster using vehicle data
                // P1 | P2
                if (IsItAPlayer())
                {
                    lifePoints = DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].damageResistance;
                    refLifePoints = DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].damageResistance;

                    //-> Access UI Life Bar
                    rectLifeBar = CanvasInGameUIRef.instance.listPlayerUIElements[vehicleInfo.playerNumber].listRectTransform[5];
                    refWidthLifeBar = rectLifeBar.rect.width;

                }
                else
                {
                    lifePoints = DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].damageResistanceAI;
                    refLifePoints = DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].damageResistanceAI;
                }
            }

            //-> Special Init done with methods connected in the Inspector
            CustomInit?.Invoke();

            b_InitDone = true;
            yield return null;
        }

        void Update()
        {
            if (b_InitDone && vehiclePrefabInit && vehiclePrefabInit.b_InitDone)
            {
                if (vehicleInfo.b_IsVehicleAvailableToMove)
                    CheckCollision();
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
                        if (hit.transform.GetComponent<PU_MineCollider>() &&
                            b_Invincibility/* &&
                            !b_StartLineInvincibility*/)
                        {
                            break;
                        }
                       
                        b_LastPosNull = true;
                        ImpactPosition = hit.point;
                        VehicleExplosionAction.Invoke();

                        //-> Check Mine Collision Case
                        CheckMineCollision(hit.transform.gameObject);
                        break;
                    }
                }
            }

            for (var i = 0; i < impactPosList.Count; i++)
                lastPosList[i] = impactPosList[i].position;

        }


        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            for (var i = 0; i < impactPosList.Count; i++)
            {
                if (impactPosList[i])
                    Gizmos.DrawSphere(impactPosList[i].position, sphereRadius);
            }

        }



        void VDVehicleExplosion()
        {
            if (!vehicleInfo.b_IsRespawn)
            {
                vehicleInfo.b_IsRespawn = true;
                if (objExplosion)
                {
                    objExplosion.transform.position = ImpactPosition;
                    objExplosion.SetActive(true);
                }
                if (aSourceExplosion && aSourceExplosion.gameObject.activeInHierarchy) aSourceExplosion.Play();

                StartCoroutine(RespawnRoutine());
            }
        }

        void VDVehicleRespawn()
        {
           
            if (objExplosion)
            {
                objExplosion.SetActive(false);
                
            }

            //-> Reset Life Bar
            if (IsItAPlayer())
            {
                rectLifeBar.sizeDelta = new Vector2(refWidthLifeBar, rectLifeBar.rect.height);
            }
        }


        public IEnumerator RespawnRoutine()
        {
            //Debug.Log("Here");
            vehicleInfo.b_IsVehicleAvailableToMove = false;
            UpdateDamageParticleFx(0);

            //-> Update the P1 P2 UI Life Gauge
            UILifeGaugeUpdate(-refLifePoints);

            float t = 0;

            if (GetComponent<VehicleAI>().enabled)
            {
                while(t< 2.5f)
                {
                    if(!PauseManager.instance.Bool_IsGamePaused)
                        t += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (t < 1)
                {
                    if (!PauseManager.instance.Bool_IsGamePaused)
                        t += Time.deltaTime;
                    yield return null;
                }
            }

            respawnPoint = GetClosestCheckpoint();

            if(VehicleRespawnPart1 != null)
                VehicleRespawnPart1.Invoke();

            t = 0;
            while (t < 1)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                    t += Time.deltaTime;
                yield return null;
            }

            if (VehicleRespawnPart2 != null)
                VehicleRespawnPart2.Invoke();

            lifePoints = refLifePoints;

            vehicleInfo.b_IsVehicleAvailableToMove = true;


            vehicleInfo.b_IsRespawn = false;
            yield return null;
        }


        public Vector3 GetClosestCheckpoint()
        {
            #region
            if (GetComponent<VehiclePathFollow>())
            {
                Transform[] checkpoints = GetComponent<VehiclePathFollow>().Track.checkpoints.ToArray();
                

                Vector3 bestTarget = Vector3.zero;
                float closestDistanceSqr = Mathf.Infinity;
                Vector3 currentPosition = transform.position;

                for(var i = 0;i< checkpoints.Length; i++)
                {
                    Vector3 directionToTarget = checkpoints[i].position - currentPosition;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        checkPointID = i;
                        bestTarget = checkpoints[i].position;
                        respawnRotation = NewRotationAfterRespawn();
                    }
                }


                //-> Special condition due to StartLine and the lap counter
                if(checkPointID == 0 && LapCounterAndPosition.instance)
                {
                    Path Track = vehiclePathFollow.Track;
                    int vehicleID = vehicleInfo.playerNumber;

                    //-> Vehicle is between Last Checkpoint and 1st checkpoint
                    if (!LapCounterAndPosition.instance.lapCounterBadgeList[vehicleInfo.playerNumber].Lock &&
                        LapCounterAndPosition.instance.lapCounterBadgeList[vehicleInfo.playerNumber].bIsPlayerIntoBufferZone)
                    {

                        bestTarget = Track.TargetPositionOnPath((Track.DistanceFromCheckpointToStart(Track.checkpoints.Count-1)));
                        respawnRotation = NewRotationAfterRespawnDist((Track.DistanceFromCheckpointToStart(Track.checkpoints.Count-1)));
                    }
                    //-> Vehicle is after startLine
                    else
                    {
                        bestTarget = Track.TargetPositionOnPath((Track.DistanceFromCheckpointToStart(0) + offsetDistanceMax) % Track.pathLength);
                        respawnRotation = NewRotationAfterRespawnDist((Track.DistanceFromCheckpointToStart(0) + offsetDistanceMax) % Track.pathLength);
                    }
                }

                return bestTarget;
            }
            else
                return Vector3.zero;
            #endregion
        }



        public Quaternion NewRotationAfterRespawn()
        {
            Quaternion newRotation;
            Path Track = GetComponent<VehiclePathFollow>().Track;                              // find the next position for the target

            Vector3 targetPos = Track.TargetPositionOnPath((Track.DistanceFromCheckpointToStart(checkPointID) + 20) % Track.pathLength);

            Vector3 dir = (targetPos -
             Track.checkpoints[(checkPointID) % Track.checkpoints.Count].transform.position).normalized;

            newRotation = Quaternion.LookRotation(dir);                    // find the new rotation for the target

            return newRotation;
        }

        public Quaternion NewRotationAfterRespawnDist(float dist)
        {
            Quaternion newRotation;
            Path Track = GetComponent<VehiclePathFollow>().Track;                              // find the next position for the target

            Vector3 targetNextPos = Track.TargetPositionOnPath((dist + 20) % Track.pathLength);
            Vector3 targetCurrentPos = Track.TargetPositionOnPath((dist) % Track.pathLength);

            Vector3 dir = (targetNextPos - targetCurrentPos).normalized;

            newRotation = Quaternion.LookRotation(dir);                    // find the new rotation for the target

            return newRotation;
        }


        //-> Update Vehicle Damage
        public void LifeUpdate(int value,int whatTypeOfDammage = -1, AudioClip hitSound = null, float hitVolume = 1)
        {
            #region
            if (!b_Invincibility/* &&
               !b_StartLineInvincibility*/)
            {
                lifePoints += value * 1;

                if (aSourceHit && !vehicleAI.enabled && hitSound != null && aSourceHit.gameObject.activeInHierarchy)
                {
                    aSourceHit.volume = hitVolume;
                    aSourceHit.clip = hitSound;
                    aSourceHit.Play();
                }

                if (damageParticle)
                {
                    damageParticleEmission = damageParticle.emission;

                    float newRateOverTime = damageParticleEmission.rateOverDistance.constant + .5f * Mathf.Abs(value);
                    damageParticleEmission.rateOverDistance = newRateOverTime;
                }

                //-> Update the P1 P2 UI Life Gauge
                UILifeGaugeUpdate(value);



                if (value < 0)
                    VehicleLoseLife?.Invoke(whatTypeOfDammage);
                if (value > 0)
                    VehicleWinLife?.Invoke(whatTypeOfDammage);


                if (lifePoints <= 0)
                {
                    ImpactPosition = transform.position;
                    VehicleExplosionAction.Invoke();
                }    
            }
           
            #endregion
        }


        public void CheckMineCollision(GameObject inObj)
        {
            //Debug.Log("inObj: " + inObj.name);
            if(inObj.gameObject.GetComponent<PU_MineCollider>())
                inObj.gameObject.GetComponent<PU_MineCollider>().pU_MinePrefab.DestroyMine();
        }


        public void UpdateDamageParticleFx(int value = 0)
        {
            if (damageParticle)
            {
                damageParticleEmission = damageParticle.emission;
                damageParticleEmission.rateOverDistance = value;
            }
        }
        public IEnumerator InvincibiltyRoutine(GameObject Grp_EnemyDetector)
        {
            b_Invincibility = true;
            float t = 0;
            yield return new WaitUntil(() => Grp_EnemyDetector.activeSelf);

            if (GetComponent<PowerUpsSystemAssistant>())
            {
                PowerUpsSystem powerUpsSystem = GetComponent<PowerUpsSystem>();
                StartCoroutine(GetComponent<PowerUpsSystemAssistant>().puShield.ShieldForceFieldRoutine(
                   powerUpsSystem.aSourcePowerUps, this, invincibilityDuration));

            }
               


            while (t < invincibilityDuration)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime;
                }
                yield return null;
            }

            b_Invincibility = false;
            yield return null;
        }




        //-> Update the P1 P2 UI Life Gauge
        public void UILifeGaugeUpdate(int lifePoints)
        {
            if (IsItAPlayer())
            {
                float widthToRemove = lifePoints * (refWidthLifeBar / refLifePoints);

                float newSize = rectLifeBar.sizeDelta.x + widthToRemove;
                newSize = Mathf.Clamp(newSize,0, refWidthLifeBar) ;

                rectLifeBar.sizeDelta = new Vector2(newSize, rectLifeBar.rect.height);
            }
        }


        bool IsItAPlayer()
        {
            //-> Rest Life Bar
            if (vehicleInfo.playerNumber == 0 ||
                vehicleInfo.playerNumber == 1 && InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2)
            {
                return true;
            }
            else
                return false;

        }


        public void InitBloodFx()
        {
            if(vehicleInfo.playerNumber < InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer)
            {
                CanvasInGameTag canvasInGameTag = FindObjectOfType<CanvasInGameTag>();
                BloodFx[] all = canvasInGameTag.GetComponentsInChildren<BloodFx>(true);

                foreach (BloodFx obj in all)
                {
                    if (obj.ID == vehicleInfo.playerNumber)
                    {
                        obj.InitBloodFx(this);
                    }
                }
            }
        }

        public void InitUIBulletDelegate()
        {
            int playerNumber = vehicleInfo.playerNumber;
            if (playerNumber < InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer)
            {
                CanvasInGameTag canvasInGameTag = FindObjectOfType<CanvasInGameTag>();
                BulletFx[] all = canvasInGameTag.GetComponentsInChildren<BulletFx>(true);

                foreach (BulletFx obj in all)
                {
                    if (obj.ID == vehicleInfo.playerNumber)
                    {
                        obj.InitBulletFx(this);
                    }
                }
            }
        }


        public void SetInvisibility(bool state)
        {
            b_Invincibility = state;
            if (state)
            {
                GetComponent<PowerUpsSystem>().DisableAllPowerUps();

                GetComponent<PowerUpsSystemAssistant>().StopAllCoroutines();

                if (GetComponent<PowerUpsSystemAssistant>())
                    GetComponent<PowerUpsSystemAssistant>().puShield.Grp_Shield.SetActive(true);
            }

        }

    }

}
