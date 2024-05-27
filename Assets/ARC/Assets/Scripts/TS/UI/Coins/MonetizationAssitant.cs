// Description: MonetizationAssitant
using UnityEngine;


namespace TS.Generics
{
    public class MonetizationAssitant : MonoBehaviour
    {
        public void BuyCar()
        {
            int selectedCar = 0;
            #region
            InfoCoins.instance.UpdateCoins();

            CanvasMainMenuManager.instance.ComeBackFromPageList[0].selectedButtonWhenBackToPage = TS_EventSystem.instance.eventSystem.currentSelectedGameObject;
            CanvasMainMenuManager.instance.ComeBackFromPageList[0].comeBackToPage = CanvasMainMenuManager.instance.currentSelectedPage;
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                //-> Display page are you sure to by this item
                if (InfoCoins.instance.currentPlayerCoins >= carPrice(selectedCar))
                {
                    int PageDoYouWantToBuyTheItem = MonetizationManager.instance.PageDoYouWantToBuyTheItem;
                    PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[PageDoYouWantToBuyTheItem].transform.parent.GetComponent<PageIn>();
                    currentMenu.DisplayNewPage(PageDoYouWantToBuyTheItem);
                }
                //-> Display Screen Not Enough money
                else
                {
                    int PageActionNotAvailable = MonetizationManager.instance.PageActionNotAvailable;
                    PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[PageActionNotAvailable].transform.parent.GetComponent<PageIn>();
                    currentMenu.DisplayNewPage(PageActionNotAvailable);
                }
            }
            #endregion
        }

        public void UnlockTrack(int trackID)
        {

        }

        public void UnlockChampionship(int championshipID)
        {

        }


        //-> Call by the button named Button_EarnCoin
        //-> Call when button back is pressed and page Grp_Page_Coin_DoYouWantToBuyTheItem is open (call from pageOut.cs)
        public void ExitMonetization()
        {
            #region
            if (MonetizationManager.instance)
            {
                GameObject newSelectedButton = CanvasMainMenuManager.instance.ComeBackFromPageList[0].selectedButtonWhenBackToPage;

                StartCoroutine(CanvasMainMenuManager.instance.listMenu[CanvasMainMenuManager.instance.currentSelectedPage].transform.parent.GetComponent<PageOut>().DisableCurrentPageAndSelectManualyANewPage(
                    CanvasMainMenuManager.instance.ComeBackFromPageList[0].comeBackToPage,
                    true,
                    .35f,
                    newSelectedButton));
            }
            #endregion
        }

        //-> Coin Reward after showing Ads
        public void CoinReward()
        {
           StartCoroutine(MonetizationManager.instance.EarnCoin());
        }

        public void DisplayPageEarnMoney()
        {
            #region
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                int PageEarnCoins = MonetizationManager.instance.PageEarnCoins;
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[PageEarnCoins].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(PageEarnCoins);
            }
            #endregion
        }

        public void DisplayPageShop()
        {
            #region
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                int PageShop = MonetizationManager.instance.PageShop;
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[PageShop].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(PageShop);
            }
            #endregion
        }


        int carPrice(int carID)
        {
            return 1000;
        }
    }

}
