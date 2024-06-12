// Description: GameModeCSAssistant. Attached to page Grp_Game_ShowChampionship
// Used to display info about the next race in Championship mode
using UnityEngine;
using UnityEngine.UI;

namespace TS.Generics
{
    public class GameModeCSAssistant : MonoBehaviour
    {
        public Image trackFullSprite;

        public GameObject grpTxt;
        public GameObject prefabTrackTxt;
        public CurrentText txtChampionshipName;

        public bool InitNextTrackPage()
        {
            // Championship
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 2)
            {
                int currentChamp = GameModeChampionship.instance.currentSelection;

                if (!DataRef.instance.championshipModeData.bDisplayChampionshipUsingListOrder)
                    currentChamp = DataRef.instance.championshipModeData.customChampionshipList[currentChamp];

                int trackID = DataRef.instance.championshipModeData.listOfChampionship[currentChamp].listTrackParams[GameModeChampionship.instance.currentTrackInTheList].TrackID;

                Sprite newSprite = DataRef.instance.tracksData.listTrackParams[trackID].fullScreenSprite;

                GameModeGlobal.instance.currentSelectedTrack = DataRef.instance.tracksData.listTrackParams[trackID].sceneName;

                // Set difficulty
                int difficulty = DataRef.instance.championshipModeData.listOfChampionship[currentChamp].listTrackParams[GameModeChampionship.instance.currentTrackInTheList].AI_Difficulty;
                InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentDifficulty = difficulty;

                if (newSprite) trackFullSprite.sprite = newSprite;
                else trackFullSprite.sprite = null;
                InitTexts(currentChamp);
            }
            return true;
        }


        private void InitTexts(int currentChamp)
        {

            int championshipNameID = DataRef.instance.championshipModeData.listOfChampionship[currentChamp].listTexts[0].EntryID;
            int championshipNameFolder = DataRef.instance.championshipModeData.listOfChampionship[currentChamp].listTexts[0].listID ;
            txtChampionshipName.NewTextWithSpecificID(championshipNameID, championshipNameFolder);

            int howManyTrack = DataRef.instance.championshipModeData.listOfChampionship[currentChamp].listTrackParams.Count;

            //-> Clear the section
            RectTransform[] children = grpTxt.GetComponentsInChildren<RectTransform>(true);
            foreach (RectTransform child in children)
                if (child.gameObject != null && child.gameObject != grpTxt.gameObject) Destroy(child.gameObject);

            //-> Instantiate prefab
            for (var i = 0; i < howManyTrack; i++)
            {
                GameObject newObj = Instantiate(prefabTrackTxt, grpTxt.transform.position, Quaternion.identity, grpTxt.transform);

                //-> Highlight selected track
                if (i == GameModeChampionship.instance.currentTrackInTheList)
                    newObj.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().enabled = true;
                else
                    newObj.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().enabled = false;


                //-> Display track Name
                int trackID = DataRef.instance.championshipModeData.listOfChampionship[currentChamp].listTrackParams[i].TrackID;
                TracksData.trackParams trackParams = DataRef.instance.tracksData.listTrackParams[trackID];
                newObj.transform.GetChild(0).GetChild(1).GetComponent<CurrentText>().NewTextWithSpecificID(trackParams.NameIDMultiLanguage, trackParams.selectedListMultiLanguage);
            }
        }
    }
}

