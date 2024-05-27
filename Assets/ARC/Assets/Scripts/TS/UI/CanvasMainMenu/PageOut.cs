//Description: PageOut.cs. To actions when the page is close and go to the previous page
//0: Select previous page | 1: Choose transition | 2: Custom Method

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace TS.Generics {
    public class PageOut : MonoBehaviour
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

        public GameObject   newSelectedButton;

        public bool         isOperationFinished = true;

        private GameObject  rememberDisabledMenu;

        private bool bIsAnimatorInProgress = false;

        [System.Serializable]
        public class sequenceNewEntry
        {
            //0: Select previous page | 1: Choose transition | 2: Custom Method
            public int _whichOperation;
            public GameObject _Obj;
            public int _Value;
            public bool isTransitionAvailable = true;
            [HideInInspector]
            public UnityEvent _MyEvent;
            public bool b_EnableFade;
            public float _FadeValue = .35f;
            public float _Volume = .5f;
        }

        [System.Serializable]
        public class displaynewCanvasSequenceClass
        {
            public string _SeqName = "";
            public List<sequenceNewEntry> _NewEntry = new List<sequenceNewEntry>();
        }

        public List<displaynewCanvasSequenceClass> listdisplaynewCanvasSequence = new List<displaynewCanvasSequenceClass>();

        [System.Serializable]
        public class ObjectAvailable
        {
            public GameObject _Obj;
            public bool b_Desktop;
            public bool b_Mobile;
        }
        public List<ObjectAvailable> listDesktopMobileObjects = new List<ObjectAvailable>();

        private ButtonNavigation btnNavigationOld;

        //-> Call by script BackButtonManager.cs when the player press back input.
        public IEnumerator BackMenu(bool b_Force = false)
        {
            if(!InfoPlayerTS.instance.b_IsPageCustomPartInProcess &&
                InfoPlayerTS.instance.b_IsAvailableToDoSomething &&
                MusicManager.instance.b_IsFading == false ||
                b_Force)              //-> There is a Music fade. Wait until msuic fade is finished
                StartCoroutine(IDisplayNewScreen());
            yield return null;
        }


        //-> Section: Enable the page
        public void enableThisMenu(
            GameObject newObj,
            int whichStep,
            bool b_MonetizationBackToMenu = false,
            bool b_FadeMonetization = false,
            float MonetizationFadeDuration = .35f)
        {
            isOperationFinished = false;

            //-> Init Canvas
            if (rememberDisabledMenu != null)
            {
                CanvasGroup CanvasGrp_01 = newObj.GetComponent<CanvasGroup>();
                CanvasGrp_01.gameObject.SetActive(true);
                CanvasGrp_01.blocksRaycasts = true;
                CanvasGrp_01.alpha = 1;
                CanvasGrp_01.interactable = true;

                StartCoroutine(I_enableThisMenuWithFade(newObj, whichStep, b_MonetizationBackToMenu, b_FadeMonetization, MonetizationFadeDuration));
            }
            else
            {
                CanvasGroup CanvasGrp_01 = newObj.GetComponent<CanvasGroup>();
                CanvasGrp_01.gameObject.SetActive(true);
                CanvasGrp_01.blocksRaycasts = true;
                CanvasGrp_01.alpha = 0;
                CanvasGrp_01.interactable = false;

                StartCoroutine(I_enableThisMenuWithFade(newObj, whichStep, b_MonetizationBackToMenu, b_FadeMonetization, MonetizationFadeDuration));
            }
        }

        IEnumerator I_enableThisMenuWithFade(
            GameObject newObj,
            int whichStep,
            bool b_MonetizationBackToMenu = false,
            bool b_FadeMonetization = false,
            float MonetizationFadeDuration = .35f)
        {
            if (!b_MonetizationBackToMenu && listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[whichStep].b_EnableFade)
            {
                if (rememberDisabledMenu != null)
                {
                    rememberDisabledMenu.transform.parent.SetSiblingIndex(CanvasMainMenuManager.instance.howManyObject - 1);
                    yield return new WaitUntil(() => rememberDisabledMenu.transform.parent.GetSiblingIndex() == CanvasMainMenuManager.instance.howManyObject - 1);
                }
            }
            if (b_MonetizationBackToMenu && b_FadeMonetization)
            {
                if (rememberDisabledMenu != null)
                {
                    rememberDisabledMenu.transform.parent.SetSiblingIndex(CanvasMainMenuManager.instance.howManyObject - 1);
                    yield return new WaitUntil(() => rememberDisabledMenu.transform.parent.GetSiblingIndex() == CanvasMainMenuManager.instance.howManyObject - 1);
                }
            }

            yield return new WaitUntil(() => newObj.activeSelf);
            CanvasMainMenuManager.instance.b_IsSwitchPageInProgress = false;

            if (rememberDisabledMenu != null)
            {
                //-> Enable Fade
                if (!b_MonetizationBackToMenu && listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[whichStep].b_EnableFade)
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
                    if(rememberDisabledMenu)
                        rememberDisabledMenu.SetActive(false);
                }
                //-> Enable Fade b_MonetizationBackToMenu
                else if (b_MonetizationBackToMenu && b_FadeMonetization)
                {
                    CanvasGroup CanvasGrp_02 = rememberDisabledMenu.GetComponent<CanvasGroup>();
                    float t = 0;
                    float duration = MonetizationFadeDuration;

                    while (t != duration)
                    {
                        t = Mathf.MoveTowards(t, duration, Time.deltaTime);
                        // var scaledValue = (rawValue - min) / (max - min);
                        float scaledValue = t / duration;
                        CanvasGrp_02.alpha = 1 - scaledValue;
                        yield return null;
                    }
                    if (rememberDisabledMenu)
                        rememberDisabledMenu.SetActive(false);
                }
                //-> No Fade
                else
                {
                    if (rememberDisabledMenu)
                        rememberDisabledMenu.SetActive(false);
                }
                rememberDisabledMenu = null;
                newObj.transform.parent.SetSiblingIndex(CanvasMainMenuManager.instance.howManyObject - 1);
            }
            else
            {
                //-> Enable Fade
                CanvasGroup CanvasGrp_01 = newObj.GetComponent<CanvasGroup>();
                if (!b_MonetizationBackToMenu && listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[whichStep].b_EnableFade)
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
                }
                //-> Enable Fade b_MonetizationBackToMenu
                else if (b_MonetizationBackToMenu && b_FadeMonetization)
                {
                    float t = 0;
                    float duration = MonetizationFadeDuration;

                    while (t != duration)
                    {
                        t = Mathf.MoveTowards(t, duration, Time.deltaTime);
                        // var scaledValue = (rawValue - min) / (max - min);
                        float scaledValue = t / duration;
                        CanvasGrp_01.alpha = scaledValue;
                        yield return null;
                    }
                    
                }
                //-> No Fade
                else
                {
                   
                }
                CanvasGrp_01.interactable = true;
            }

            if (TransitionManager.instance && bIsAnimatorInProgress) { TransitionManager.instance.UnPauseAnimation(); }

            isOperationFinished = true;
            yield return null;
        }


        //-> Section: disable the page
        public void DisableCurrentMenu()
        {
            GameObject obj = CanvasMainMenuManager.instance.listMenu[CanvasMainMenuManager.instance.currentSelectedPage];

            //Debug.Log(obj.transform.parent.name);
            isOperationFinished = false;
            GameObject newObj = obj;
            //-> Init Canvas
            CanvasGroup CanvasGrp_02 = newObj.GetComponent<CanvasGroup>();
            CanvasGrp_02.blocksRaycasts = false;
            CanvasGrp_02.alpha = 1;
            CanvasGrp_02.interactable = false;

            rememberDisabledMenu = newObj;
            isOperationFinished = true;
        }

       

        //-> Section: Sequence when the menu go back to the previous page
        public IEnumerator IDisplayNewScreen()
        {
            CanvasMainMenuManager.instance.b_IsSwitchPageInProgress = true;
            InfoPlayerTS.instance.b_ProcessToDisplayNewPageInProgress = true;

            //-> Remember the last selected button
            if (TS_EventSystem.instance.eventSystem.currentSelectedGameObject &&
                TS_EventSystem.instance.eventSystem.currentSelectedGameObject.GetComponent<ButtonNavigation>())
            {
                btnNavigationOld = TS_EventSystem.instance.eventSystem.currentSelectedGameObject.GetComponent<ButtonNavigation>();
            }


            if (listdisplaynewCanvasSequence.Count > 0)
            {
                if (listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[0]._whichOperation != 5) // 5: Nothing selected (Monetize Page case)
                    TS_EventSystem.instance.eventSystem.SetSelectedGameObject(null);

                for (var i = 0; i < listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry.Count; i++)
                {
                    int whichOperation = listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[i]._whichOperation;
                    int value = listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[i]._Value;
                    bool b_IsTransitionAvailable = listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[i].isTransitionAvailable;
                    GameObject newObj = listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[i]._Obj;
                    float _Volume = listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[i]._Volume;

                    switch (whichOperation)
                    {

                        case 0://0: Enable a Canvas Group
                            DisableCurrentMenu();
                            yield return new WaitUntil(() => isOperationFinished == true);
                            GameObject _newPage = CanvasMainMenuManager.instance.listMenu[value];

                            if (_newPage != null)
                            {
                                enableThisMenu(_newPage, i);
                                yield return new WaitUntil(() => isOperationFinished == true);

                            }
                            CanvasMainMenuManager.instance.currentSelectedPage = value;

                            //-> Update Monitization Manager (if needed)
                            //CanvasMainMenuManager.instance.comeBackToPage = value;

                            break;
                    }

                    if (whichOperation == 1 && b_IsTransitionAvailable)
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

                    if (whichOperation == 2 && b_IsTransitionAvailable)
                    {
                        InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
                        listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[i]._MyEvent.Invoke();
                        yield return new WaitUntil(() => InfoPlayerTS.instance.b_IsPageCustomPartInProcess == false);
                    }

                    // -> Set New Selected Button
                    if (whichOperation == 3)
                    {
                        //Debug.Log(listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[i]._Obj.name);
                        GameObject newButton = listdisplaynewCanvasSequence[currentListDisplayed]._NewEntry[i]._Obj;


                        if (IntroInfo.instance.globalDatas.returnPageOutSetSelectedButtonAllowed())
                        {
                            TS_EventSystem.instance.eventSystem.SetSelectedGameObject(newButton);
                        }
                          
                        
                    }

                    // -> Play a sound
                    if (whichOperation == 4)
                    {
                        if (value < SfxList.instance.listAudioClip.Count &&
                            SfxList.instance.listAudioClip[value] != null) { }
                            SoundFxManager.instance.Play(SfxList.instance.listAudioClip[value], _Volume);
                    }

                    yield return null;
                }

            }

            //-> Init the last selected button
            if (btnNavigationOld)
                btnNavigationOld.eventInit?.Invoke();

            yield return new WaitUntil(() => InfoPlayerTS.instance.b_IsAvailableToDoSomething == true);
            //Debug.Log("Process Ended");
            InfoPlayerTS.instance.b_ProcessToDisplayNewPageInProgress = false;
            bIsAnimatorInProgress = false;
            yield return null;
        }


        //-> Monetize system back to previous page
        public IEnumerator DisableCurrentPageAndSelectManualyANewPage(int newPageValue,bool B_fade = false,float FadeDuration =.35f,GameObject newButton = null)
        {
            //if (newButton) Debug.Log("0 newSelectedButton: " + newButton.name);
            isOperationFinished = false;
            listdisplaynewCanvasSequence[0]._NewEntry[0].b_EnableFade = B_fade;
            listdisplaynewCanvasSequence[0]._NewEntry[0]._FadeValue = FadeDuration;

            DisableCurrentMenu();
            yield return new WaitUntil(() => isOperationFinished == true);

            GameObject _newPage = CanvasMainMenuManager.instance.listMenu[newPageValue];

            if (_newPage != null)
            {
                enableThisMenu(_newPage, 0);
                yield return new WaitUntil(() => isOperationFinished == true);
            }
            CanvasMainMenuManager.instance.currentSelectedPage = newPageValue;


            //-> Select new button
            if (IntroInfo.instance.globalDatas.returnPageOutSetSelectedButtonAllowed())
            {
                if(newButton) TS_EventSystem.instance.eventSystem.SetSelectedGameObject(newButton);
            }
                    
            
                
            isOperationFinished = true;


            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;


            yield return null;
        }

    }
}
