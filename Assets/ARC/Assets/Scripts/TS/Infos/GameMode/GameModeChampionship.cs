// Description: GameModeChampionship: Access from anywhere info about Championship mode
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class GameModeChampionship : MonoBehaviour
    {
        public static GameModeChampionship instance = null;


        public int          currentSelection = 0;
        public List<bool>   listChampionshipState = new List<bool>(); // Remember if a championship has been won.
        public List<int>    listChampionshipPosition = new List<int>(); // Remember best position at the end of the Championship has been won. 


        public int          currentTrackInTheList = 0;

        public List<int>    listScore = new List<int>();               // Remember score during the current championship

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
            if(sData == "")
            {

                //-> Update championship State
                for(var i = 0;i< DataRef.instance.championshipModeData.listOfChampionship.Count; i++)
                {
                    listChampionshipState.Add(DataRef.instance.championshipModeData.listOfChampionship[i].Unlock);
                }

                //-> Update the best player position for each Championship. -1 mean the player never play this championship
                for (var i = 0; i < DataRef.instance.championshipModeData.listOfChampionship.Count; i++)
                {
                    listChampionshipPosition.Add(-1);
                }
                
            }
            else
            {

                string[] codes = sData.Split('_');
                int counter = 0;
                int howManyEntries = int.Parse(codes[counter]);
                counter++;
                //-> Update championship State
                for (var i = 0; i < howManyEntries; i++)
                {
                    listChampionshipState.Add(TrueFalse(codes[counter]));
                    counter++;
                }

                
                howManyEntries = int.Parse(codes[counter]);
                counter++;
                //-> Update The best player position for each Championship.
                for (var i = 0; i < howManyEntries; i++)
                {
                    listChampionshipPosition.Add(int.Parse(codes[counter]));
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

