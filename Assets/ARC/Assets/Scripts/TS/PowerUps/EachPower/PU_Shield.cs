// Description: PU_Shield: Called by PowerUpsSystemAssisatnt on vehicle.
using System.Collections;
using UnityEngine;

namespace TS.Generics
{
    [System.Serializable]
    public class PU_Shield
    {
        public GameObject Grp_Shield;

        public AudioClip shieldSound;
        public float aVolume = 1;
        public bool b_IsShieldActivated;
        private int lastLifePoints = -1;

        public float shieldDuration = 2;


        public IEnumerator ShieldForceFieldRoutine(AudioSource aSource, VehicleDamage vehicleDamage,float duration)
        {
            vehicleDamage.b_Invincibility = true;
            b_IsShieldActivated = true;
            if(Grp_Shield)Grp_Shield.SetActive(true);

            if (shieldSound && aSource && aSource.gameObject.activeInHierarchy)
            {
                aSource.clip = shieldSound;
                aSource.volume = aVolume;
                aSource.Play();
            }

            float t = 0;

            while(t!= duration)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t = Mathf.MoveTowards(t, duration, Time.deltaTime);
                   // if (!b_IsShieldActivated) break;
                }
                yield return null;
            }

            for(var i = 0;i < 4; i++)
            {
                t = 0;
                if (Grp_Shield) Grp_Shield.SetActive(!Grp_Shield.activeSelf);
                while (t != .25f)
                {
                    if (!PauseManager.instance.Bool_IsGamePaused)
                    {
                        t = Mathf.MoveTowards(t, .25f, Time.deltaTime);
                     //   if (!b_IsShieldActivated) break;
                    }
                    yield return null;
                }
            }
            

            if (Grp_Shield) Grp_Shield.SetActive(false);
            b_IsShieldActivated = false;
            vehicleDamage.b_Invincibility = false;

            //Debug.Log("Shield Done");

            yield return null;
        }

        public void InitShieldPowerUp()
        {
            b_IsShieldActivated = false;
            //Debug.Log("Shield Init");
        }

        public void DisableShieldPowerUp()
        {
            b_IsShieldActivated = false;
        }

        //-> Ai check if Shield must be activated
        public void CheckAIEnabledShield(AudioSource aSource, VehicleDamage vehicleDamage, bool b_IsPowerUpDetected)
        {
            if(lastLifePoints != vehicleDamage.lifePoints && lastLifePoints != -1
                ||
                b_IsPowerUpDetected)
            {
                b_IsShieldActivated = true;
            }

            lastLifePoints = vehicleDamage.lifePoints;
        }
    }
}