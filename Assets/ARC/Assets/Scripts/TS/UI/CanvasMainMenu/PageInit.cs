// Description: PageInit: Initialize the page.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics {
    public class PageInit : MonoBehaviour
    {
        public bool SeeInspector;
        public bool helpBox = true;
        public bool moreOptions;

        public List<EditorMethodsList_Pc.MethodsList> methodsList       // Create a list of Custom Methods that could be edit in the Inspector
            = new List<EditorMethodsList_Pc.MethodsList>();

        public CallMethods_Pc callMethods;


        public IEnumerator IPageInit()
        {
            CanvasMainMenuManager.instance.b_WaitUntilPageInit = false;


            // Enabled/Disable gameObject depending the selected platform
            if (GetComponent<ObjectsDependingPlatform>())
            {
                ObjectsDependingPlatform listObj = GetComponent<ObjectsDependingPlatform>();
                int selectPatform = IntroInfo.instance.globalDatas.selectedPlatform;
                bool newObjState = false;
                for (var i = 0;i< listObj.listDesktopMobileObjects.Count; i++)
                {
                    // Desktop | Other
                    if (selectPatform == 0) 
                        newObjState = listObj.listDesktopMobileObjects[i].b_Desktop;
                    // Mobile
                    else
                        newObjState = listObj.listDesktopMobileObjects[i].b_Mobile;

                    listObj.listDesktopMobileObjects[i]._Obj.SetActive(newObjState);

                    if (newObjState)
                        yield return new WaitUntil(() => listObj.listDesktopMobileObjects[i]._Obj.activeSelf);
                    else
                        yield return new WaitUntil(() => !listObj.listDesktopMobileObjects[i]._Obj.activeSelf);
                }

            }


            // Call all the methods contained in methodList. Those methods initialze the menu.
            for (var i = 0; i < methodsList.Count; i++)
            {
                //Debug.Log("step: " + i);
                yield return new WaitUntil(() => callMethods.Call_One_Bool_Method(methodsList, i) == true);
            }

            CanvasMainMenuManager.instance.b_WaitUntilPageInit = true;
            yield return null;
        }
    }
}
