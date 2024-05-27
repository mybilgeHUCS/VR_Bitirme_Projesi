//Description: PageIn.cs. To actions when the page is opened
//0: Enable this Page | 1: Disable current page | 2: Choose transition | 3: Custom Method | 4: Init Menu
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TS.Generics;

namespace TS.Generics
{
    public class PageIn : MonoBehaviour
    {
        public bool         SeeInspector;
        public bool         moreOptions;
        public int          toolbarInt = 0;

        public bool         helpBox = true;

        public bool         b_EverythingDoneBeforeLoading;
        public bool         b_IsLoadingFinished = true;

        public int          currentListDisplayed = 0;
        public bool         b_ChangeName = false;

        public string       newListName = "New List";

        private GameObject  rememberDisabledMenu;

        public bool         b_SpecialPage = false;

        private bool        bIsAnimatorInProgress = false;

        [System.Serializable]
        public class sequenceNewEntry
        {
            //0: Enable this Page
            //1: Disable current page
            //2: Choose transition
            //3: Custom Method
            //4: Init Menu
            public int          _whichOperation;
            public GameObject   _Obj;
            public int          _Value;
            public int          _Value2;
            public bool         isTransitionAvailable = true;
            public bool         b_EnableFade;
            public float        _FadeValue = .35f;
            public float        _Volume;
            public bool         b_InteractableState;

            [HideInInspector]
            public UnityEvent _MyEvent;
        }

        [System.Serializable]
        public class displaynewCanvasSequenceClass
        {
            public string _SeqName = "";
            public List<sequenceNewEntry> _NewEntry = new List<sequenceNewEntry>();
        }

        public List<displaynewCanvasSequenceClass> listdisplaynewCanvasSequence = new List<displaynewCanvasSequenceClass>();

        public bool isOperationFinished = true;

        [System.Serializable]
        public class ObjectAvailable
        {
            public GameObject _Obj;
            public bool b_Desktop;
            public bool b_Mobile;
        }

        public void DisplayNewPage(int whichPage)
        {
            //-> There is a Music fade. Wait until msuic fade is finished
            if (MusicManager.instance.b_IsFading == false)
                StartCoroutine(IDisplayNewPage(whichPage, 0));
        }

        public void DisplayNewPage(int whichPage, int whichSequence = 0)
        {
            //-> There is a Music fade. Wait until msuic fade is finished
            if (MusicManager.instance.b_IsFading == false)
                StartCoroutine(IDisplayNewPage(whichPage,whichSequence));
        }

        public IEnumerator IDisplayNewPage(int whichPage, int whichSequence)
        {
            InfoPlayerTS.instance.b_ProcessToDisplayNewPageInProgress = true;
     
            CanvasMainMenuManager.instance.b_IsSwitchPageInProgress = true;
         
            TS_EventSystem.instance.eventSystem.SetSelectedGameObject(null);

            for (var i = 0; i < listdisplaynewCanvasSequence[whichSequence]._NewEntry.Count; i++)
            {
                int whichOperation              = listdisplaynewCanvasSequence[whichSequence]._NewEntry[i]._whichOperation;
                int value                       = listdisplaynewCanvasSequence[whichSequence]._NewEntry[i]._Value;
                int value2                      = listdisplaynewCanvasSequence[whichSequence]._NewEntry[i]._Value2;
                bool b_IsTransitionAvailable    = listdisplaynewCanvasSequence[whichSequence]._NewEntry[i].isTransitionAvailable;
                float _Volume                   = listdisplaynewCanvasSequence[whichSequence]._NewEntry[i]._Volume;
                bool b_InteractableState        = listdisplaynewCanvasSequence[whichSequence]._NewEntry[i].b_InteractableState;
                float FadeDuration              = listdisplaynewCanvasSequence[whichSequence]._NewEntry[i]._FadeValue;

                switch (whichOperation)
                 {
                    case 10: //Cross fade between two Pages
                        PagesCrossFade(value, value2, FadeDuration);
                        yield return new WaitUntil(() => isOperationFinished == true);
                        break;
                    case 9: //Make a canvas Intaractable to false or true
                        SetCanvasInteractable(value, b_InteractableState);
                        break;

                    case 8://1: Disable Page with its number
                        DisablePageWithItsNumber(value);
                        break;

                    case 1://1: Disable Current Page
                        DisableCurrentMenu();
                         break;
                    case 0://0: Enable this page   
                        enableThisMenu(transform.GetChild(0).gameObject, i);
                        yield return new WaitUntil(() => isOperationFinished == true);
                        break;
                 }

                //-> Transition
                if(whichOperation == 2 && b_IsTransitionAvailable && SceneInitManager.instance.bAllowTransition)
                {
                    int whichTransitionType = TransitionManager.instance.listMultiMethods[value].whichTransitionType;
                    //1: Animation is use for this transition
                    if (whichTransitionType == 1) 
                    {
                        //Debug.Log("Anim");
                        bIsAnimatorInProgress = true;
                        StartCoroutine(TransitionManager.instance.Transition(value, true));
                    }
                    // 0: Scripting is used for this animation
                    else
                    {
                        StartCoroutine(TransitionManager.instance.Transition(value));
                    }
                    
                    yield return new WaitUntil(() => TransitionManager.instance.isTransitionPart1Progress == false);
                }

                // -> custom method
                if (whichOperation == 3 && b_IsTransitionAvailable)
                {
                    InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
                    listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[i]._MyEvent.Invoke();
                    yield return new WaitUntil(() => InfoPlayerTS.instance.b_IsPageCustomPartInProcess == false);
                }

                // -> init menu
                if (whichOperation == 4)
                {
                    StartCoroutine(GetComponent<PageInit>().IPageInit());
                    yield return new WaitUntil(() => CanvasMainMenuManager.instance.b_WaitUntilPageInit == true);
                }

                // -> Set New Selected Button
                if (whichOperation == 5)
                {
                    if (IntroInfo.instance.globalDatas.returnPageInSetSelectedButtonAllowed())
                    {
                        GameObject newButton = listdisplaynewCanvasSequence[whichSequence]._NewEntry[i]._Obj;
                        TS_EventSystem.instance.eventSystem.SetSelectedGameObject(newButton);
                    }
                }

                // -> Play a sound
                if (whichOperation == 6)
                {
                    if(value < SfxList.instance.listAudioClip.Count &&
                        SfxList.instance.listAudioClip[value] != null)
                    SoundFxManager.instance.Play(SfxList.instance.listAudioClip[value], _Volume);
                }

                yield return null;
            }

            //-> Update Monitization Manager (if needed)
            if (!b_SpecialPage && CanvasMainMenuManager.instance.ComeBackFromPageList.Count > 0)
                CanvasMainMenuManager.instance.ComeBackFromPageList[0].comeBackToPage = whichPage;

            CanvasMainMenuManager.instance.currentSelectedPage = whichPage;

            yield return new WaitUntil(() => InfoPlayerTS.instance.b_IsAvailableToDoSomething == true);

            Debug.Log("Process Ended");

            InfoPlayerTS.instance.b_ProcessToDisplayNewPageInProgress = false;
            bIsAnimatorInProgress = false;
            yield return null;
        }

