//Desciption: ButtonNavigation.cs. Attached to buttons. Used to "Animate" the buttons
// depending the button state: selected, PointerEnter, Exit, CLick ....
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TS.Generics
{
    public class ButtonNavigation : MonoBehaviour,
                                    IPointerEnterHandler,
                                    IPointerExitHandler,
                                    IPointerClickHandler,
                                    ISelectHandler,
                                    IDeselectHandler,
                                    IPointerDownHandler,
                                    IPointerUpHandler
    {
        [HideInInspector]
        public bool             SeeInspector;
        [HideInInspector]
        public bool             moreOptions;
        [HideInInspector]
        public bool             helpBox = true;

        public bool             b_AutoSelect = true;
        public bool             b_ClicSoundSelect = true;
        public int              clicSoundID = 0;

        public bool             b_OnClicSound = true;
        public int              onClicSoundIDAllowed = 1;
        public int              onClicSoundIDWrong = 4;
        private float           volumeClic = .5f;

        public bool             b_ScaleSelect = true;
        private Vector3         refScale = new Vector3(1,1,1);
        public Vector3          scale = new Vector3(1.1f, 1.1f,1);
        public float            scaleSpeed = 2;
      
        public CanvasGroup      canvasGroup;

        public UnityEvent       eventInit;

        public UnityEvent       eventEnter;
        public UnityEvent       eventExit;

        public UnityEvent       eventPointerDown;
        public UnityEvent       eventPointerUp;

        private ButtonCustom    buttonCustom;

        

        void Start()
        {
            buttonCustom = GetComponent<ButtonCustom>();

            refScale = gameObject.GetComponent<RectTransform>().localScale;
        
            // Find the button parent that contain the component CanvasGroup
            Transform objWithCanvasGroup = gameObject.transform.parent;
            if(gameObject.transform.parent != null)
            {
                while (canvasGroup == null)
                {
                    if (objWithCanvasGroup.GetComponent<CanvasGroup>())
                        {canvasGroup = objWithCanvasGroup.GetComponent<CanvasGroup>();}
                    else
                    {
                        if (objWithCanvasGroup.transform.parent != null)
                        {objWithCanvasGroup = objWithCanvasGroup.transform.parent;}
                        else
                            break;
                    }
                }
            }

            Button btn = GetComponent<Button>();
            if (btn) btn.onClick.AddListener(OnClickTS);


            eventInit.Invoke();
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            // Desktop | Other platform
            if (IntroInfo.instance.globalDatas.returnButtonNavigationAllowed() &&
                Cursor.visible) {
                if (TS_EventSystem.instance.eventSystem.currentSelectedGameObject != this.gameObject)
                {
                    if (b_AutoSelect && InfoPlayerTS.instance.returnCheckState(0))
                    {
                        SetSelected();
                    }
                    
                }
            } 
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // Desktop | Other platform
            if (IntroInfo.instance.globalDatas.returnButtonNavigationAllowed() &&
                    InfoPlayerTS.instance.returnCheckState(0) &&
                Cursor.visible) {
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //OnClickTS();
        }

        // Button is selected.
        public void OnSelect(BaseEventData eventData)
        {
            // Desktop | Other platform
            if (IntroInfo.instance.globalDatas.returnButtonNavigationAllowed()) {
                if (b_ClicSoundSelect &&
                    InfoPlayerTS.instance.returnCheckState(0) &&
                    SceneInitManager.instance.b_IsInitDone)
                    SoundFxManager.instance.Play(SfxList.instance.listAudioClip[clicSoundID], volumeClic);
                //StartCoroutine(Test());
                //TypoSize0();
                if (b_ScaleSelect)
                {
                    StopAllCoroutines();
                    StartCoroutine(ChangeScaleRoutine(scale));
                   
                }

                eventEnter.Invoke();
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            // Desktop | Other platform
            if(IntroInfo.instance.globalDatas.returnButtonNavigationAllowed() &&
                    !InfoPlayerTS.instance.b_ProcessToDisplayNewPageInProgress)
            {
                if (b_ScaleSelect)
                {
                    StopAllCoroutines();
                    StartCoroutine(ChangeScaleRoutine(refScale));
                    
                }
                eventExit.Invoke();
            }
        }

        public void SetSelected()
        {
           TS_EventSystem.instance.eventSystem.SetSelectedGameObject(this.gameObject);
        }
        
        
      
        IEnumerator ChangeScaleRoutine(Vector3 target)
        {
            RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
            while (rectTrans.localScale != target)
            {
                rectTrans.localScale = Vector3.MoveTowards(rectTrans.localScale, target, Time.deltaTime * scaleSpeed);

                yield return null;
            }

            yield return null;
        }

        public void OnPointerDown(PointerEventData pointerEventData)
        {
            if (IntroInfo.instance.globalDatas.returnButtonNavigationAllowed() &&
                Cursor.visible)
            {
                eventPointerDown.Invoke();
            }
        }

        public void OnPointerUp(PointerEventData pointerEventData)
        {
            if (IntroInfo.instance.globalDatas.returnButtonNavigationAllowed() &&
                Cursor.visible)
            {
                eventPointerUp.Invoke();
            }
        }

        public void OnClickTS()
        {
            if (canvasGroup.interactable)
            {
                if (buttonCustom && b_OnClicSound && gameObject.activeInHierarchy)
                {
                    StartCoroutine(buttonCustom.CallAllTheMethodsOneByOne((checkCondition) =>
                    {
                        if (checkCondition)
                            SoundFxManager.instance.Play(SfxList.instance.listAudioClip[onClicSoundIDAllowed], volumeClic);
                        else
                            SoundFxManager.instance.Play(SfxList.instance.listAudioClip[onClicSoundIDWrong], volumeClic);
                    }));
                }
                else if (b_OnClicSound) SoundFxManager.instance.Play(SfxList.instance.listAudioClip[onClicSoundIDAllowed], volumeClic);

            }
        }
    }

}
