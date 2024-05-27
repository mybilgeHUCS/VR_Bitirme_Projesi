using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [CreateAssetMenu(fileName = "ArcadeModeData", menuName = "TS/ArcadeModeData")]
    public class ArcadeModeData : ScriptableObject
    {
        public bool         MoreOptions;
        public bool         HelpBox;

        public bool         bDisplayTrackUsingTrackListOrder = true;
        public List<int>    customTrackList = new List<int>();

        public int          howManyVehicleByRace = 8;
        public bool         winCoinsAfterRace = true;


    }
}

