using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;

namespace NonDestroyObject.DataManage
{
    /// <summary>
    /// Unity PlayerPrefs로 관리하는 데이터들을 다루는 매니저 //TODO PlayerManager에 있는 DataManager 전체에 대한 Dependency 말아 올리기 
    /// </summary>
    [Serializable]
    public class PlayerDataManager
    {
        // Not Updated Frequently
        public int Id => id;
        public string UserName => userName;
        public int Stage => stage;
        public int BaseAtk => baseAtk;
        public int Coin => coin;
        public List<int> EnhanceIngredientList => enhanceIngredientList;
        public int LastUpdated => lastUpdated;
        public int TopStage => topStage;
        
        // Need Update to Get
        public int CurrentEnemyHp => (int) (enemyStartHp * Math.Pow(enemyHpMultiplier, stage));
        public int DailyGatchaCount
        {
            get
            {
                if (lastUpdated < GetCurrentTime())
                    dailyGatchaCount = 0;
                lastUpdated = GetCurrentTime();
                return dailyGatchaCount;
            }
        }
        public int GatchaCosts => gatchaStartCoin * (int)Math.Pow(2, DailyGatchaCount);

        public int Atk
        {
            get
            {
                int atk = baseAtk;

                int plusVal = GetOptionValue(EquipmentOption.AtkAddValue);
                float multVal = 1 + (GetOptionValue(EquipmentOption.AtkAddPercent) / 100.0f);

                atk = (int)(multVal * (atk + plusVal));
                
                return atk;
            }
        }

        public int ClearCoin
        {
            get
            {
                int coin = stage;

                int plusVal = GetOptionValue(EquipmentOption.CoinGainAddValue);
                float multVal = 1 + (GetOptionValue(EquipmentOption.CoinGainAddPercent) / 100.0f);

                coin = (int) (multVal * (coin + plusVal));
                
                return coin;
            }
        }
        public List<int> EquipedItemIdList => equipedItemIdList;

        public int GetOptionValue(EquipmentOption option)
        {
            int val = 0;

            foreach (var equippedItemId in equipedItemIdList)
            {
                EquipItemObject equippedItem = DataManager.Instance.itemManager.GetEquipItemObjectWithId(equippedItemId);

                if (equippedItem != null && equippedItem.Option == option)
                    val += equippedItem.optionValue;
            }

            return val;
        }

        [Header("Static Values")]
        [SerializeField] private int enemyStartHp = 50;
        [SerializeField] private float enemyHpMultiplier = 1.2f;
        [SerializeField] private int gatchaStartCoin = 100;

        [Header("Current Status")] 
        [SerializeField] private int id;
        [SerializeField] private string userName;
        [SerializeField] private int stage;
        [SerializeField] private int baseAtk;
        [SerializeField] private int coin;
        [SerializeField] private int dailyGatchaCount;
        [SerializeField] private List<int> enhanceIngredientList;
        
        [Header("Non-Server Dependent Variables, Only handled in Client")]
        [SerializeField] private int lastUpdated;
        [SerializeField] private List<int> equipedItemIdList;
        
        [Header("Update Frequently")]
        [SerializeField] private int topStage;

        public void Start()
        {
            LoadAllPrefs();
            FetchAllStatus(false);
        }
        
        /// <summary>
        /// Must be called in Main Thread
        /// </summary>
        /// <param name="idp"></param>
        /// <param name="userNameParam"></param>
        public void InitUser(int idp, string userNameParam)
        {
            UniTask.SwitchToMainThread();
            ClearAllPrefs(idp, userNameParam);
        }
        
        public void DeleteUser()
        {
            NetworkManager.Instance.DeleteUser(id).Forget();
            ClearAllPrefs();
        }

        public void GatchaIncrement()
        {
            dailyGatchaCount += 1;
            PlayerPrefs.SetInt("DailyGatchaNum", dailyGatchaCount);
        }

        /// <summary>
        /// Try Spending "amount" of coin.
        /// Return True if success, False if failed.
        /// </summary>
        /// <param name="amount"></param>
        public bool SpendCoin(int amount)
        {
            if (coin < amount) 
                return false;

            coin -= amount;
            PlayerPrefs.SetInt("Coin", coin);
            FetchAllStatus(true);
            return true;
        }

        public void UpdateEquipItem(int index, int uid)
        {
            equipedItemIdList[index] = uid;
            UIManager.Instance.UpdateAllUIInGame();
            FetchAllStatus(false);
        }

        public void StageReset()
        {
            stage = ((int)((topStage - 1) / 10.0f) * 10) + 1;
            FetchAllStatus(true);
        }
        
        public void StageCleared()
        {
            stage += 1;
            if (stage > topStage)
                topStage = stage;
            baseAtk += 10 * (int)((100 + GetOptionValue(EquipmentOption.BaseAtkGainAddPercent)) / 100.0f);
            coin += stage;
            FetchAllStatus(false);
        }

        private enum IntPlayerPrefName
        {
            Id,
            TopStage,
            BaseAtk,
            Coin,
            DailyGatchaNum,
            LastUpdated,
            
            EnhanceIngredient1,
            EnhanceIngredient2,
            EnhanceIngredient3,
            EnhanceIngredient4,
            EnhanceIngredient5,
            EnhanceIngredient6,
            EnhanceIngredient7,
            EnhanceIngredient8,
            