        public void SetCanvasInteractable(int whichCanvas, bool b_State)
        {
            CanvasGroup CanvasGrp = CanvasMainMenuManager.instance.listMenu[whichCanvas].GetComponent<CanvasGroup>();
            CanvasGrp.interactable = b_State;
        }
        
        public void enableThisMenu(GameObject newObj,int whichStep)
        {
            isOperationFinished = false;
            //-> Init Canvas

            if (rememberDisabledMenu != null)
            {
                CanvasGroup CanvasGrp_01 = newObj.GetComponent<CanvasGroup>();
                CanvasGrp_01.gameObject.SetActive(true);
                CanvasGrp_01.blocksRaycasts = true;
                CanvasGrp_01.alpha = 1;

                StartCoroutine(I_enableThisMenuWithFade(newObj, whichStep));
            }
            else
            {
                CanvasGroup CanvasGrp_01 = newObj.GetComponent<CanvasGroup>();
                CanvasGrp_01.gameObject.SetActive(true);
                CanvasGrp_01.blocksRaycasts = true;
                CanvasGrp_01.alpha = 0;

                StartCoroutine(I_enableThisMenuWithFade(newObj, whichStep));
            }
        }

        IEnumerator I_enableThisMenuWithFade(GameObject newObj, int whichStep)
        {
            //-> Enable Fade
            if (listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[whichStep].b_EnableFade)
            {
                if (rememberDisabledMenu != null)
                {
                    rememberDisabledMenu.transform.parent.SetSiblingIndex(CanvasMainMenuManager.instance.howManyObject - 1);
                    yield return new WaitUntil(() => rememberDisabledMenu.transform.parent.GetSiblingIndex() == CanvasMainMenuManager.instance.howManyObject - 1);
                }
            }

            yield return new WaitUntil(() => newObj.activeSelf);
            CanvasGroup CanvasGrp_01 = newObj.GetComponent<CanvasGroup>();

            // The new page is displayed on screen
            CanvasMainMenuManager.instance.b_IsSwitchPageInProgress = false;

            if (rememberDisabledMenu != null)
            {
                //-> Enable Fade
                if (listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[whichStep].b_EnableFade)
                {
                    CanvasGroup CanvasGrp_02 = rememberDisabledMenu.GetComponent<CanvasGroup>();
                    float t = 0;
                    float duration = listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[whichStep]._FadeValue;

                    while (t != duration)
                    {
                        t = Mathf.MoveTowards(t, duration, Time.deltaTime);
                        // var scaledValue = (rawValue - min) / (max - min);
                        float scaledValue = t / duration;
                        CanvasGrp_02.alpha = 1 - scaledValue;
                        yield return null;
                    }
                    rememberDisabledMenu.SetActive(false);
                    Debug.Log("Fade");
                }
                //-> No Fade
                else
                {
                    rememberDisabledMenu.SetActive(false);
                    Debug.Log("No Fade");
                }
                rememberDisabledMenu = null;
                newObj.transform.parent.SetSiblingIndex(CanvasMainMenuManager.instance.howManyObject - 1);
            }
            else
            {
                //-> Enable Fade
                newObj.transform.parent.SetSiblingIndex(CanvasMainMenuManager.instance.howManyObject - 1);
                
                if (listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[whichStep].b_EnableFade)
                {
                    float t = 0;
                    float duration = listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[whichStep]._FadeValue;

                    while (t != duration)
                    {
                        t = Mathf.MoveTowards(t, duration, Time.deltaTime);
                        // var scaledValue = (rawValue - min) / (max - min);
                        float scaledValue = t / duration;
                        CanvasGrp_01.alpha = scaledValue;
                        yield return null;
                    }
                    Debug.Log("Fade Only");
                }
                //-> No Fade
                else
                {
                    Debug.Log("No Fade Only");
                }
               
                
            }
            CanvasGrp_01.interactable = true;

            if (TransitionManager.instance && bIsAnimatorInProgress) { TransitionManager.instance.UnPauseAnimation(); }

            isOperationFinished = true;
            yield return null;
        }

