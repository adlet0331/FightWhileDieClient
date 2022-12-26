using System;
using NonDestroyObject.Data;

namespace NonDestroyObject
{
    public class DataManager : Singleton<DataManager>
    {
        public PlayerDataManager PlayerDataManager;
        public StaticDataManager StaticDataManager;
        public ItemManager ItemManager;

        private void Start()
        {
            PlayerDataManager = new PlayerDataManager();
            PlayerDataManager.Start();
        }
    }
}