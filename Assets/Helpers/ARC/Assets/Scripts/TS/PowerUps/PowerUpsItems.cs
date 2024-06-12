// Description: PowerUpsItems:
//-Give info about the power-up (PowerType 1:Repair | 2:Machine Gun | 3:Shield | 5: Mine | 5: Missile)
//-Rotate the power if needed
//-Disable the power during 2s if the power is selected by a vehicle. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class PowerUpsItems : MonoBehaviour
    { 
        [HideInInspector]
        public bool                     SeeInspector;
        [HideInInspector]
        public bool                     moreOptions;
        [HideInInspector]
        public bool                     helpBox = true;

        public bool                     rotationOverrideByGrp_PowerUp = true;
        public bool                     b_EnableRotation = true;
        [HideInInspector]
        public float                    rotationSpeed = 100;
        public int                      PowerType;
        public AudioSource              aSource;

        public GameObject               Grp_Item;
        private Collider                boxCollider;

        public bool                     b_HideItemWhenActivated = true;


        private GrpPowerUp              grpPowerUp;
        private int                     whichPowerUpInList;

        public bool randomPU = false;
        private int durationRandomPU = 2;
        private float t = 0;
        public List<int> randomPUList = new List<int>();
        private int currentRandomSelectedPU =0;
        void Start()
        {
            boxCollider = GetComponent<Collider>();

            if(transform.parent && transform.parent.GetComponent<GrpPowerUp>())
            {
                grpPowerUp = transform.parent.GetComponent<GrpPowerUp>();

                for (var i = 0; i < grpPowerUp.listLookAtPowerUp.Count; i++)
                {
                    if (grpPowerUp.listLookAtPowerUp[i].powerUpToLookAT == transform)
                    {
                        whichPowerUpInList = i;

                    }
                }
            }
            ReceiveDamage();
        }

        void Update()
        {
            if(Grp_Item.activeSelf && b_EnableRotation)
            {
                if (!rotationOverrideByGrp_PowerUp)
                    Grp_Item.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                else if (rotationOverrideByGrp_PowerUp)
                    Grp_Item.transform.localRotation = PowerUpsItemsGlobalParams.instance.refRotation.localRotation;
            }

            if (randomPU)
            {
                t += Time.deltaTime;
                if(t > durationRandomPU)
                {
                    currentRandomSelectedPU = (currentRandomSelectedPU+1)% randomPUList.Count;

                    PowerType = randomPUList[currentRandomSelectedPU];
                    t = 0;
                }
            }
        }


        public void PowerUpItemActivated(bool bIsPlayerTwo)
        {
            if (aSource && aSource.gameObject.activeInHierarchy)
            {
                // Spacial sound for Player 2
                if (bIsPlayerTwo)
                    aSource.spatialBlend = 0;

                aSource.Play();
            }
            if (b_HideItemWhenActivated) {
                boxCollider.enabled = false;
                Grp_Item.SetActive(false);
                StartCoroutine(PowerUpItemInitRoutine(bIsPlayerTwo));
            }
           
        }


        public IEnumerator PowerUpItemInitRoutine(bool bIsPlayerTwo)
        {
            float t = 0;

            while (t < 2)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime;
                }
                yield return null;
            }

            boxCollider.enabled = true;
            Grp_Item.SetActive(true);

            if (transform.parent && transform.parent.GetComponent<GrpPowerUp>())
                grpPowerUp.listLookAtPowerUp[whichPowerUpInList].b_PowerUpAvailable = true;


            if (bIsPlayerTwo)
                aSource.spatialBlend = 1;

            yield return null;
        }

        public virtual void ReceiveDamage()
        {
            Debug.Log("Original");
          // this.ReceiveDamage2();
        }
    }
}