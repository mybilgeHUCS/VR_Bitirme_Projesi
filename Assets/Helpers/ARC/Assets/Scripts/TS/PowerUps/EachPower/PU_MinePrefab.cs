// Description: PU_MinePrefab. Attached to the mine prefab. Managed Mine behavior
using System.Collections;
using UnityEngine;

namespace TS.Generics
{
    [System.Serializable]
    public class PU_MinePrefab : MonoBehaviour
    {
        public bool infiniteLifeTime;
        public float lifeTime = 20;
        public GameObject Grp_Mine;
        public Transform Grp_Circle;
        public AnimationCurve animCircleGrow;

        void Start()
        {
            if(!infiniteLifeTime)
                StartCoroutine(MineRoutine());
        }

        public IEnumerator MineRoutine()
        {
            float t = 0;
            float tGrow = 0;
            Grp_Circle.localScale = Vector3.zero;

            while (t!= lifeTime)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t = Mathf.MoveTowards(t, lifeTime, Time.deltaTime);

                    if(tGrow < 1)
                    {
                        tGrow += Time.deltaTime / 1;
                        Grp_Circle.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1, 1, 1), animCircleGrow.Evaluate(t));
                    }
                }
                   
                yield return null;
            }


            for(var i = 0; i < 5; i++)
            {
                t = 0;
                Grp_Mine.SetActive(!Grp_Mine.activeSelf);
                while (t != .3f)
                {
                    if (!PauseManager.instance.Bool_IsGamePaused)
                        t = Mathf.MoveTowards(t, .3f, Time.deltaTime);
                    yield return null;
                }
                yield return null;
            }

            DestroyMine();

            yield return null;
        }

       

        public void DestroyMine()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}