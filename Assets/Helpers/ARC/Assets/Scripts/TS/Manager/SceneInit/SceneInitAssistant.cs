// Description: SceneInitAssistant: Attached to SceneInitManager object. Methods called to init the scene.
using System.Collections;
using UnityEngine;
using TS.Generics;
using UnityEngine.Rendering.Universal;


public class SceneInitAssistant : MonoBehaviour
{
    private int                         loadInProgress = 0;

    public bool                         b_InitDone;
    private bool                        b_InitInProgress;

    //public UniversalRenderPipelineAsset urpRenderer;

    public bool A_S101_IntroAlreadyLoaded()
    {
        IntroInfo.instance.introAlreadyLoaded = true;
        return true;
    }


    public bool A_S102_InitGameMode()
    {
        //-> Play the coroutine Once
        if (!b_InitInProgress)
        {
            b_InitInProgress = true;
            b_InitDone = false;
            int selectedGameMode = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode;
            SceneStepsManager.instance.NextStep(selectedGameMode, 0);
            StartCoroutine(InitGameModeRoutine());
        }
        //-> Check if the coroutine is finished
        else if (b_InitDone)
            b_InitInProgress = false;

        return b_InitDone;
    }

    // Wait until the Game Mode initialization ended (script SceneStepManager.cs)
    IEnumerator InitGameModeRoutine()
    {
        #region
        b_InitDone = false;
        yield return new WaitUntil(() => SceneStepsManager.instance.b_IsSceneStepManagerRunning == false);
        Debug.Log("Init Game Mode Done");
        b_InitDone = true;

        yield return null;
        #endregion
    }

    public bool A_S103_InitAllTexts()
    {
        return LanguageManager.instance.Bool_UpdateAllTexts();
    }

    public bool A_S104_DisplayMenuDependingGameMode()
    {
        #region
        //-> Play the coroutine Once
        if (loadInProgress == 0)
        {
            loadInProgress = 1;
            StartCoroutine(DisplayMenuDependingGameModeRoutine());
        }
        //-> Check if the coroutine is finished
        else if (loadInProgress == 2)
            loadInProgress = 3;

        if (loadInProgress == 3)
        {
            loadInProgress = 0;
            return true;
        }
        else
            return false;
        #endregion
    }

    //-> 
    public IEnumerator DisplayMenuDependingGameModeRoutine()
    {
        #region
        Debug.Log("Open Page Starts");
        SceneStepsManager.instance.NextStep(4, 0);  //  Main Menu step sequence
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => InfoPlayerTS.instance.returnCheckState(0) == true);
        loadInProgress = 2;
        yield return null;
        #endregion
    }


    public bool A_S105_InstantiateAnAirplane(GameObject planePrefab)
    {
        #region
        GameObject newPlane = Instantiate(planePrefab);
        newPlane.GetComponent<VehiclePrefabInit>().bInitVehicleInfo(0);
        return true;
        #endregion
    }


    public bool AllowTransition()
    {
        //-> Prevent to play a transition when the scene is launched
        SceneInitManager.instance.bAllowTransition = true;
        return true;
    }

    public bool UpdateCoin()
    {
        InfoCoins.instance.UpdateCoins();
        return true;
    }

    public bool URPShadowDistanceHundred()
    {
        //if(urpRenderer)urpRenderer.shadowDistance = 100;
        return true;
    }

    public bool URPShadowDistanceThousand()
    {
        //if (urpRenderer) urpRenderer.shadowDistance = 1000;
        return true;
    }

    public bool URPShadowDistanceCustom(int value = 1000)
    {
        //if (urpRenderer) urpRenderer.shadowDistance = value;
        return true;
    }
}