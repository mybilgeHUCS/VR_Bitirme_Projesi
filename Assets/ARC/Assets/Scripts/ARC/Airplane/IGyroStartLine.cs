// Description: interface IGyroStartLine<T>: Use to initialize the vehicle rotation on the starting grid.
using UnityEngine;

namespace TS.Generics
{
    public interface IGyroStartLine<T>
    {
        void InitVehicleGyroPosition(Quaternion quaternion);
    }
}
