// Description: IPUSysOnTriggerEnter: Interface used to disable a power-up

namespace TS.Generics
{
    public interface IPUSystemDisable<T>
    {
        public void DisablePowerUp(T powerUpsSystem);
    }
}
