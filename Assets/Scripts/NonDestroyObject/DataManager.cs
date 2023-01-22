using System;
using Cysharp.Threading.Tasks;
using NonDestroyObject.DataManage;

namespace NonDestroyObject
{
    /// <summary>
    /// 여러 데이터들을 관리하는 매니저
    /// </summary>
    public class DataManager : Singleton<DataManager>
    {
        public PlayerDataManager playerDataManager;
        public StaticDataManager staticDataManager; 
        public ItemManager itemManager;

        public void DeleteUser()
        {
            playerDataManager.DeleteUser();
            itemManager.Clear();
        }
        
        private void Start()
        {
            playerDataManager = new PlayerDataManager();
            playerDataManager.Start();

            staticDataManager = new StaticDataManager();
            staticDataManager.Start();

            itemManager = new ItemManager();
            itemManager.Start();
        }
    }
}