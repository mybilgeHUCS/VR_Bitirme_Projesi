// Description: CanvasLoadingManager: Used to Show and Hide the Loading Canvas.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class CanvasLoadingManager : MonoBehaviour
    {
        public GameObject                   objCanvasLoading;

        public static CanvasLoadingManager  instance = null;

        public bool                         b_CanvasLoadingIsDisplayed = false;

        public List<GameObject>             listInfoGrp = new List<GameObject>();

        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            #endregion
        }

        public IEnumerator DisplayCanvasLoading(int DisplayInfo = -100)
        {
            for (var i = 0; i < listInfoGrp.Count; i++)
                if (listInfoGrp[i].activeSelf) listInfoGrp[i].SetActive(false);

            if(DisplayInfo != -100 && DisplayInfo < listInfoGrp.Count)
                listInfoGrp[DisplayInfo].SetActive(true);


            objCanvasLoading.SetActive(true);

            if(!objCanvasLoading.transform.GetChild(0).gameObject.activeSelf)
                objCanvasLoading.transform.GetChild(0).gameObject.SetActive(true);

            yield return new WaitUntil(() => objCanvasLoading.transform.GetChild(0).gameObject.activeSelf);
            yield return new WaitUntil(() => objCanvasLoading.activeSelf);

            b_CanvasLoadingIsDisplayed = true;

            yield return null;
        }


        public IEnumerator CloseCanvasLoading()
        {
            objCanvasLoading.SetActive(false);

            yield return new WaitUntil(() => !objCanvasLoading.activeSelf);
            
            for (var i = 0; i < listInfoGrp.Count; i++)
                listInfoGrp[i].SetActive(false);

            b_CanvasLoadingIsDisplayed = false;

            //Debug.Log("Disable Loading screen");
            yield return null;
        }
    }

}
