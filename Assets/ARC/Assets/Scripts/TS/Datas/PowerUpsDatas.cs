using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [CreateAssetMenu(fileName = "PowerUpsDatas", menuName = "TS/PowerUpsDatas")]
    public class PowerUpsDatas : ScriptableObject
    {
        [System.Serializable]
        public class PowerUp {
            public string name;
            public Sprite spPowerUp;
            public GameObject powerUpPrefab;
        }

        public List<PowerUp> listPowerUps = new List<PowerUp>();

        public GameObject puRandomPrefab;
        public List<int> randomSeqRef = new List<int> { 1, 2, 3, 5, 6, 7 };
        public List<int> randomSeq = new List<int> { 1, 2, 3, 5, 6, 7 };
    }
}

