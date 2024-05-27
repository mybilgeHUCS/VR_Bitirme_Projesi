// Description: IPUSysOnTriggerEnter: Interface used to do something when the vehicle enter into a power-up

namespace TS.Generics
{
    public interface IPUSysOnTriggerEnter<T>
    {
        public void OnTriggerEnterPowerUp(T powerUpsSystem);
    }
}

