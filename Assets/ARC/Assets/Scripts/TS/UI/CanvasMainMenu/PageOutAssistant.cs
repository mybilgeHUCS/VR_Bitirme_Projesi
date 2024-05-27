// Description: PageOutAssistant: Methods used by PageOut.cs
using UnityEngine;


namespace TS.Generics {
    public class PageOutAssistant : MonoBehaviour
    {
        public bool Test_01()
        {
            Debug.Log("Test 01");

            return true;
        }

        public bool Test_02()
        {
            Debug.Log("Test 02");

            return true;
        }

        //Use in Monetization Page In
        public void CanvasInteractable()
        {
            if (CanvasMainMenuManager.instance.ComeBackFromPageList[0].comeBackToPage == 0) CanvasMainMenuManager.instance.listMenu[5].transform.parent.GetComponent<PageIn>().SetCanvasInteractable(0, false);
            if (CanvasMainMenuManager.instance.ComeBackFromPageList[0].comeBackToPage == 1) CanvasMainMenuManager.instance.listMenu[5].transform.parent.GetComponent<PageIn>().SetCanvasInteractable(1, false);
            if (CanvasMainMenuManager.instance.ComeBackFromPageList[0].comeBackToPage == 2) CanvasMainMenuManager.instance.listMenu[5].transform.parent.GetComponent<PageIn>().SetCanvasInteractable(2, false);
            if (CanvasMainMenuManager.instance.ComeBackFromPageList[0].comeBackToPage == 3) CanvasMainMenuManager.instance.listMenu[5].transform.parent.GetComponent<PageIn>().SetCanvasInteractable(3, false);
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

    }
}
