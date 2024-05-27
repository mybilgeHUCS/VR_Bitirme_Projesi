// Description : CallMethods_Pc.cs : use to call function
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace TS.Generics
{
    [System.Serializable]
    public class CallMethods_Pc
    {

        // --> Call a list of methods 
        public void Call_A_Method(List<EditorMethodsList_Pc.MethodsList> methodsList)
        {
            #region
            for (var i = 0; i < methodsList.Count; i++)
            {

                if (methodsList[i].obj != null && methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).GetParameters().Length > 0)
                {

                    ParameterInfo pInfo = methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).GetParameters().First();


                    if (CheckValueType(pInfo) == 1)
                    {                                                                                           // --> int value																					
                        methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].intValue });     // call method with an integer argument
                    }

                    if (CheckValueType(pInfo) == 2)
                    {                                                                                           // --> GameObject value
                        methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].objValue });     // call method with an GameObject argument
                    }
                    if (CheckValueType(pInfo) == 3)
                    {                                                                                           // --> string value
                        methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].stringValue });  // call method with an string argument
                    }
                    if (CheckValueType(pInfo) == 4)
                    {                                                                                           // --> float value
                        methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].floatValue });   // call method with an float argument
                    }
                    if (CheckValueType(pInfo) == 5)
                    {                                                                                           // --> float value
                        methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].audioValue });   // call method with an AudioCLip argument
                    }

                }
                else if (methodsList[i].obj != null)
                {
                    methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                        methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { });
                }
            }
            #endregion
        }

        // --> Call a list of methods 
        public void Call_A_Specific_Method(List<EditorMethodsList_Pc.MethodsList> methodsList, int value)
        {
            #region
            for (var i = 0; i < methodsList.Count; i++)
            {
                if (i == value)
                {
                    if (methodsList[i].obj != null && methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).GetParameters().Length > 0)
                    {

                        ParameterInfo pInfo = methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).GetParameters().First();


                        if (CheckValueType(pInfo) == 1)
                        {                                                                                           // --> int value                                                                                    
                            methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                                methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].intValue });     // call method with an integer argument
                        }

                        if (CheckValueType(pInfo) == 2)
                        {                                                                                           // --> GameObject value
                            methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                                methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].objValue });     // call method with an GameObject argument
                        }
                        if (CheckValueType(pInfo) == 3)
                        {                                                                                           // --> string value
                            methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                                methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].stringValue });  // call method with an string argument
                        }
                        if (CheckValueType(pInfo) == 4)
                        {                                                                                           // --> float value
                            methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                                methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].floatValue });   // call method with an float argument
                        }
                        if (CheckValueType(pInfo) == 5)
                        {                                                                                           // --> float value
                            methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                                methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { methodsList[i].audioValue });   // call method with an AudioCLip argument
                        }

                    }
                    else if (methodsList[i].obj != null)
                    {
                        methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { });
                    }
                }
            }
            #endregion
        }

        // --> Call a list of methods only boolean
        public bool Call_A_Method_Only_Boolean(List<EditorMethodsList_Pc.MethodsList> methodsList)
        {
            #region
            bool result = true;
            for (var i = 0; i < methodsList.Count; i++)
            {
                if (methodsList[i].obj != null)
                {
                    Type info = methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).ReturnType;

                    if (info == typeof(System.Boolean))
                    {
                        result = (bool)methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { });
                    }
                }
                if (result == false) break;
            }
            return result;
            #endregion
        }

        // --> Call a list of methods only boolean
        public bool Call_One_Bool_Method(List<EditorMethodsList_Pc.MethodsList> methodsList, int value)
        {
            #region
            bool result = true;
            //for (var i = 0; i < methodsList.Count; i++)
            //{
            if (methodsList[value].obj != null)
            {
                if (methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).GetParameters().Length > 0)
                {
                    Type info = methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).ReturnType;

                    if (info == typeof(System.Boolean))
                    {
                        ParameterInfo pInfo = methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).GetParameters().First();
                        if (CheckValueType(pInfo) == 1)
                        {                                                                                           // --> int value
                            result = (bool)methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).Invoke(
                           methodsList[value].obj.GetComponent(methodsList[value].scriptRef.GetType()), new object[] { methodsList[value].intValue });
                        }

                        if (CheckValueType(pInfo) == 2)
                        {                                                                                           // --> GameObject value
                            result = (bool)methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).Invoke(
                                methodsList[value].obj.GetComponent(methodsList[value].scriptRef.GetType()), new object[] { methodsList[value].objValue });     // call method with an GameObject argument
                        }
                        if (CheckValueType(pInfo) == 3)
                        {                                                                                           // --> string value
                            result = (bool)methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).Invoke(
                                methodsList[value].obj.GetComponent(methodsList[value].scriptRef.GetType()), new object[] { methodsList[value].stringValue });  // call method with an string argument
                        }
                        if (CheckValueType(pInfo) == 4)
                        {                                                                                           // --> float value
                            result = (bool)methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).Invoke(
                                methodsList[value].obj.GetComponent(methodsList[value].scriptRef.GetType()), new object[] { methodsList[value].floatValue });   // call method with an float argument
                        }
                        if (CheckValueType(pInfo) == 5)
                        {                                                                                           // --> AudioClip value
                            result = (bool)methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).Invoke(
                                methodsList[value].obj.GetComponent(methodsList[value].scriptRef.GetType()), new object[] { methodsList[value].audioValue });   // call method with an AudioCLip argument
                        }
                    }

                }
                else
                {
                    result = (bool)methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).Invoke(
                        methodsList[value].obj.GetComponent(methodsList[value].scriptRef.GetType()), new object[] { });

                }
            }
            else
            {
                Debug.Log(methodsList[value].methodInfoName + " is not a boolean Method");

            }
            //if (result == false) break;
            //}
            return result;
            #endregion
        }


        // --> Call a list of a method (only boolean) and check if the method return true (no argument)
        public bool Call_BoolMethod_CheckIfReturnTrue(List<EditorMethodsList_Pc.MethodsList> methodsList, int value)
        {
            #region

            bool result = (bool)methodsList[value].scriptRef.GetType().GetMethod(methodsList[value].methodInfoName).Invoke(
            methodsList[value].obj.GetComponent(methodsList[value].scriptRef.GetType()), new object[] { });

            return result;
            #endregion
        }


        // --> Call a list of methods only boolean (LoadInfoPlayer)
        public bool Call_A_Method_LoadInfoPlayer(List<EditorMethodsList_Pc.MethodsList> methodsList, string[] codes)
        {
            #region
            bool result = true;
            for (var i = 0; i < methodsList.Count; i++)
            {
                if (methodsList[i].obj != null)
                {
                    Type info = methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).ReturnType;

                    if (info == typeof(System.Boolean))
                    {
                        result = (bool)methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { codes[i] });
                    }
                }
                if (result == false) break;
            }
            return result;
            #endregion
        }

        // --> Call a list of methods only string
        public string Call_A_Method_Only_String(List<EditorMethodsList_Pc.MethodsList> methodsList, string defaultResult = "")
        {
            #region
            string result = defaultResult;
            for (var i = 0; i < methodsList.Count; i++)
            {
                if (methodsList[i].obj != null && methodsList[i].scriptRef != null)
                {
                    Type info = methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).ReturnType;

                    if (info == typeof(System.String))
                    {
                        result = (string)methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { });
                    }
                }
            }
            return result;
            #endregion
        }

        // --> Call a list of methods only string
        public string Call_One_String_Method(List<EditorMethodsList_Pc.MethodsList> methodsList, int whichMethod, string defaultResult = "")
        {
            #region
            string result = defaultResult;
            //for (var i = 0; i < methodsList.Count; i++)
            //{
            if (methodsList[whichMethod].obj != null && methodsList[whichMethod].scriptRef != null)
            {
                Type info = methodsList[whichMethod].scriptRef.GetType().GetMethod(methodsList[whichMethod].methodInfoName).ReturnType;

                if (info == typeof(System.String))
                {
                    result = (string)methodsList[whichMethod].scriptRef.GetType().GetMethod(methodsList[whichMethod].methodInfoName).Invoke(
                        methodsList[whichMethod].obj.GetComponent(methodsList[whichMethod].scriptRef.GetType()), new object[] { });
                }
            }
            //}
            return result;
            #endregion
        }

        // --> Call a method named ReturnSaveData when game is saving objects data
        public string Call_A_Method_Only_String_SaveData(List<EditorMethodsList_Pc.MethodsList> methodsList, string defaultResult)
        {
            #region
            string result = defaultResult;
            for (var i = 0; i < methodsList.Count; i++)
            {
                if (methodsList[i].obj != null && methodsList[i].scriptRef != null)
                {
                    //Debug.Log(methodsList [i].obj.name);
                    Type info = methodsList[i].scriptRef.GetType().GetMethod("ReturnSaveData").ReturnType;

                    if (info == typeof(System.String))
                    {
                        result = (string)methodsList[i].scriptRef.GetType().GetMethod("ReturnSaveData").Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { });
                    }
                }
            }
            return result;
            #endregion
        }

        // --> Call a method named F_ObjectLoadData when game is saving objects data
        public void Call_A_Method_ObjectLoadData(List<EditorMethodsList_Pc.MethodsList> methodsList, string s_Value)
        {
            #region
            for (var i = 0; i < methodsList.Count; i++)
            {

                if (methodsList[i].obj != null)
                {

                    Type info = methodsList[i].scriptRef.GetType().GetMethod("saveSystemInitGameObject").ReturnType;

                    methodsList[i].scriptRef.GetType().GetMethod("saveSystemInitGameObject").Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { s_Value }); // call method with an string argument

                }
            }
            #endregion
        }


        // --> Call a method named F_ResetPuzzle when game is saving objects data
        public void Call_A_Method_WithSpecificStringArgument(List<EditorMethodsList_Pc.MethodsList> methodsList, string s_Value)
        {
            #region
            for (var i = 0; i < methodsList.Count; i++)
            {

                if (methodsList[i].obj != null)
                {

                    Type info = methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).ReturnType;

                    methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                        methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { s_Value }); // call method with an string argument

                }
            }
            #endregion
        }

        // --> Call a method that return a float
        public float Call_A_Method_Only_Float(List<EditorMethodsList_Pc.MethodsList> methodsList, float defaultResult = 0)
        {
            #region
            float result = 0;
            for (var i = 0; i < methodsList.Count; i++)
            {
                if (methodsList[i].obj != null && methodsList[i].scriptRef != null)
                {
                    Type info = methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).ReturnType;

                    if (info == typeof(float))
                    {
                        result = (float)methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { });
                    }
                }
            }
            return result;
            #endregion
        }

        // --> Call a method that return a float
        public int Call_A_Method_Only_Int(List<EditorMethodsList_Pc.MethodsList> methodsList, float defaultResult = 0)
        {
            #region
            int result = 0;
            for (var i = 0; i < methodsList.Count; i++)
            {
                if (methodsList[i].obj != null && methodsList[i].scriptRef != null)
                {
                    Type info = methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).ReturnType;

                    if (info == typeof(int))
                    {
                        result = (int)methodsList[i].scriptRef.GetType().GetMethod(methodsList[i].methodInfoName).Invoke(
                            methodsList[i].obj.GetComponent(methodsList[i].scriptRef.GetType()), new object[] { });
                    }
                }
            }
            return result;
            #endregion
        }



        // --> Check the argument type of a method
        public int CheckValueType(ParameterInfo param)
        {
            #region
            if (param.ParameterType == typeof(Int32))
                return 1;
            if (param.ParameterType == typeof(GameObject))
                return 2;
            if (param.ParameterType == typeof(String))
                return 3;
            if (param.ParameterType == typeof(float))
                return 4;
            if (param.ParameterType == typeof(AudioClip))
                return 5;
            return 0;
            #endregion
        }
    }
}