// Description: interface IVehicleStartLine<T>: Use to initialize the vehicle position when the vehicle is instantiated on the starting grid.
using UnityEngine;

namespace TS.Generics
{
    public interface IVehicleStartLine<T>
    {
        void InitVehiclePosition(Vector3 pos);
        void InitVehicleOffsetPosition(Vector3 pos);
    }
}
