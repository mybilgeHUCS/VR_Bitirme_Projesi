// Description: SceneInitManager: Use to initialize the scene.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TS.Generics;

public class SceneInitManager : MonoBehaviour
{
    public static SceneInitManager instance = null;
    public bool SeeInspector;

    public List<EditorMethodsList_Pc.MethodsList> methodsList       // Create a list of Custom Methods that could be edit in the Inspector
    = new List<EditorMethodsList_Pc.MethodsList>();

    public CallMethods_Pc callMethods;                              // Access script taht allow to call public function in this script.

    public bool b_Auto = true;                                      // If True: Save is auto load when scene starts.
    public bool b_IsInitDone = true;

    public bool b_Test;

    public bool bAllowTransition = false;

    void Awake()
    {
        //-> Check if instance already exists
        if (instance == null)
            instance = this;
    }

    // Use this for initialization
    void Start()
    {
        #region
        if (b_Auto)
            StartCoroutine(CallAllTheMethodsOneByOne());

        #endregion
    }

   
    //-> Call all the methods in the list 
   public  IEnumerator CallAllTheMethodsOneByOne()
    {
        #region
        b_IsInitDone = false;

        for (var i = 0; i < methodsList.Count; i++)
        {
            yield return new WaitUntil(() => callMethods.Call_One_Bool_Method(methodsList, i) == true);        
        }

        b_IsInitDone = true;
        yield return null;
        #endregion
    }


    //Example with While
   public bool Test_01()
    {
        #region
        while (!b_Test)
        {

            return false;
        }

        Debug.Log("End 1");

        return true;
        #endregion
    }

    // Example Bool
    public bool Test_02()
    {
        #region
        Debug.Log("End");

        return true;
        #endregion
    }

    
}