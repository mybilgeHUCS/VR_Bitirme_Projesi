// Description: IPowerUpSystemInit. Interface used to init power-ups
namespace TS.Generics
{
    [System.Serializable]
    public class PUInfo
    {
        public PowerUpsSystem powerUpsSystem;
        public int ID;

        public PUInfo(PowerUpsSystem _powerUpsSystem, int _ID)
        {
            powerUpsSystem = _powerUpsSystem;
            ID = _ID;
        }
    }

    public interface IPowerUpSystemInit<T>
    {
        public void InitPowerUp(T powerUpsSystem);
    }
}
