// Description: InfoCoins: Access from anywhere info about Coins. How many coins, update/ADd coins.
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace TS.Generics {
    public class InfoCoins : MonoBehaviour
    {
        public static InfoCoins instance = null;

        public int currentPlayerCoins = 1000;

        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            #endregion
        }

        public void Start()
        {

        }

        public void InitCoins(string sData)
        {
            if (sData == "")
            {
                UpdateCoins(0);
            }
            else
            {
                currentPlayerCoins = 0;
                string[] codes = sData.Split('_');
                UpdateCoins(int.Parse(codes[0]));
            }
                
        }

        public void UpdateCoins(int addCoins = 0)
        {
            currentPlayerCoins += addCoins;

            btnCoin[] allBtnCoin = CanvasMainMenuManager.instance.objCanvasMainMenuManager.GetComponentsInChildren<btnCoin>(true);

            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";     // Replace , with blank space
            nfi.NumberGroupSizes = new int[] { 3}; // 1000

            string formatedCoins = currentPlayerCoins.ToString("#,0", nfi);


            foreach (btnCoin child in allBtnCoin)
            {
                child.transform.GetChild(0).transform.GetChild(1).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry(formatedCoins) });
            }
        }


        public void CheckIfTHePlayerCanBuyTheItem(int PageNumber)
        {

            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[PageNumber].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(PageNumber);
            }

        }

    }
}
