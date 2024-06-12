// Description: TutoAssistant: Attached to TutoManager. Methods called by TutoManager.
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class TutoAssistant : MonoBehaviour
    {
        public int              TSInputKeyBooster = 7;
        public int              TSInputKeyPowerUps = 8;

        private bool            bInitPowerUpDone;
        private bool            bInitBooster;

        private int             PowerUpCounter;
        private int             BoosterCounter;

        private TutoTagID       objPowerUpTuto;
        private TutoTagID       objBoosterTuto;

        private PowerUpsSystem  PowerUps;



        private void OnDestroy()
        {
            if (bInitPowerUpDone)
                PowerUps.NewPowerUpSelected -= NewPowerUpSelected;
                
            if (bInitBooster)
                InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[TSInputKeyBooster].OnGetKeyDownReceived -= BoosterKeyDown;
        }

        public void InitPowerUp()
        {
            objPowerUpTuto = FindObjTutoTag(0);
            objPowerUpTuto.transform.GetChild(0).gameObject.SetActive(false);

            if (!PlayerPrefs.HasKey("Tuto_0"))
            {
                PowerUps = VehiclesRef.instance.listVehicles[0].GetComponent<PowerUpsSystem>();
                PowerUps.NewPowerUpSelected += NewPowerUpSelected;

                string sInputName = "<b>" + InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[TSInputKeyPowerUps]._Keycode.ToString() + "</b>";
                objPowerUpTuto.objList[1].GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry(0, 183), new TextEntry(sInputName) });

                bInitPowerUpDone = true;
            }
        }

        public void NewPowerUpSelected(int selectedPowerUp)
        {
            if(selectedPowerUp == 0)
                objPowerUpTuto.transform.GetChild(0).gameObject.SetActive(false);
            else
            {
                if (PowerUpCounter < 4)
                {
                    PowerUpCounter++;
                    objPowerUpTuto.transform.GetChild(0).gameObject.SetActive(true);
                    objPowerUpTuto.objList[0].GetComponent<UITranslation>().RectTranslation();
                }
            }
        }


        public void InitBooster()
        {
            objBoosterTuto = FindObjTutoTag(1);
            objBoosterTuto.transform.GetChild(0).gameObject.SetActive(false);

            if (!PlayerPrefs.HasKey("Tuto_0"))
            {
                InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[TSInputKeyBooster].OnGetKeyDownReceived += BoosterKeyDown;
                Countdown.instance.multiListUnityEvents[0].ActionAfterCountdown += BoosterActionAfterCountdown;

                string sInputName = "<b>" +  InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[TSInputKeyBooster]._Keycode.ToString() + "</b>";
                objBoosterTuto.objList[1].GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry(0, 184), new TextEntry(sInputName) });

                bInitBooster = true;
            }
            
        }

        public void BoosterActionAfterCountdown(int whichCountdown)
        {
            objBoosterTuto.transform.GetChild(0).gameObject.SetActive(true);
            objBoosterTuto.objList[0].GetComponent<UITranslation>().RectTranslation();

        }

        public void BoosterKeyDown()
        {
            BoosterCounter++;
            if (BoosterCounter == 4)
                objBoosterTuto.transform.GetChild(0).gameObject.SetActive(false);
        }


        TutoTagID FindObjTutoTag(int ID)
        {
            TutoTagID[] tutoTagIDs = FindObjectsOfType<TutoTagID>();

            foreach (TutoTagID tutoTag in tutoTagIDs)
                if (tutoTag.ID == ID)
                {
                    return tutoTag;
                }

            return null;
        }

    }
}