        public void DisableCurrentMenu()
        {
            isOperationFinished = false;
            GameObject newObj = CanvasMainMenuManager.instance.listMenu[CanvasMainMenuManager.instance.currentSelectedPage];
            //-> Init Canvas
            CanvasGroup CanvasGrp_02 = newObj.GetComponent<CanvasGroup>();
            CanvasGrp_02.blocksRaycasts = false;
            CanvasGrp_02.alpha = 1;
            CanvasGrp_02.interactable = false;
            rememberDisabledMenu = newObj;
            isOperationFinished = true;
        }

        public void DisablePageWithItsNumber(int value)
        {
            Debug.Log("DisablePageWithItsNumber");
            isOperationFinished = false;
            GameObject newObj = CanvasMainMenuManager.instance.listMenu[value];
            //-> Init Canvas
            CanvasGroup CanvasGrp_02 = newObj.GetComponent<CanvasGroup>();
            CanvasGrp_02.blocksRaycasts = false;
            CanvasGrp_02.alpha = 1;
            CanvasGrp_02.interactable = false;
            rememberDisabledMenu = newObj;
            isOperationFinished = true;
        }

        public void PagesCrossFade(int Page_01, int Page_02, float FadeDuration)
        {
            isOperationFinished = false;
            StartCoroutine(I_PagesCrossFade(Page_01, Page_02, FadeDuration));
        }

        IEnumerator I_PagesCrossFade(int PageFrom, int PageTo, float FadeDuration)
        {
            Debug.Log("Cross Fade");
            //-> Enable Fade
            GameObject objPage01 = CanvasMainMenuManager.instance.listMenu[PageFrom];
            GameObject objPage02 = CanvasMainMenuManager.instance.listMenu[PageTo];
            objPage02.transform.parent.SetSiblingIndex(CanvasMainMenuManager.instance.howManyObject - 1);
            objPage01.transform.parent.SetSiblingIndex(CanvasMainMenuManager.instance.howManyObject - 1);
            CanvasGroup canvasGrp_01 = objPage01.GetComponent<CanvasGroup>();
            CanvasGroup canvasGrp_02 = objPage02.GetComponent<CanvasGroup>();

            canvasGrp_02.alpha = 0;
            objPage02.SetActive(true);

            yield return new WaitUntil(() => objPage02.activeSelf);

            float t = 0;
            float duration = FadeDuration;

            while (t != duration)
            {
                t = Mathf.MoveTowards(t, duration, Time.deltaTime);
                // var scaledValue = (rawValue - min) / (max - min);
                float scaledValue = t / duration;
                canvasGrp_01.alpha = 1 - scaledValue;
                canvasGrp_02.alpha = scaledValue;
                yield return null;
            }

            canvasGrp_02.interactable = true;
            canvasGrp_02.blocksRaycasts = true;

            objPage01.SetActive(false);
            canvasGrp_01.interactable = false;
            canvasGrp_01.blocksRaycasts = false;
            yield return new WaitUntil(() => !objPage01.activeSelf);

            isOperationFinished = true;
            yield return null;
        }


        //-> Demo method called when the page is initialized
        public void InitMenu()
        {
            Debug.Log("Init Menu");
        }


        public void InitMenu2()
        {
            StartCoroutine(Init());
        }
        public IEnumerator Init()
        {
            isOperationFinished = false;
            Debug.Log("Init Step 1");
            yield return new WaitForSeconds(2);
            Debug.Log("Init Step 2");

            isOperationFinished = true;
            yield return null;
        }


        public void EnableGameObject(GameObject obj)
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            isOperationFinished = false;
            obj.SetActive(true);
            isOperationFinished = true;
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }
        public void DisableGameObject(GameObject obj)
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            isOperationFinished = false;
            obj.SetActive(false);
            isOperationFinished = true;
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

    }
}

