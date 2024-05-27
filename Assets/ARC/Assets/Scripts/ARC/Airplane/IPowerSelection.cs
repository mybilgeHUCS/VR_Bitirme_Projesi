// Description: IPowerSelection<T> . interface used to add rules to choose AI Power-up
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [System.Serializable]
    public class PowerUpsList
    {
        public List<int> powerUpsList;
        public int vehicleNumber;
        public int ID;

        public PowerUpsList(List<int> _powerUpsList, int _vehicleNumber, int _ID)
        {
            powerUpsList = _powerUpsList;
            vehicleNumber = _vehicleNumber;
            ID = _ID;
        }
    }

    
    public interface IPowerSelection<T>
    {
        [SerializeField]
        Transform PowerUpSelectionRules(T powerUpsList);
    }
}
