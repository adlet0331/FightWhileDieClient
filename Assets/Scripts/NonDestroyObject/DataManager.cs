using System;
using NonDestroyObject.DataManage;

namespace NonDestroyObject
{
    /// <summary>
    /// 여러 데이터들을 관리하는 매니저
    /// </summary>
    public class DataManager : Singleton<DataManager>
    {
        public PlayerDataManager PlayerDataManager;
        public StaticDataManager StaticDataManager;
        public ItemManager ItemManager;

        public void DeleteUser()
        {
            PlayerDataManager.DeleteUser();
            ItemManager.Clear();
        }
        
        private void Start()
        {
            PlayerDataManager = new PlayerDataManager();
            PlayerDataManager.Start();

            ItemManager = new ItemManager();
            ItemManager.Start();
        }
    }
}