// Description: InitUIDependingGameMode: Init UI depending the number of player (1-2) (splitscreen).
// Attached to References object in the Hierarchy
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class InitUIDependingGameMode : MonoBehaviour
    {
        public static InitUIDependingGameMode instance = null;

        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public int currentModeSelected;
        public int currentTypeToAddToTheList;
        public string newModeName;      // Use for the custom editor

        [System.Serializable]
        public class StepElement
        {
            public int type;
            public int idInTheList;
        }

        [System.Serializable]
        public class RectTransformElement
        {
            public bool b_ShowInEditor;

            public RectTransform rect;
            public bool b_Enabled;

            public bool b_Scale;
            public Vector3 newScale = Vector3.one;

            public bool b_Anchors;
            public Vector2 newAnchorMin = Vector2.one;
            public Vector2 newAnchorMax = Vector2.one;

            public bool b_Pivot;
            public Vector2 newPivot = Vector2.one;

            public string description;
        }

        [System.Serializable]
        public class CameraElement
        {
            public bool b_ShowInEditor;

            public Camera cam;
            public bool b_Enabled;

            public bool b_ViewportRect;
            public float ViewportRectX;
            public float ViewportRectY;
            public float ViewportRectW;
            public float ViewportRectH;

            public string description;

        }

        [System.Serializable]
        public class InitGameMode
        {
            public string name;
            public List<StepElement> steps = new List<StepElement>();

            public List<RectTransformElement> listRectTransform = new List<RectTransformElement>();
            public List<CameraElement> listCamera = new List<CameraElement>();

        }

        public List<InitGameMode> listInitGameMode = new List<InitGameMode>();

        public bool b_InitDone;
        private bool b_InitInProgress;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        void Start()
        {
        }

        private void OnDestroy()
        {

        }

        //-> Init Lap counter
        public bool bInitSpliScreenUI()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(bInitRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;


            return b_InitDone;
            #endregion
        }

        IEnumerator bInitRoutine()
        {
            #region
            b_InitDone = false;
            int whichMode = 1;  // Split screen: Two Players
            if(InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2)
                whichMode = 0;  // One player


            for (var i = 0; i < listInitGameMode[whichMode].steps.Count; i++)
            {
                int type = listInitGameMode[whichMode].steps[i].type;
                int idInTheList = listInitGameMode[whichMode].steps[i].idInTheList;

                switch (type)
                {
                    // RectTransform
                    case 0:
                        //-> Enable or disable the object
                        bool state = listInitGameMode[whichMode].listRectTransform[idInTheList].b_Enabled;
                        listInitGameMode[whichMode].listRectTransform[idInTheList].rect.gameObject.SetActive(state);

                        //-> Change Rectransform Scale
                        if (listInitGameMode[whichMode].listRectTransform[idInTheList].b_Scale)
                            listInitGameMode[whichMode].listRectTransform[idInTheList].rect.localScale =
                                listInitGameMode[whichMode].listRectTransform[idInTheList].newScale;

                        //-> Change Rectransform Anchors
                        if (listInitGameMode[whichMode].listRectTransform[idInTheList].b_Anchors)
                        {
                            listInitGameMode[whichMode].listRectTransform[idInTheList].rect.anchorMin =
                                listInitGameMode[whichMode].listRectTransform[idInTheList].newAnchorMin;
                            listInitGameMode[whichMode].listRectTransform[idInTheList].rect.anchorMax =
                                listInitGameMode[whichMode].listRectTransform[idInTheList].newAnchorMax;
                        }

                        //-> Change Rectransform Pivot
                        if (listInitGameMode[whichMode].listRectTransform[idInTheList].b_Pivot)
                        {
                            listInitGameMode[whichMode].listRectTransform[idInTheList].rect.pivot =
                                listInitGameMode[whichMode].listRectTransform[idInTheList].newPivot;
                        }
                        break;

                    // Camera
                    case 1:

                        //-> Change Cam Enable/Disable
                        if (listInitGameMode[whichMode].listCamera[idInTheList].b_Enabled)
                        {
                            bool camState = listInitGameMode[whichMode].listCamera[idInTheList].b_Enabled;
                            listInitGameMode[whichMode].listCamera[idInTheList].cam.gameObject.SetActive(camState);
                        }

                        //-> Change viewport rect
                        if (listInitGameMode[whichMode].listCamera[idInTheList].b_ViewportRect)
                        {
                            float X = listInitGameMode[whichMode].listCamera[idInTheList].ViewportRectX;
                            float Y = listInitGameMode[whichMode].listCamera[idInTheList].ViewportRectY;
                            float W = listInitGameMode[whichMode].listCamera[idInTheList].ViewportRectW;
                            float H = listInitGameMode[whichMode].listCamera[idInTheList].ViewportRectH;
                            listInitGameMode[whichMode].listCamera[idInTheList].cam.rect = new Rect(X, Y, W, H);
                        }
                        break;
                }
            }



            b_InitDone = true;
            yield return null;
            #endregion
        }

    }

}
