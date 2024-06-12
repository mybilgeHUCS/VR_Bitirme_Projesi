using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    [CreateAssetMenu(fileName = "VehicleUIColorsDatas", menuName = "TS/VehicleUIColorsDatas")]
    public class VehicleUIColorsDatas : ScriptableObject
    {
        public bool helpBox;
       public List<Color> listVehicleUIColorsDatas = new List<Color>();
    }
}

