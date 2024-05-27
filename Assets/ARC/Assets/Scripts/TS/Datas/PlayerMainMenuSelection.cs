using UnityEngine;

namespace TS.Generics
{
    [CreateAssetMenu(fileName = "PlayerMainMenuSelection", menuName = "TS/PlayerMainMenuSelection")]
    public class PlayerMainMenuSelection : ScriptableObject
    {
        public bool     helpBox;
        public int      HowManyPlayer;
        public int      currentGameMode;
        public int      howManyVehicleInSelectedGameMode;

        public int          currentDifficulty = 0;
    }
}

