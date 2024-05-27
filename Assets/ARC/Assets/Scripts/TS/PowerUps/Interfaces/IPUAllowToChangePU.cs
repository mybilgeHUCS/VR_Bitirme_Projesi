// Description: IPUAllowToChangePU: Interface used to check if the vehicle can change its power-up

namespace TS.Generics
{
    [System.Serializable]
    public class PUAllowChange
    {
        public PowerUpsSystem powerUpsSystem;
        public PowerUpsItems powerUpsItems;
        public int ID;

        public PUAllowChange(PowerUpsSystem _powerUpsSystem, PowerUpsItems _powerUpsItems, int _ID)
        {
            powerUpsSystem = _powerUpsSystem;
            powerUpsItems = _powerUpsItems;
            ID = _ID;
        }
    }

    public interface IPUAllowToChangePU<T>
    {
        public bool AllowToChangePowerUp(T powerUpsSystem);
    }
}

