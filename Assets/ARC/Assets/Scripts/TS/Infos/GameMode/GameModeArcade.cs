// Description: GameModeArcade: Access from anywhere info about GameModeArcade.
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class GameModeArcade : MonoBehaviour
    {
        public static GameModeArcade    instance = null;
        public int                      currentSelection = 0;
        public List<bool>               listArcadeTrackState = new List<bool>(); // Remember if a track in Arcade Mode is unlocked.


        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            #endregion
        }

        public void Init(string sData)
        {
            //Debug.Log("ini: ''" + sData + "''");
            if (sData == "")
            {
                //-> Update Arcade State
                for (var i = 0; i < DataRef.instance.tracksData.listTrackParams.Count; i++)
                {
                    listArcadeTrackState.Add(DataRef.instance.tracksData.listTrackParams[i].isUnlockedArcade);
                }
            }
            else
            {
                string[] codes = sData.Split('_');
                int counter = 0;
                //-> Update Arcade State
                for (var i = 0; i < codes.Length; i++)
                {
                    listArcadeTrackState.Add(TrueFalse(codes[i]));
                    counter++;
                }
            }
        }

        bool TrueFalse(string value)
        {
            if (value == "T") return true;
            else return false;
        }
    }
}

