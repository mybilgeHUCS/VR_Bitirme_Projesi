// Description: BulletFx: Display the blood Fx when the player is hit by Mine or when the player explode
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class BloodFx : MonoBehaviour
    {
        public int                      ID;
        private VehicleDamage           vehicleDamage;
        public GameObject               grpBlood;
        private CanvasGroup             canvasGrp;

        public List<RectTransform>      bloodRectList = new List<RectTransform>();
        public AnimationCurve           animCurveBloodIn;
        public AnimationCurve           animCurveBloodOut;
        public float                    fadeInDuration = .15f;
        public float                    delay = .25f;
        public float                    fadeOutDuration = 1f;

        public AudioClip                aBloodClip;
        public float                    aBloodVolume = 1;
        private bool                    bInProgress = false;

        public bool                     bInit = false;

        public void InitBloodFx(VehicleDamage _vehicleDamage)
        {
            vehicleDamage = _vehicleDamage;
            vehicleDamage.VehicleLoseLife += VehicleLoseLife;

            vehicleDamage.VehicleExplosionAction += VehicleDestruction;

            canvasGrp = grpBlood.transform.parent.GetComponent<CanvasGroup>();
            canvasGrp.alpha = 0;
            for (var i = 0; i < bloodRectList.Count; i++)
                bloodRectList[i].localScale = Vector3.zero;

            bInit = true;
        }

        public void OnDestroy()
        {
            if (bInit)
            {
                vehicleDamage.VehicleLoseLife -= VehicleLoseLife;
                vehicleDamage.VehicleExplosionAction += VehicleDestruction;
            }   
        }

        public void VehicleLoseLife(int whatTypeOfDammage)
        {
            if(!bInProgress)
                StartCoroutine(VehicleLoseLifeRoutine());
        }
       

        IEnumerator VehicleLoseLifeRoutine()
        {
            bInProgress = true;
            float t = 0;

            float currentAlpha = canvasGrp.alpha;

            grpBlood.SetActive(true);

            List<Vector3> tmpBloodRectScaleList = new List<Vector3>();
            for (var i = 0; i < bloodRectList.Count; i++)
                tmpBloodRectScaleList.Add(bloodRectList[i].localScale);

            if (aBloodClip)
                SoundFxManager.instance.Play(aBloodClip, aBloodVolume);

            //-> Display blood Fx Canvas
            while (t < 1)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime / fadeInDuration;

                    for(var i = 0;i< bloodRectList.Count;i++)
                        bloodRectList[i].localScale = Vector3.Lerp(tmpBloodRectScaleList[i], new Vector3(1,1,1), animCurveBloodIn.Evaluate(t));

                    canvasGrp.alpha = Mathf.Lerp(currentAlpha, .7f, t);
                }
                yield return null;
            }

            //-> Delay blood Fx Canvas
            t = 0;

            while (t < 1)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime / delay;
                }
                yield return null;
            }

            //-> Release blood Fx Canvas
            t = 0;

            currentAlpha = canvasGrp.alpha;
            while (t < 1)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime / fadeOutDuration;

                    for (var i = 0; i < bloodRectList.Count; i++)
                        bloodRectList[i].localScale = Vector3.Lerp(new Vector3(1, 1, 1), Vector3.zero, animCurveBloodOut.Evaluate(t));

                    canvasGrp.alpha = Mathf.Lerp(currentAlpha, 0, t);
                }
                yield return null;
            }

            grpBlood.SetActive(false);
            bInProgress = false;
            yield return null;
        }

        public void VehicleDestruction()
        {
            if (gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(VehicleLoseLifeRoutine());
            }
        }

    }
}

