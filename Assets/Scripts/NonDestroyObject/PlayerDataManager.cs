using Managers;

namespace NonDestroyObject
{
    public interface ITrait
    {
        public abstract void SetTrait();
        public abstract void DeSetTrait();
    }
    public abstract class BaseData
    {
        public int ItemTypeId;
        public string ItemNameString;
    }
    public class EquipItem : BaseData, ITrait
    {
        public int BaseAttack;
        public int EnhanceAttack;

        public void SetTrait()
        {
            throw new System.NotImplementedException();
        }

        public void DeSetTrait()
        {
            throw new System.NotImplementedException();
        }
    }
    public class PlayerDataManager : Singleton<PlayerDataManager>
    {
        
    }
}
