using UnityEngine;

namespace TS.Generics
{
    public class NewPUExample :
        MonoBehaviour,
        IPowerUpSystemInit<PUInfo>,
        IPUSystemUIInit<PUInfo>,
        IPUSystemDisable<PUInfo>,
        IPUSysUpdateAI<PUInfo>,
        IPUSysUpdateplayer<PUInfo>,
        IPUSysOnTriggerEnter<PUInfo>,
        IPUAllowToChangePU<PUAllowChange>
    {
        // You will find examples for each section in script PowerUPSystemAssistant.cs.
        // (Project tab: Assets -> Scripts -> TS -> PowerUps -> PowerUPSystemAssistant.cs)
        // For example if you want an example for InitPowerUpUI go to script PowerUPSystemAssistant.cs and search the method using the same name.
        // You will find all the default Power-up cases.


        //-----> SECTION: Init the Power-up UI
        public void InitPowerUpUI(PUInfo puInfo)
        {
            Debug.Log("PU Example -> Init all UI Power-up");
        }


        //----> SECTION: Init the Power-ups
        public void InitPowerUp(PUInfo puInfo)
        {
            Debug.Log("PU Example -> Init the Power-up");

            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (powerUpsSystem.vehicleAI.enabled)
            {
                //Player: Init the power-up                                 
            }
            else {
                //AI: Init the power-up
            }
        }

        //----> SECTION: Disable All Power-ups
        public void DisablePowerUp(PUInfo puInfo)
        {
            Debug.Log("PU Example -> Reset the Power-up");
        }

        //---->  SECTION UPDATE AI (What to do when the AI has the Power-up enabled)
        public void AIUpdatePowerUp(PUInfo puInfo)
        {
            Debug.Log("PU Example -> AI: Update selected Power-up");
        }

        //-> Update() Player Power up (What to do when the Player has the Power-up enabled)
        public void PlayerUpdatePowerUp(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (powerUpsSystem.b_IsKeyPressedDown)
            {
                powerUpsSystem.NewPowerUp();                                    // Select Power-up 0 (No power-up)
            }
            Debug.Log("PU Example -> Player: Update selected Power-up");
        }

        //----> SECTION: OnTriggerEnter (What to do when vehicle enter in Power-up trigger)
        public void OnTriggerEnterPowerUp(PUInfo puInfo)
        {
            Debug.Log("PU Example -> OnTriggerEnter Power Up");
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;

            // Select the value corresponding to your power-up
            // in ScriptableObject obj PowerUpsDatas (Contains sprite,prefab info)
            // In the example the power-up ID = 7
            int selectedPowerUp = 7;
            
            if (powerUpsSystem.vehicleAI.enabled)
            {
                powerUpsSystem.NewPowerUp(selectedPowerUp);                                   
            }
            else
            {
                powerUpsSystem.NewPowerUp(selectedPowerUp);
            }

        }

        //----> SECTION: Check If Vehicle Is Allowed To Change Its Power-Up
        public bool AllowToChangePowerUp(PUAllowChange pUAllowChange)
        {
            Debug.Log("PU Example -> Allow To Change Power Up");
            return true;
        }
    }
}

