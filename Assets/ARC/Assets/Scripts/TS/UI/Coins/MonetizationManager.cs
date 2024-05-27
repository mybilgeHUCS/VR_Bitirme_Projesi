// Description: MonetizationManager: Not implemented
using System.Collections;
using UnityEngine;

namespace TS.Generics
{
    public class MonetizationManager : MonoBehaviour
    {
        public static MonetizationManager instance = null;

        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public int PageDoYouWantToBuyTheItem = 4;
        public int PageActionNotAvailable = 5;
        public int PageEarnCoins = 6;
        public int PageShop = 7;


        public int CoinReward = 300;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        
        public IEnumerator EarnCoin()
        {
            InfoPlayerTS.instance.b_IsAvailableToDoSomething = false;

            SoundFxManager.instance.Play(SfxList.instance.listAudioClip[1]);    // Coin Sound

            InfoCoins.instance.UpdateCoins(CoinReward);

            InfoPlayerTS.instance.b_IsAvailableToDoSomething = true;

            //-> Use to say that step is finished in Page_In.cs
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;

            yield return null;
        }
    }

}
