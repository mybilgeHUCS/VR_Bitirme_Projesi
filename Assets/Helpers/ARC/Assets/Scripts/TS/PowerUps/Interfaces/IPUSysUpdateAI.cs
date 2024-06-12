// Description: IPUSysUpdateAI: Interface used to manage the behavior of a power-up for AI

namespace TS.Generics
{
    public interface IPUSysUpdateAI<T>
    {
        public void AIUpdatePowerUp(T powerUpsSystem);
    }
}
