// Description: SlideShow. Manage the slideshow system
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace TS.Generics
{
    public class SlideShow : MonoBehaviour
    {
        [HideInInspector]
        public bool                     SeeInspector;
        [HideInInspector]
        public bool                     moreOptions;
        [HideInInspector]
        public bool                     helpBox = true;


        public Transform                Grp_SlideshowContainer;                 // This object caontains all slides

        public List<Transform>          listRef;
        public List<CanvasGroup>        listSquare;
        public List<CanvasGroup>        listSquareRef;
        public List<Vector3>            listSquarePos;

        [System.Serializable]
        public class ObjsInSquare
        {
            public List<GameObject>     objsList;
            public List<CurrentText>    txtsList;
            public List<Image>          imagesList;
        }

        public List<ObjsInSquare>       objsInSquareList = new List<ObjsInSquare>();

        public List<Transform>          listRefTransPos;

        public Transform                slideObj;

        public bool                     bMoveAvailable = true;

        public int                      currentSelection = 0;
        public int                      howManyEntries = 10;
        float                           t = 0;
        public float                    duration = 1;  // the transition duration
        public AnimationCurve           animSpeedCurve;

        public int                      whichSelectedInList = 0;

        private int                     posInList = 0;

        public List<EditorMethodsList_Pc.MethodsList> methodsListHowManyEntries         // Create a list of Custom Methods that could be edit in the Inspector
        = new List<EditorMethodsList_Pc.MethodsList>();
       
        public List<EditorMethodsList_Pc.MethodsList> methodsListGetCurrentSelection    // Create a list of Custom Methods that could be edit in the Inspector
        = new List<EditorMethodsList_Pc.MethodsList>();

        public List<EditorMethodsList_Pc.MethodsList> methodsListSetCurrentSelection    // Create a list of Custom Methods that could be edit in the Inspector
        = new List<EditorMethodsList_Pc.MethodsList>();

        public UnityEvent               initEvents;                                     // Methods called when the slideshow is initialized when a page is opened

        public List<EditorMethodsList_Pc.MethodsList> methodsListNewEntry               // Methods called when a new entry is displayed
        = new List<EditorMethodsList_Pc.MethodsList>();

        public CallMethods_Pc           callMethods;                                    // Access script taht allow to call public function in this script.

        bool                            IsRoutineInProgress = false;                                   

        // Start is called before the first frame update
        public bool Init()
        {
            //-> Clear the slideshow
            whichSelectedInList = 0;
            currentSelection = 0;
            howManyEntries = 10;
          
           
            //-> Reset Slide Positions
            for (var i = 0; i < listSquareRef.Count; i++)
            {
                listSquareRef[i].transform.position = listSquarePos[i];
                listSquareRef[i].gameObject.SetActive(true);
            }

            //-> Reorder Slides position depending the current selected slide
            if (listSquarePos.Count == 0)
            {
                for(var i = 0;i< listSquare.Count;i++)
                {
                    listSquarePos.Add(listSquare[i].transform.position);
                    listSquareRef.Add(listSquare[i]);
                }
            }

            currentSelection = callMethods.Call_A_Method_Only_Int(methodsListGetCurrentSelection);
            howManyEntries = callMethods.Call_A_Method_Only_Int(methodsListHowManyEntries);

            posInList = 0;
            for(var i = 0;i <= currentSelection; i++) {
                if (i > 2 && i < howManyEntries - 2)
                {
                    posInList--;
                    posInList = (listSquare.Count + posInList) % listSquare.Count;
                }
            }

            
            for (var i = 0; i < listSquare.Count; i++)
            {
                int posInListModulo = (listSquare.Count + posInList + i) % listSquare.Count;
                listSquare[posInListModulo] = listSquareRef[i];
            }


            //-> init Slide Button
            foreach (CanvasGroup obj in listSquare)
            {
                if (obj.gameObject.activeInHierarchy)
                {
                    obj.transform.GetChild(1).GetComponent<ButtonNavigation>().eventExit.Invoke();
                }
                //obj.alpha = .5f;
            }

            for (var i = 0; i < listSquare.Count; i++)
            {
                if (i >= howManyEntries)
                    listSquare[i].gameObject.SetActive(false);
                else
                {
                    listSquare[i].gameObject.SetActive(true);
                }
            }


            //-> Move each Object to its position depending the curent selection
            if (howManyEntries > 6)
            {
                if (currentSelection == 0)
                {
                    whichSelectedInList = 0;
                    for (var i = 0; i < listSquare.Count; i++)
                        listSquare[i].transform.position = listRefTransPos[(i + 6) % listRefTransPos.Count].transform.position;
                }
                if (currentSelection == 1)
                {
                    whichSelectedInList = 1;
                    for (var i = 0; i < listSquare.Count; i++)
                        listSquare[i].transform.position = listRefTransPos[(i + 5) % listRefTransPos.Count].transform.position;
                }

                if (currentSelection == 2)
                {
                    whichSelectedInList = 2;
                    for (var i = 0; i < listSquare.Count; i++)
                        listSquare[i].transform.position = listRefTransPos[(i + 4) % listRefTransPos.Count].transform.position;
                }

                if (currentSelection >= 3)
                {
                    whichSelectedInList = 3;
                    for (var i = 0; i < listSquare.Count; i++)
                        listSquare[i].transform.position = listRefTransPos[(i + 3) % listRefTransPos.Count].transform.position;
                }

                if (currentSelection == howManyEntries - 3)
                {
                    whichSelectedInList = listSquare.Count - 3;
                    for (var i = 0; i < listSquare.Count; i++)
                        listSquare[i].transform.position = listRefTransPos[(i + 2) % listRefTransPos.Count].transform.position;
                }
                if (currentSelection == howManyEntries - 2)
                {
                    whichSelectedInList = listSquare.Count - 2;
                    for (var i = 0; i < listSquare.Count; i++)
                        listSquare[i].transform.position = listRefTransPos[(i + 1) % listRefTransPos.Count].transform.position;
                }
                if (currentSelection == howManyEntries - 1)
                {
                    whichSelectedInList = listSquare.Count - 1;
                    for (var i = 0; i < listSquare.Count; i++)
                        listSquare[i].transform.position = listRefTransPos[(i + 0) % listRefTransPos.Count].transform.position;
                }

            }
            else
            {
                whichSelectedInList = currentSelection;
                for (var i = 0; i < listSquare.Count; i++)
                    listSquare[i].transform.position = listRefTransPos[(i + 6 - currentSelection) % listRefTransPos.Count].transform.position;
            }           

            if (listSquare[whichSelectedInList].gameObject.activeInHierarchy)
            {
                listSquare[whichSelectedInList].transform.GetChild(1).GetComponent<ButtonNavigation>().eventEnter.Invoke();
            }

            //-> Call custom methods to init the slideshow depending whet needed to be displayed
            initEvents.Invoke();

            return true;
        }

        //-> Call when the player press next slideshow entry button
        public void MoveRight()
        {
            if (bMoveAvailable && currentSelection < howManyEntries-1 && gameObject.activeInHierarchy)
                StartCoroutine(MoveRightRoutine());
        }

        IEnumerator MoveRightRoutine()
        {
            bMoveAvailable = false;
            currentSelection++;

            //-> Save the value of the new selected entry
            callMethods.Call_A_Method(methodsListSetCurrentSelection);

            foreach (CanvasGroup obj in listSquare)
                if (obj.gameObject.activeSelf) obj.transform.SetParent(slideObj);

            foreach (CanvasGroup obj in listSquare)
                if(obj.gameObject.activeSelf) obj.transform.GetChild(1).GetComponent<ButtonNavigation>().eventExit.Invoke();

            //-> Move Slides from right to left | Update Alpha
            t = 0;
            Vector3 startPos = slideObj.transform.position;
           while (t < 1)
            {
                t += Time.deltaTime / duration;

                slideObj.transform.position = Vector3.Lerp(startPos, listRef[0].position, animSpeedCurve.Evaluate(t));
                yield return null;
            }


            foreach (CanvasGroup obj in listSquare)
            {
                if (obj.gameObject.activeSelf)
                {
                    obj.transform.SetParent(Grp_SlideshowContainer);
                }   
            }

            listSquare[whichSelectedInList+1].transform.GetChild(1).GetComponent<ButtonNavigation>().eventEnter.Invoke();
            slideObj.transform.position = listRef[1].position;          // Center Ref Pos
          
            if (currentSelection > 3 && currentSelection < howManyEntries-3)
            {
                listSquare[0].transform.position = listRef[4].position; // Right Spawn Ref Pos
                listSquare.Insert(listSquare.Count, listSquare[0]);
                listSquare.RemoveAt(0);
            }

            //-> Find the selected slide
            if(howManyEntries > 6)
            {
                if (currentSelection == 0) whichSelectedInList = 0;
                if (currentSelection == 1) whichSelectedInList = 1;
                if (currentSelection == 2) whichSelectedInList = 2;
                if (currentSelection >= 3) whichSelectedInList = 3;

                if (currentSelection == howManyEntries - 3) whichSelectedInList = listSquare.Count - 3;
                if (currentSelection == howManyEntries - 2) whichSelectedInList = listSquare.Count - 2;
                if (currentSelection == howManyEntries - 1) whichSelectedInList = listSquare.Count - 1;
            }
            else
            {
                whichSelectedInList = currentSelection;
            }

            //-> Call custom methods to initialize the next entry
            StartCoroutine(NewEntryRoutine());

            yield return new WaitUntil(() => IsRoutineInProgress == true);
            bMoveAvailable = true;
            yield return null;
        }

        //-> Call when the player press Previous slideshow entry button
        public void MoveLeft()
        {
            if (bMoveAvailable && currentSelection > 0 && gameObject.activeInHierarchy)
                StartCoroutine(MoveLeftRoutine());
        }

        IEnumerator MoveLeftRoutine()
        {
            bMoveAvailable = false;
            if (currentSelection > 3 && currentSelection < howManyEntries - 3)
            {
                listSquare[listSquare.Count - 1].transform.position = listRef[3].position; // Right Spawn Ref Pos
                listSquare.Insert(0, listSquare[listSquare.Count - 1]);
                listSquare.RemoveAt(listSquare.Count - 1);
            }

            currentSelection--;

            //-> Save the value of the new selected entry
            callMethods.Call_A_Method(methodsListSetCurrentSelection);


            //-> Move Slides from left to right | Update Alpha
            foreach (CanvasGroup obj in listSquare)
                if (obj.gameObject.activeSelf) obj.transform.SetParent(slideObj);

            t = 0;
            Vector3 startPos = slideObj.transform.position;

            foreach (CanvasGroup obj in listSquare)
            {
                if (obj.gameObject.activeSelf)
                    obj.transform.GetChild(1).GetComponent<ButtonNavigation>().eventExit.Invoke();
            }           

            while (t < 1)
            {
                t += Time.deltaTime / duration;
                slideObj.transform.position = Vector3.Lerp(startPos,  listRef[2].position, animSpeedCurve.Evaluate(t));     
                yield return null;
            }

            foreach (CanvasGroup obj in listSquare)
            {
                if (obj.gameObject.activeSelf)
                {
                    obj.transform.SetParent(Grp_SlideshowContainer);
                   // obj.alpha = .5f;
                }
            }

            if (whichSelectedInList != 3 || currentSelection == 1 || currentSelection == 2)
            {listSquare[whichSelectedInList - 1].transform.GetChild(1).GetComponent<ButtonNavigation>().eventEnter.Invoke();}
            else
            {listSquare[whichSelectedInList].transform.GetChild(1).GetComponent<ButtonNavigation>().eventEnter.Invoke();}


            slideObj.transform.position = listRef[1].position; // Center Ref Pos

            //-> Find the selected slide
            if (howManyEntries > 6)
            {
                if (currentSelection == 0) whichSelectedInList = 0;
                if (currentSelection == 1) whichSelectedInList = 1;
                if (currentSelection == 2) whichSelectedInList = 2;
                if (currentSelection >= 3) whichSelectedInList = 3;

                if (currentSelection == howManyEntries - 3) whichSelectedInList = listSquare.Count - 3;
                if (currentSelection == howManyEntries - 2) whichSelectedInList = listSquare.Count - 2;
                if (currentSelection == howManyEntries - 1) whichSelectedInList = listSquare.Count - 1;
            }
            else
            {
                whichSelectedInList = currentSelection;
            }

            //-> Call custom methods to initialize the next entry
            StartCoroutine(NewEntryRoutine());

            yield return new WaitUntil(() => IsRoutineInProgress == true);

            bMoveAvailable = true;
            yield return null;
        }

        //-> Call all the methods in the list
        public IEnumerator NewEntryRoutine()
        {
            #region
            IsRoutineInProgress = true;
            for (var i = 0; i < methodsListNewEntry.Count; i++)
            {
                yield return new WaitUntil(() => callMethods.Call_One_Bool_Method(methodsListNewEntry, i) == true);
            }
            IsRoutineInProgress = false;
            yield return null;
            #endregion
        }

        public void InitSelectedSlide()
        {
            listSquare[whichSelectedInList].transform.GetChild(1).GetComponent<ButtonNavigation>().eventEnter.Invoke();
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }
    }

}
