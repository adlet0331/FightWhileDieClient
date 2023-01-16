using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NonDestroyObject.DataManage
{
    /// <summary>
    /// Unity PlayerPrefs로 관리하는 데이터들을 다루는 매니저
    /// </summary>
    [Serializable]
    public class PlayerDataManager
    {
        public int Id => id;
        public string UserName => userName;
        public int Stage => stage;
        public int BaseAtk => baseAtk;
        public int EnemyHp => enemyHp;
        public int Coin => coin;
        public int DailyGatchaCount => dailyGatchaCount;
        public List<int> EnhanceIngredientList => enhanceIngredientList;
        public int TopStage => topStage;
        public int Atk => atk;
        public int ClearCoin => clearCoin;

        public int GatchaStartCoin = 100;

        [Header("Current Status")] 
        [SerializeField] private int id;
        [SerializeField] private string userName;
        [SerializeField] private int stage = 1;
        [SerializeField] private int baseAtk = 50;
        [SerializeField] private int enemyHp = 50;
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
            UpdateAllStatus(false);
        }
        
        /// <summary>
        /// Must be called in Main Thread
        /// </summary>
        /// <param name="idp"></param>
        /// <param name="userNameParam"></param>
        public void InitUser(int idp, string userNameParam)
        {
            UniTask.SwitchToMainThread();
            this.id = idp;
            this.userName = userNameParam;
            PlayerPrefs.SetInt("Id", idp);
            PlayerPrefs.SetString("Name", userNameParam);
            UIManager.Instance.UpdateUserName(this.userName);
        }
        
        public void DeleteExistingUser()
        {
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
            UpdateAllStatus(false);
        }
        
        public void StageCleared()
        {
            stage += 1;
            if (stage > topStage)
                topStage = stage;
            baseAtk += 10;
            coin += stage;
            UpdateAllStatus(true);
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

        private void ClearAllPrefs()
        {
            NetworkManager.Instance.DeleteUser(id).Forget();

            userName = string.Empty;
            id = 0;
            topStage = 1;
            baseAtk = 50;
            coin = 10;
            dailyGatchaCount = 0;

            for (int i = 1; i <= 8; i++)
            {
                enhanceIngredientList[i] = 0;
            }

            UpdateAllStatus(true);
        }

        private void UpdateAtk()
        {
            atk = baseAtk;
            CombatManager.Instance.player.UpdateStatus(1, atk);
        }

        private void UpdateEnemyHp()
        {
            enemyHp = (int)(50 * Math.Pow(1.2f, stage));
            CombatManager.Instance.enemyAI.UpdateStatus(enemyHp, 1);
        }

        private void UpdateClearCoin()
        {
            clearCoin = stage;
        }

        private void UpdateMainUI()
        {
            UIManager.Instance.UpdateUserName(userName);
            UIManager.Instance.UpdateStage(stage);
            UIManager.Instance.UpdateEnemyHp(enemyHp);
            UIManager.Instance.UpdateAttackVal(atk);
            UIManager.Instance.UpdateCoinVal(coin);
        }
        
        private async UniTaskVoid SaveAllInfosServer()
        {
            var checkConnection = await NetworkManager.Instance.CheckConnection();
            switch (checkConnection)
            {
                // No Connection
                case CheckConnectionResult.NoConnectionToServer:
                    break;
                case CheckConnectionResult.Success:
                    NetworkManager.Instance.FetchUser().Forget();
                    break;
                case CheckConnectionResult.SuccessButNoIdInServer:
                    if (userName == String.Empty)
                    {
                        await UniTask.SwitchToMainThread();
                        UIManager.Instance.ShowPopupEnterYourNickname();
                        break;
                    }
                    var createNewUser = await NetworkManager.Instance.CreateNewUser(userName);

                    switch (createNewUser)
                    {
                        case CreateNewUserResult.Success:
                            await UniTask.SwitchToMainThread();
                            break;
                    }
                    break;
            }
        }

        private void UpdateAllStatus(bool sendToServer)
        {
            // Update variables
            UpdateAtk();
            UpdateEnemyHp();
            UpdateClearCoin();
            // Update UI
            UpdateMainUI();
            // Update Prefs and Server
            SaveAllInfoPrefs();
            if (sendToServer)
                SaveAllInfosServer().Forget();
        }
    }
}
