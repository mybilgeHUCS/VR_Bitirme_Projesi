// Description: BulletFx: Display the blood Fx when the player is hit by the machine gun bullets
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class BulletFx : MonoBehaviour
    {
        public int                  ID;
        private VehicleDamage       vehicleDamage;

        public List<RectTransform>  bulletRectList = new List<RectTransform>();
        public List<CanvasGroup>    canvasGroupList = new List<CanvasGroup>();

        public bool bInit = false;

        public void InitBulletFx(VehicleDamage _vehicleDamage)
        {
            vehicleDamage = _vehicleDamage;
            vehicleDamage.VehicleLoseLife += VehicleLoseLife;

            for (var i = 0; i < bulletRectList.Count; i++) {
                bulletRectList[i].transform.parent.gameObject.SetActive(false);
                canvasGroupList.Add(bulletRectList[i].transform.parent.GetComponent<CanvasGroup>());
            }

            bInit = true;
        }

        public void OnDestroy()
        {
            if (bInit)
                vehicleDamage.VehicleLoseLife -= VehicleLoseLife;
        }

        public void VehicleLoseLife(int whatTypeOfDammage)
        {
            // whatTypeOfDammage: Machine Gun = 0 | Missile = 1
            if (whatTypeOfDammage == 0)
            {
                int selectedBullet = SelectedBullet();
                if (selectedBullet != -1)
                    StartCoroutine(VehicleLoseLifeRoutine(selectedBullet));
            }
        }

        IEnumerator VehicleLoseLifeRoutine(int selectedBullet)
        {
            //bInProgress = true;
            selectedBullet %= bulletRectList.Count;

            float t = 0;
            float duration = 1.15f;

            float randScale = UnityEngine.Random.Range(.75f, 1.1f);
            bulletRectList[selectedBullet].transform.parent.localScale = new Vector3(randScale, randScale, 1);
            bulletRectList[selectedBullet].transform.parent.gameObject.SetActive(true);

            canvasGroupList[selectedBullet].alpha = 1;

            //-> Display blood Fx Canvas
            while (t < 1)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime / duration;
                }
                yield return null;
            }

            //-> Fade out Fx Canvas
            t = 0;
            float releaseDuration = .5f;

            float currentAlpha = canvasGroupList[selectedBullet].alpha;

            while (t < 1)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime / releaseDuration;

                    canvasGroupList[selectedBullet].alpha = Mathf.Lerp(currentAlpha, 0, t);
                }
                yield return null;
            }

            bulletRectList[selectedBullet].transform.parent.gameObject.SetActive(false);
            yield return null;
        }

        int SelectedBullet()
        {
            int rand = UnityEngine.Random.Range(0, bulletRectList.Count);

            for (var i = rand; i < bulletRectList.Count + rand; i++)
            {

                if (!bulletRectList[i % bulletRectList.Count].transform.parent.gameObject.activeSelf)
                {
                    return i;
                }
            }

            return -1;
        }

    }
}

