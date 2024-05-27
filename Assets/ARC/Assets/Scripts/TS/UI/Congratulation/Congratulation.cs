// Description: Congratulation. Manage the Congratulation sequence.
using UnityEngine;
using UnityEngine.Events;

namespace TS.Generics
{
    public class Congratulation : MonoBehaviour
    {
        public int          playerID;
        public UnityEvent   CongratulationEvents = new UnityEvent();

        public void CongratulationSeq()
        {
            CongratulationEvents?.Invoke();
        }
    }
}
