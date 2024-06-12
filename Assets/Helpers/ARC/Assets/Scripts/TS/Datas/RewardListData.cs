using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [CreateAssetMenu(fileName = "RewardListData", menuName = "TS/RewardListData")]
    public class RewardListData : ScriptableObject
    {
        [System.Serializable]
        public class RewardParams
        {
            public string name;
            public Sprite img;
            public GameObject giftPrefab;

            public RewardParams(string _name, Sprite _img,GameObject _giftPrefab)
            {
                name = _name;
                img = _img;
                giftPrefab = _giftPrefab;
            }
        }

        public List<RewardParams> rewardParamsList = new List<RewardParams>();
        public int sfxID = 1;
    }
}

