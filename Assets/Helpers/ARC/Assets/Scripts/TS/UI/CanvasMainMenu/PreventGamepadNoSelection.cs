//Description: PreventGamepadNoSelection: If no button selected -> Select a new UI button
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TS.Generics;
//using UnityEditor;

namespace TS.Generics
{
    public class PreventGamepadNoSelection : MonoBehaviour
    {
        public static PreventGamepadNoSelection instance = null;

        string HorizontalAxis = "";
        string VerticalAxis = "";


        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;

            else if (instance != this)
                Destroy(gameObject);
        }

        private void Start()
        {
            HorizontalAxis = TS_EventSystem.instance.standaloneInputModule.horizontalAxis;
            VerticalAxis = TS_EventSystem.instance.standaloneInputModule.verticalAxis;
        }

        void Update()
        {
            // only Desktop | Other platforms
            if (IntroInfo.instance && IntroInfo.instance.globalDatas.selectedPlatform == 0)    
                MPreventGamepadNoSelection();
        }


        // If any button are selected in the UI, select automaticaly a button if the player press an Axis
        public void MPreventGamepadNoSelection()
        {
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                if (TS_EventSystem.instance.eventSystem.currentSelectedGameObject == null)
                {
                    if (Mathf.Abs(Input.GetAxisRaw(HorizontalAxis)) >= 0.4f ||
                    Mathf.Abs(Input.GetAxisRaw(VerticalAxis)) >= 0.4f)
                    {
                        GameObject newCurrentButton = findAButton();
                        if (newCurrentButton)TS_EventSystem.instance.eventSystem.SetSelectedGameObject(newCurrentButton);
                    }
                }
            }     
        }


        // return a button that can be selected in the current UI
        GameObject findAButton()
        {
            Button[] availableButton = FindObjectsOfType<Button>();
            for (var i = 0;i < availableButton.Length; i++)
            {
                if (availableButton[i].enabled)
                {
                    //Selection.activeGameObject = availableButton[i].gameObject;
                    return availableButton[i].gameObject;
                }
            }
            
            return null;
        }
    }
}
