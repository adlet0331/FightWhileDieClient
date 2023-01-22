using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils;

namespace NonDestroyObject.DataManage
{
    /// <summary>
    /// Unity PlayerPrefs로 관리하는 데이터들을 다루는 매니저 //TODO PlayerManager에 있는 DataManager 전체에 대한 Dependency 말아 올리기 
    /// </summary>
    [Serializable]
    public class PlayerDataManager
    {
        public int Id => id;
        public string UserName => userName;
        public int Stage => stage;
        public int BaseAtk => baseAtk;
        public int CurrentEnemyHp => (int) (enemyStartHp * Math.Pow(enemyHpMultiplier, stage));
        public int Coin => coin;
        public int GatchaCosts => gatchaStartCoin * (int)Math.Pow(2, dailyGatchaCount);
        public List<int> EnhanceIngredientList => enhanceIngredientList;
        public int TopStage => topStage;
        public int Atk => atk;
        public int ClearCoin => clearCoin;


        [Header("Static Values")]
        [SerializeField] private int enemyStartHp = 50;
        [SerializeField] private float enemyHpMultiplier = 1.2f;
        [SerializeField] private int gatchaStartCoin = 100;

        [Header("Current Status")] 
        [SerializeField] private int id;
        [SerializeField] private string userName;
        [SerializeField] private int stage = 1;
        [SerializeField] private int baseAtk = 50;
        [SerializeField] private int coin = 10;
        [SerializeField] private int dailyGatchaCount;
        [SerializeField] private List<int> enhanceIngredientList;
        [Header("Update")]
        [SerializeField] private int topStage;
        [SerializeField] private int atk = 50;
        [SerializeField] private int clearCoin;
        
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
            UpdateMainUI();
            return true;
        }
        
        public void StageReset()
        {
            stage = ((int)(topStage / 10.0f) * 10) + 1;
            FetchAllStatus(true);
        }
        
        public void StageCleared()
        {
            stage += 1;
            if (stage > topStage)
                topStage = stage;
            baseAtk += 10;
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
            
            EnhanceIngredient1,
            EnhanceIngredient2,
            EnhanceIngredient3,
            EnhanceIngredient4,
            EnhanceIngredient5,
            EnhanceIngredient6,
            EnhanceIngredient7,
            EnhanceIngredient8,
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
            
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient1, enhanceIngredientList[1]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient2, enhanceIngredientList[2]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient3, enhanceIngredientList[3]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient4, enhanceIngredientList[4]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient5, enhanceIngredientList[5]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient6, enhanceIngredientList[6]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient7, enhanceIngredientList[7]);
            SaveIntPrefs(IntPlayerPrefName.EnhanceIngredient8, enhanceIngredientList[8]);
        }

        private void ClearAllPrefs(int pid = 0, string name = "")
        {
            userName = name;
            id = pid;
            topStage = 1;
            baseAtk = 50;
            coin = 10;
            dailyGatchaCount = 0;

            for (int i = 1; i <= 8; i++)
            {
                enhanceIngredientList[i] = 0;
            }

            FetchAllStatus(true);
        }

        private void UpdateAtk()
        {
            atk = baseAtk;
        }
        
        private void UpdateClearCoin()
        {
            clearCoin = stage;
        }

        private void UpdateMainUI()
        {
            UIManager.Instance.UpdateUserName(userName);
            UIManager.Instance.UpdateStage(stage);
            UIManager.Instance.UpdateEnemyHp(CurrentEnemyHp);
            UIManager.Instance.UpdateAttackVal(atk);
            UIManager.Instance.UpdateCoinVal(coin);
        }

        private void FetchAllStatus(bool sendToServer)
        {
            // Update variables
            UpdateAtk();
            UpdateClearCoin();
            // Update UI
            UpdateMainUI();
            // Update Prefs and Server
            SaveAllInfoPrefs();
            if (sendToServer)
                NetworkManager.Instance.FetchUser().Forget();
        }
    }
}
