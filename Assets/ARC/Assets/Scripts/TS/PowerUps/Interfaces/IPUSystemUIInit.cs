// Description: IPUSysOnTriggerEnter: Interface used to init power-up UI

namespace TS.Generics
{
    public interface IPUSystemUIInit<T>
    {
        public void InitPowerUpUI(T powerUpsSystem);
    }
}
