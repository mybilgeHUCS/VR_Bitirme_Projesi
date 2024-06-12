//Description: GarageTagPivot: Script use as a tag for the garage system.
using UnityEngine;


namespace TS.Generics
{
    public class GarageTagPivot : MonoBehaviour
    {
        public static GarageTagPivot instance = null;              //Static instance allows to be accessed by any other script.
        public Transform backPos;
        public Transform frontPos;
        public int ID = 0;                                          // 0: Garage | 1: Selection P1 | 2: Slection P2
    }
}
