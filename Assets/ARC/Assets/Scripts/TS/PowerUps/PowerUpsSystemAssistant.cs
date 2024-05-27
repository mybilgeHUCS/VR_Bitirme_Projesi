// Desciption: PowerUpsSystemAssistant: Managed Power-up. Attached to the vehicle.
using UnityEngine;
using UnityEngine.UI;

namespace TS.Generics
{
    public class PowerUpsSystemAssistant :
        MonoBehaviour,
        IPowerUpSystemInit<PUInfo>,
        IPUSystemUIInit<PUInfo>,
        IPUSystemDisable<PUInfo>,
        IPUSysUpdateAI<PUInfo>,
        IPUSysUpdateplayer<PUInfo>,
        IPUSysOnTriggerEnter<PUInfo>,
        IPUAllowToChangePU<PUAllowChange>
    {
        //-> 1: Repair
        public PU_Repair puRepair = new PU_Repair();

        //-> 2: Machine Gun
        public PU_MachineGun puMachineGun = new PU_MachineGun();

        //-> 3: Shield
        public PU_Shield puShield = new PU_Shield();

        //-> 4: Booster
        //Not implemented

        //-> 5: Mines
        public PU_Mine puMine = new PU_Mine();

        //-> 6: Missile
        public PU_Missile pu_Missile;


        //----> SECTION: Init All Power-ups
        public void InitPowerUp(PUInfo puInfo)
        {
            // 0: No Power-Up
            if (puInfo.ID == 0)
                NoPowerUpInit(puInfo);

            // 1: Repair
            if (puInfo.ID == 1)
                RepairInit(puInfo);

            // 2: Machine Gun
            if (puInfo.ID == 2)
                MachineGunInit(puInfo);

            // 3: Shield
            if (puInfo.ID == 3)
                ShieldInit(puInfo);

            // 4: Booster (Not implemented)
            if (puInfo.ID == 4)
                BoosterInit(puInfo);

            // 5: Mine
            if (puInfo.ID == 5)
                MineInit(puInfo);

            //6: Missile
            if (puInfo.ID == 6)
                MissileInit(puInfo);
        }

        //-> 0: No Power-Up
        public void NoPowerUpInit(PUInfo puInfo)
        {
            //Debug.Log("No Power-up Init: Done");
        }

        //-> 1: Repair
        public void RepairInit(PUInfo puInfo)
        {
            //Debug.Log("Repair Init: Done");
        }

        //-> 2: Machine Gun
        public void MachineGunInit(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (powerUpsSystem.vehicleAI.enabled) puMachineGun.InitMachineGunPowerUp(true, powerUpsSystem.aSourcePowerUps, powerUpsSystem);
            else puMachineGun.InitMachineGunPowerUp(false, powerUpsSystem.aSourcePowerUps, powerUpsSystem);
            //Debug.Log("Machine Gun Init: Done");
        }

        //-> 3: Shield
        public void ShieldInit(PUInfo puInfo)
        {
            //Debug.Log("Shield Init: Done");
        }

        //-> 4: Booster (Not implemented)
        public void BoosterInit(PUInfo puInfo)
        {
            //Debug.Log("Booster Init: Done");
        }
        //-> 5: Mine
        public void MineInit(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (powerUpsSystem.vehicleAI.enabled) puMine.InitMinePowerUp(this, true, powerUpsSystem);
            else puMine.InitMinePowerUp(this, false, powerUpsSystem);
            //Debug.Log("Mine Init: Done");
        }

        //-> 6: Missile
        public void MissileInit(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (powerUpsSystem.vehicleAI.enabled) pu_Missile.InitMissilePowerUp(true, powerUpsSystem);
            else pu_Missile.InitMissilePowerUp(false, powerUpsSystem);
            //Debug.Log("Missile Init: Done");
        }


        //-----> SECTION: Init Power-up UI
        public void InitPowerUpUI(PUInfo puInfo)
        {
            // 0: UI No Power-Up
            if (puInfo.ID == 0)
                NoPowerUpUIInit(puInfo);

            // 1: UI Repair
            if (puInfo.ID == 1)
                RepairUIInit(puInfo);

            // 2: UI Machine Gun
            if (puInfo.ID == 2)
                MachineGunUIInit(puInfo);

            // 3: UI Shield
            if (puInfo.ID == 3)
                ShieldUIInit(puInfo);

            // 4: UI Booster (Not implemented)
            if (puInfo.ID == 4)
                BoosterUIInit(puInfo);

            // 5: UI Mine
            if (puInfo.ID == 5)
                MineUIInit(puInfo);

            //6: UI Missile
            if (puInfo.ID == 6)
                MissileUIInit(puInfo);
        }

        //-> 0: UI No Power-Up
        public void NoPowerUpUIInit(PUInfo puInfo)
        {
            //Debug.Log("No Power-up Init: Done");
        }

        //-> 1: UI Repair
        public void RepairUIInit(PUInfo puInfo)
        {
            //Debug.Log("UI Repair Init: Done");
        }

        //-> 2: Machine Gun
        public void MachineGunUIInit(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;

            //-> Connect a UI circle for the machine gun
            GameObject canvasInGame = GameObject.FindGameObjectWithTag("CanvasInGame");
            if (canvasInGame)
            {
                MachineGunCircleTag[] machinGunCircle = canvasInGame.GetComponentsInChildren<MachineGunCircleTag>(true);

                foreach (MachineGunCircleTag obj in machinGunCircle)
                {
                    if (obj && obj.PlayerID == powerUpsSystem.vehicleInfo.playerNumber)
                    {
                        puMachineGun.imTarget = obj.GetComponent<Image>();
                    }
                }
            }

            //-> Machine Gun create UI bullets
            CanvasInGameUIRef.instance.listPlayerUIElements[powerUpsSystem.vehicleInfo.playerNumber].listRectTransform[3].parent.gameObject.SetActive(false);

            RectTransform bullet = CanvasInGameUIRef.instance.listPlayerUIElements[powerUpsSystem.vehicleInfo.playerNumber].listRectTransform[3];
            Transform bulletParent = bullet.transform.parent;
            puMachineGun.listUIBullet.Add(bullet);

            for (var i = 1; i < puMachineGun.refHowManyBullet; i++)
            {
                RectTransform newBullet = Instantiate(bullet, bulletParent);
                puMachineGun.listUIBullet.Add(newBullet);
            }
            //Debug.Log("UI Machine Gun Init: Done");
        }

        //-> 3: UI Shield
        public void ShieldUIInit(PUInfo puInfo)
        {
            //Debug.Log("UI Shield Init: Done");
        }

        //-> 4: UI Booster (Not implemented)
        public void BoosterUIInit(PUInfo puInfo)
        {
            //Debug.Log("UI Booster Init: Done");
        }

        //-> 5: UI Mine
        public void MineUIInit(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            //-> Create UI Mine
            CanvasInGameUIRef.instance.listPlayerUIElements[powerUpsSystem.vehicleInfo.playerNumber].listRectTransform[4].parent.gameObject.SetActive(false);

            RectTransform mine = CanvasInGameUIRef.instance.listPlayerUIElements[powerUpsSystem.vehicleInfo.playerNumber].listRectTransform[4];
            Transform mineParent = mine.transform.parent;
            puMine.listUIMine.Add(mine);

            for (var i = 1; i < puMine.howManyMines; i++)
            {
                RectTransform newMine = Instantiate(mine, mineParent);
                puMine.listUIMine.Add(newMine);
            }
            Debug.Log("UI Mine Init: Done");
        }

        //-> 6: UI Missile
        public void MissileUIInit(PUInfo puInfo)
        {
            Debug.Log("UI Missile Init: Done");
        }



        //----> SECTION: Disable All Power-ups
        public void DisablePowerUp(PUInfo puInfo)
        {
            // 0: Disable No Power-Up
            if (puInfo.ID == 0)
                NoPowerUpDisable(puInfo);

            // 1: Disable Repair
            if (puInfo.ID == 1)
                DisableRepair(puInfo);

            // 2: Disable Machine Gun
            if (puInfo.ID == 2)
                DisableMachineGun(puInfo);

            // 3: Disable Shield
            if (puInfo.ID == 3)
                DisableShield(puInfo);

            // 4: Disable Booster (Not implemented)
            if (puInfo.ID == 4)
                DisableBooster(puInfo);

            // 5: Disable Mine
            if (puInfo.ID == 5)
                DisableMine(puInfo);

            //6: Disable Missile
            if (puInfo.ID == 6)
                DisableMissile(puInfo);
        }

        //-> 0: Disable No Power-Up
        public void NoPowerUpDisable(PUInfo puInfo)
        {
            //Debug.Log("No Power-up Init: Done");
        }

        //-> 1: Disable Repair
        public void DisableRepair(PUInfo puInfo)
        {
            //Debug.Log("Disable Repair: Done");
        }

        //-> 2: Disable Machine Gun
        public void DisableMachineGun(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            puMachineGun.DisableMachineGun(powerUpsSystem.vehicleAI.enabled);

            //Debug.Log("Disable Machine Gun: Done");
        }

        //-> 3: Disable Shield
        public void DisableShield(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            // Disabled only when the vehicle is destroyed by an explosion
            if (powerUpsSystem.Grp_EnemyDetector.activeSelf)
                puShield.DisableShieldPowerUp();
            //Debug.Log("Disable Shield: Done");
        }

        //-> 4: Disable Booster (Not implemented)
        public void DisableBooster(PUInfo puInfo)
        {
            //Debug.Log("Disable Booster: Done");
        }

        //-> 5: Disable Mine
        public void DisableMine(PUInfo puInfo)
        {
            //PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            puMine.DisableMinePowerUp();
            //Debug.Log("Disable Mine: Done");
        }

        //-> 6: Disable Missile
        public void DisableMissile(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            pu_Missile.DisableMissilePowerUp(powerUpsSystem.vehicleInfo.playerNumber);
            //Debug.Log("Disable Missile: Done");
        }






        //---->  SECTION UPDATE AI
        public void AIUpdatePowerUp(PUInfo puInfo)
        {
            // 0: UpdateAI No Power-Up
            if (puInfo.ID == 0)
                UpdateAINoPowerUp(puInfo);

            // 1: UpdateAI Repair
            if (puInfo.ID == 1)
                UpdateAIRepair(puInfo);

            // 2: UpdateAI Machine Gun
            if (puInfo.ID == 2)
                UpdateAIMachineGun(puInfo);

            // 3: UpdateAI Shield
            if (puInfo.ID == 3)
                UpdateAIShield(puInfo);

            // 4: UpdateAI Booster (Not implemented)
            if (puInfo.ID == 4)
                UpdateAIBooster(puInfo);

            // 5: UpdateAI Mine
            if (puInfo.ID == 5)
                UpdateAIMine(puInfo);

            //6: UpdateAI Missile
            if (puInfo.ID == 6)
                UpdateAIMissile(puInfo);
        }

        //-> 0: Update No Power-Up
        public void UpdateAINoPowerUp(PUInfo puInfo)
        {
            //Debug.Log("Update AI No Power-Up: Done");
        }

        //-> 1: Update Repair
        public void UpdateAIRepair(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (powerUpsSystem.vehicleDamage.lifePoints <= 2
                ||
                powerUpsSystem.b_IsPowerUpDetected)
            {
                //Debug.Log("Repair AI Start");
                puRepair.RepairVehicle(this.gameObject, powerUpsSystem.aSourcePowerUps);
                powerUpsSystem.NewPowerUp();
            }
            //Debug.Log("Update AI Repair: Done");
        }

        //-> 2: Machine Gun
        public void UpdateAIMachineGun(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (puMachineGun.b_Is_MachineGun_Available)
                StartCoroutine(puMachineGun.MachineGunRoutine(true, this));
            if (puMachineGun.howManyBullet <= 0)
                powerUpsSystem.NewPowerUp();
            //Debug.Log("Update AI Machine Gun: Done");
        }

        //-> 3: UI Shield
        public void UpdateAIShield(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (puShield.b_IsShieldActivated)
            {
                StartCoroutine(puShield.ShieldForceFieldRoutine(powerUpsSystem.aSourcePowerUps, powerUpsSystem.vehicleDamage, puShield.shieldDuration));
                powerUpsSystem.NewPowerUp();
            }
            else
            {
                puShield.CheckAIEnabledShield(powerUpsSystem.aSourcePowerUps, powerUpsSystem.vehicleDamage, powerUpsSystem.b_IsPowerUpDetected);
            }
            //Debug.Log("Update AI Shield: Done");
        }

        //-> 4: UI Booster (Not implemented)
        public void UpdateAIBooster(PUInfo puInfo)
        {
            //Debug.Log("Update AI Booster: Done");
        }

        //-> 5: UI Mine
        public void UpdateAIMine(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (puMine.currentMineNumber == 0 && puMine.b_IsMineActivated)
            {
                powerUpsSystem.NewPowerUp();
            }
            else
            {
                puMine.CheckAIEnabledMine(powerUpsSystem.aSourcePowerUps, powerUpsSystem.b_IsPowerUpDetected, this);
            }
            //Debug.Log("Update AI Mine: Done");
        }

        //-> 6: UI Missile
        public void UpdateAIMissile(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (pu_Missile.b_Is_Missile_Available)
            {
                StartCoroutine(pu_Missile.MissileAIRoutine(powerUpsSystem.aSourcePowerUps, powerUpsSystem.vehicleInfo.playerNumber, this));
            }
            if (pu_Missile.currentMissileNumber <= 0)
                powerUpsSystem.NewPowerUp();
            //Debug.Log("Update AI Missile: Done");
        }


        //-> Update() Player Power up
        public void PlayerUpdatePowerUp(PUInfo puInfo)
        {
            // 0: UpdatePlayer No Power-Up
            if (puInfo.ID == 0)
                UpdatePlayerNoPowerUp(puInfo);

            // 1: UpdatePlayer Repair
            if (puInfo.ID == 1)
                UpdatePlayerRepair(puInfo);

            // 2: UpdatePlayer Machine Gun
            if (puInfo.ID == 2)
                UpdatePlayerMachineGun(puInfo);

            // 3: UpdatePlayer Shield
            if (puInfo.ID == 3)
                UpdatePlayerShield(puInfo);

            // 4: UpdatePlayer Booster (Not implemented)
            if (puInfo.ID == 4)
                UpdatePlayerBooster(puInfo);

            // 5: UpdatePlayer Mine
            if (puInfo.ID == 5)
                UpdatePlayerMine(puInfo);

            //6: UpdatePlayer Missile
            if (puInfo.ID == 6)
                UpdatePlayerMissile(puInfo);
        }

        //---->  SECTION UPDATE Players
        //-> 0: Update Player 
        public void UpdatePlayerNoPowerUp(PUInfo puInfo)
        {
            //Debug.Log("Update Player No Power-up: Done");
        }

        //-> 1: Update Player Repair
        public void UpdatePlayerRepair(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (powerUpsSystem.b_IsKeyPressedDown)
            {
                puRepair.RepairVehicle(this.gameObject, powerUpsSystem.aSourcePowerUps);
                powerUpsSystem.NewPowerUp();
            }
            //Debug.Log("Update Player Repair: Done");
        }

        //-> 2: Update Player Machine Gun
        public void UpdatePlayerMachineGun(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            puMachineGun.MachineGunTarget(puMachineGun.ignoreObjsList);
            if (powerUpsSystem.b_IsKeyPressedDown && !powerUpsSystem.b_IsKeyPressed)
            {
                if (puMachineGun.b_Is_MachineGun_Available)
                    StartCoroutine(puMachineGun.MachineGunRoutine(false, this));

                powerUpsSystem.b_IsKeyPressed = true;
            }
            if (powerUpsSystem.b_IsKeyPressedUp && !powerUpsSystem.b_IsKeyPressed)
            {
                puMachineGun.ResetMachineGun(powerUpsSystem.vehicleAI.enabled);
                powerUpsSystem.b_IsKeyPressed = false;
            }

            if (puMachineGun.howManyBullet == 0)
                powerUpsSystem.NewPowerUp();

            //Debug.Log("Update Player Machine Gun: Done");
        }

        //-> 3: Update Player Shield
        public void UpdatePlayerShield(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (powerUpsSystem.b_IsKeyPressedDown)
            {
                if (!puShield.b_IsShieldActivated)
                    StartCoroutine(puShield.ShieldForceFieldRoutine(powerUpsSystem.aSourcePowerUps, powerUpsSystem.vehicleDamage, puShield.shieldDuration));
                powerUpsSystem.NewPowerUp();
            }
            //Debug.Log("Update Player Shield: Done");
        }

        //-> 4: Update Player Booster (Not implemented)
        public void UpdatePlayerBooster(PUInfo puInfo)
        {
            //Debug.Log("Update Player Booster: Done");
        }

        //-> 5: Update Player Mine
        public void UpdatePlayerMine(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (powerUpsSystem.b_IsKeyPressedDown && !powerUpsSystem.b_IsKeyPressed)
            {
                puMine.InstantiateMinePrefab(powerUpsSystem.aSourcePowerUps, this);
                powerUpsSystem.b_IsKeyPressed = true;
                if (puMine.currentMineNumber == 0)
                {
                    powerUpsSystem.NewPowerUp();
                }
            }
            //Debug.Log("Update Player Mine: Done");
        }

        //-> 6: Update Player Missile
        public void UpdatePlayerMissile(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            pu_Missile.DisplayUIMissileTargets(powerUpsSystem.vehicleInfo.playerNumber);

            if (powerUpsSystem.b_IsKeyPressedDown && !powerUpsSystem.b_IsKeyPressed)
            {
                StartCoroutine(pu_Missile.InstantiateMissilePrefabRoutine(powerUpsSystem.aSourcePowerUps, powerUpsSystem.vehicleInfo.playerNumber, this));
                powerUpsSystem.b_IsKeyPressed = true;
                powerUpsSystem.NewPowerUp();
            }
            //Debug.Log("Update Player Missile: Done");
        }



        //----> SECTION: OnTriggerEnter
        public void OnTriggerEnterPowerUp(PUInfo puInfo)
        {
            // 0: OnTriggerEnter No Power-up
            if (puInfo.ID == 0)
                OnTriggerEnterNoPU(puInfo);

            // 1: OnTriggerEnter Repair
            if (puInfo.ID == 1)
                OnTriggerEnterRepair(puInfo);

            // 2: OnTriggerEnter Machine Gun
            if (puInfo.ID == 2)
                OnTriggerEnterMachineGun(puInfo);

            // 3: OnTriggerEnter Shield
            if (puInfo.ID == 3)
                OnTriggerEnterShield(puInfo);

            // 4: OnTriggerEnter Booster (Not implemented)
            if (puInfo.ID == 4)
                OnTriggerEnterBooster(puInfo);

            // 5: OnTriggerEnter Mine
            if (puInfo.ID == 5)
                OnTriggerEnterMine(puInfo);

            //6: OnTriggerEnter Missile
            if (puInfo.ID == 6)
                OnTriggerEnterMissile(puInfo);
        }

        //-> 0: OnTriggerEnter No Power-Up
        public void OnTriggerEnterNoPU(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            powerUpsSystem.NewPowerUp(0);
            //Debug.Log("Update Player Repair: Done");
        }

        //-> 1: OnTriggerEnter Repair
        public void OnTriggerEnterRepair(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            powerUpsSystem.NewPowerUp(1);
            puRepair.InitRepairPowerUp();
            //Debug.Log("Update Player Repair: Done");
        }

        //-> 2: OnTriggerEnter Player Machine Gun
        public void OnTriggerEnterMachineGun(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            powerUpsSystem.NewPowerUp(2);
            if (powerUpsSystem.vehicleAI.enabled) puMachineGun.InitMachineGunPowerUp(true, powerUpsSystem.aSourcePowerUps, powerUpsSystem);
            else puMachineGun.InitMachineGunPowerUp(false, powerUpsSystem.aSourcePowerUps, powerUpsSystem);
            //Debug.Log("Update Player Machine Gun: Done");
        }

        //-> 3: OnTriggerEnter Player Shield
        public void OnTriggerEnterShield(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            puShield.InitShieldPowerUp();
            powerUpsSystem.NewPowerUp(3);
            //Debug.Log("Update Player Shield: Done");
        }

        //-> 4: OnTriggerEnter Player Booster (Not implemented)
        public void OnTriggerEnterBooster(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            powerUpsSystem.vehicleBooster.B_EnableBoosterPowerUp();
            powerUpsSystem.NewPowerUp();
            //Debug.Log("Update Player Booster: Done");
        }

        //-> 5: OnTriggerEnter Player Mine
        public void OnTriggerEnterMine(PUInfo puInfo)
        {
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (powerUpsSystem.vehicleAI.enabled) puMine.InitMinePowerUp(this, true, powerUpsSystem);
            else puMine.InitMinePowerUp(this, false, powerUpsSystem);
            powerUpsSystem.NewPowerUp(5);
            //Debug.Log("Update Player Mine: Done");
        }

        //-> 6: OnTriggerEnter Player Missile
        public void OnTriggerEnterMissile(PUInfo puInfo)
        {
            //Debug.Log("Update Player Missile: Done");
            PowerUpsSystem powerUpsSystem = puInfo.powerUpsSystem;
            if (powerUpsSystem.vehicleAI.enabled) pu_Missile.InitMissilePowerUp(true,powerUpsSystem);
            else pu_Missile.InitMissilePowerUp(false, powerUpsSystem);
            powerUpsSystem.NewPowerUp(6);
            //
        }



        //----> SECTION: Check If Vehicle Is Allowed To Change Its Power-Up
        public bool AllowToChangePowerUp(PUAllowChange pUAllowChange)
        {
            // 0: ChangeNotAllowed
            if (pUAllowChange.ID == 0)
               return ChangeNotAllowed(pUAllowChange);

            // 1: NoPowerUp
            if (pUAllowChange.ID == 1)
                return NoPowerUp(pUAllowChange);

            // 2: PlayerReloadMachineGun
            if (pUAllowChange.ID == 2)
                return PlayerReloadMachineGun(pUAllowChange);

            // 3: PlayerRepair
            if (pUAllowChange.ID == 3)
                return PlayerRepair(pUAllowChange);

            // 4: AIWithRepair
            if (pUAllowChange.ID == 4)
                return AIWithRepair(pUAllowChange);

            // 5: AIReloadMachineGun
            if (pUAllowChange.ID == 5)
                return AIReloadMachineGun(pUAllowChange);

            //6: AIWithShield
            if (pUAllowChange.ID == 6)
                return AIWithShield(pUAllowChange);

            //7: AIWithMine
            if (pUAllowChange.ID == 7)
                return AIWithMine(pUAllowChange);

            //8: AIWithMissile
            if (pUAllowChange.ID == 8)
                return AIWithMissile(pUAllowChange);

            return true;
        }

        public bool ChangeNotAllowed(PUAllowChange pUAllowChange)
        {
            PowerUpsSystem powerUpsSystem = pUAllowChange.powerUpsSystem;
            if (!powerUpsSystem.bKeepSelectedPowerUp)
                return true;

            return false;
        }

        public bool NoPowerUp(PUAllowChange pUAllowChange)
        {
            PowerUpsSystem powerUpsSystem = pUAllowChange.powerUpsSystem;
            if (powerUpsSystem.bKeepSelectedPowerUp &&
                powerUpsSystem.currentPowerUps == 0)
                return true;

            return false;
        }

        public bool PlayerReloadMachineGun(PUAllowChange pUAllowChange)
        {
            PowerUpsSystem powerUpsSystem = pUAllowChange.powerUpsSystem;
            PowerUpsItems powerUpsItems = pUAllowChange.powerUpsItems;
            if (!powerUpsSystem.vehicleAI.enabled &&
                powerUpsSystem.bKeepSelectedPowerUp &&
                powerUpsSystem.currentPowerUps == 2 &&
                powerUpsItems.PowerType == 2)
                return true;

            return false;
        }

        public bool PlayerRepair(PUAllowChange pUAllowChange)
        {
            PowerUpsSystem powerUpsSystem = pUAllowChange.powerUpsSystem;
            PowerUpsItems powerUpsItems = pUAllowChange.powerUpsItems;
            if (!powerUpsSystem.vehicleAI.enabled &&
               powerUpsSystem.bKeepSelectedPowerUp &&
               powerUpsSystem.currentPowerUps != 1 &&
               powerUpsItems.PowerType == 1)
                return true;
            return false;
        }

        public bool AIWithRepair(PUAllowChange pUAllowChange)
        {
            PowerUpsSystem powerUpsSystem = pUAllowChange.powerUpsSystem;
            PowerUpsItems powerUpsItems = pUAllowChange.powerUpsItems;
            if (powerUpsSystem.vehicleAI.enabled &&
                powerUpsSystem.bKeepSelectedPowerUp &&
                powerUpsSystem.currentPowerUps == 1 &&
                powerUpsItems.PowerType != 1)
                return true;
            return false;
        }

        public bool AIReloadMachineGun(PUAllowChange pUAllowChange)
        {
            PowerUpsSystem powerUpsSystem = pUAllowChange.powerUpsSystem;
            PowerUpsItems powerUpsItems = pUAllowChange.powerUpsItems;
            if (powerUpsSystem.vehicleAI.enabled &&
                powerUpsSystem.bKeepSelectedPowerUp &&
                powerUpsSystem.currentPowerUps == 2 &&
                (powerUpsItems.PowerType == 2
                ||
               powerUpsSystem.vehicleDamage.lifePoints < 5
                ||
                LapCounterAndPosition.instance.posList[powerUpsSystem.vehicleInfo.playerNumber].RacePos == 0 &&
                powerUpsItems.PowerType == 3 || powerUpsItems.PowerType == 5 || powerUpsItems.PowerType == 1))
                return true;

            return false;
        }

        public bool AIWithShield(PUAllowChange pUAllowChange)
        {
            PowerUpsSystem powerUpsSystem = pUAllowChange.powerUpsSystem;
            PowerUpsItems powerUpsItems = pUAllowChange.powerUpsItems;

            if (powerUpsSystem.vehicleAI.enabled &&
                powerUpsSystem.bKeepSelectedPowerUp &&
                powerUpsSystem.currentPowerUps == 3 &&
                (powerUpsItems.PowerType != 3 ||
                powerUpsSystem.vehicleDamage.lifePoints < 5))
                return true;

            return false;
        }

        public bool AIWithMine(PUAllowChange pUAllowChange)
        {
            PowerUpsSystem powerUpsSystem = pUAllowChange.powerUpsSystem;
            PowerUpsItems powerUpsItems = pUAllowChange.powerUpsItems;

            if (powerUpsSystem.vehicleAI.enabled &&
                powerUpsSystem.bKeepSelectedPowerUp &&
                (powerUpsSystem.currentPowerUps == 5 &&
                powerUpsItems.PowerType != 5 ||
                powerUpsSystem.vehicleDamage.lifePoints < 5))
                return true;

            return false;
        }

        public bool AIWithMissile(PUAllowChange pUAllowChange)
        {
            PowerUpsSystem powerUpsSystem = pUAllowChange.powerUpsSystem;
            PowerUpsItems powerUpsItems = pUAllowChange.powerUpsItems;
            if (powerUpsSystem.vehicleAI.enabled &&
               powerUpsSystem.bKeepSelectedPowerUp &&
               (powerUpsSystem.currentPowerUps == 6 &&
               powerUpsItems.PowerType != 6 ||
               powerUpsSystem.vehicleDamage.lifePoints < 5))
                return true;
            return false;
        }





        //----> SECTION: Other
        public void BulletParticles()
        {
            for (var i = 0; i < puMachineGun.listParticles.Count; i++)
            {
                puMachineGun.listParticles[i].Play();
            }
        }

        /*public void InitUIBulletDelegate()
        {
            int playerNumber = GetComponent<VehicleInfo>().playerNumber;
            if (playerNumber < InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer)
            {
                BulletFx[] all = FindObjectsOfType<BulletFx>();

                foreach (BulletFx obj in all)
                {
                    if (obj.ID == playerNumber)
                    {
                        Debug.Log("Init Machine Gunnnnnnnnnnnnnnnnnn");
                        obj.InitBulletFx(puMachineGun);
                    }
                }
            }
        }*/
    }
}


