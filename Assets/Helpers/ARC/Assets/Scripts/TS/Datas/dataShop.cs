using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [CreateAssetMenu(fileName = "dataShop", menuName = "TS/dataShop")]
    public class dataShop : ScriptableObject
    {
        [System.Serializable]
        public class cShopItem
        {
            public string   NameEditor;
            public int      NameIDMultiLanguage = 0;
            public bool     isItemUnlocked = false;
            public int      Price = 100;
        }

        public List<cShopItem> listShopItems = new List<cShopItem>();


        public bool MoreOptions;
        public bool HelpBox;
        public int currentelectedDatas = 0;
    }
}