            EquipItem1Id,
            EquipItem2Id
        }

        private enum StringPlayerPrefName
        {
            Name
        }

        private void SaveIntPrefs(IntPlayerPrefName name, int val)
        {
            PlayerPrefs.SetInt(name.ToString(), val);
        }

        private void SaveStringPrefs(StringPlayerPrefName name, string val)
        {
            PlayerPrefs.SetString(name.ToString(), val);
        }

        private int LoadIntPrefs(IntPlayerPrefName name)
        {
            return PlayerPrefs.GetInt(name.ToString(), 0);
        }
        
        private string LoadStringPrefs(StringPlayerPrefName name)
        {
            return PlayerPrefs.GetString(name.ToString(), String.Empty);
        }

        /// <summary>
        /// Check Initialize User If No Name Exist!
        /// TODO: Project Manager-> Script Execution Order Dependency! (Resolve?)
        /// UIManager -> NetworkManager -> DataManager
        /// </summary>
        private void LoadAllPrefs()
        {
            userName = LoadStringPrefs(StringPlayerPrefName.Name);
            
            id = LoadIntPrefs(IntPlayerPrefName.Id);
            topStage = LoadIntPrefs(IntPlayerPrefName.TopStage);
            baseAtk = LoadIntPrefs(IntPlayerPrefName.BaseAtk);
            coin = LoadIntPrefs(IntPlayerPrefName.Coin);
            dailyGatchaCount = LoadIntPrefs(IntPlayerPrefName.DailyGatchaNum);
            lastUpdated = LoadIntPrefs(IntPlayerPrefName.LastUpdated);
            
            enhanceIngredientList = new List<int>();
            enhanceIngredientList.Add(0);
            enhanceIngredientList.Add(LoadIntPrefs(IntPlayerPrefName.EnhanceIngredient1));
            enhanceIngredientList.Add(LoadIntPrefs(IntPlayerPrefName.EnhanceIngredient2));
            enhanceIngredientList.Add(LoadIntPrefs(IntPlayerPrefName.EnhanceIngredient3));
            enhanceIngredientList.Add(LoadIntPrefs(IntPlayerPrefName.EnhanceIngredient4));
            enhanceIngredientList.Add(LoadIntPrefs(IntPlayerPrefName.EnhanceIngredient5));
            enhanceIngredientList.Add(LoadIntPrefs(IntPlayerPrefName.EnhanceIngredient6));
            enhanceIngredientList.Add(LoadIntPrefs(IntPlayerPrefName.EnhanceIngredient7));
            enhanceIngredientList.Add(LoadIntPrefs(IntPlayerPrefName.EnhanceIngredient8));

            equipedItemIdList = new List<int>();
            equipedItemIdList.Add(LoadIntPrefs(IntPlayerPrefName.EquipItem1Id));
            equipedItemIdList.Add(LoadIntPrefs(IntPlayerPrefName.EquipItem2Id));

            if (userName == string.Empty)
            {
                UIManager.Instance.enterYourNamePopup.Open();
            }
        }
        
        private void SaveAllInfoPrefs()
        {
            SaveStringPrefs(StringPlayerPrefName.Name, userName);
            
            SaveIntPrefs(IntPlayerPrefName.Id, id);
            SaveIntPrefs(IntPlayerPrefName.TopStage, topStage);
            SaveIntPrefs(IntPlayerPrefName.BaseAtk, baseAtk);
            SaveIntPrefs(IntPlayerPrefName.Coin, coin);
            SaveIntPrefs(IntPlayerPrefName.DailyGatchaNum, dailyGatchaCount);
            SaveIntPrefs(IntPlayerPrefName.LastUpdated, lastUpdated);
            
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient1, enhanceIngredientList[1]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient2, enhanceIngredientList[2]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient3, enhanceIngredientList[3]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient4, enhanceIngredientList[4]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient5, enhanceIngredientList[5]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient6, enhanceIngredientList[6]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient7, enhanceIngredientList[7]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient8, enhanceIngredientList[8]);
            
            SaveIntPrefs(IntPlayerPrefName.EquipItem1Id, equipedItemIdList[0]);
            SaveIntPrefs(IntPlayerPrefName.EquipItem2Id, equipedItemIdList[1]);
        }

        private void ClearAllPrefs(int pid = 0, string name = "")
        {
            userName = name;
            id = pid;
            topStage = 1;
            // For Debuging
            baseAtk = 50000;
            coin = 100000;
            dailyGatchaCount = 0;
            lastUpdated = GetCurrentTime();

            for (int i = 0; i <= 1; i++)
            {
                equipedItemIdList[i] = -1;
            }

            for (int i = 1; i <= 8; i++)
            {
                enhanceIngredientList[i] = 0;
            }

            FetchAllStatus(true);
        }
        
        private int GetCurrentTime()
        {
            var currentDate = DateTime.Today;
            var date2Int = int.Parse(currentDate.ToString("yyyyMMdd"));
            return date2Int;
        }

        private void FetchAllStatus(bool sendToServer)
        {
            UIManager.Instance.UpdateAllUIInGame();
            SaveAllInfoPrefs();
            if (sendToServer)
                NetworkManager.Instance.FetchUser().Forget();
        }
    }
}
