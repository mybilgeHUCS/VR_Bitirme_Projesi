// Description: LapCounterStartLine: Used to Tag objects.
// 0: Represent the start line | 1: Represent the bufferZone Enter | 2: Represent the bufferZone Exit
// Use in association with LapCounterBadge (vehicle) to prevent bug with vehicle and the start line.
// Example: Doesn't allow the vehicle to respawn after the start line if the vehicle is iin his last lap.
using UnityEngine;

namespace TS.Generics
{
    public class LapCounterStartLine : MonoBehaviour
    {
        public int State = 0;       // 0: Represent the start line | 1: Represent the bufferZone Enter | 2: Represent the bufferZone Exit
    }
}
