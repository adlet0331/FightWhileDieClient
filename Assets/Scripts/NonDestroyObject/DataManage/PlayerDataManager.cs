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
        public int TopStage => topStage;
        public int Atk => atk;
        public int ClearCoin => clearCoin;
        public int DailyGatchaNum => dailyGatchaNum;
        public List<int> EnhanceIngredientList => enhanceIngredientList;

        [Header("Current Status")] 
        [SerializeField] private int id;
        [SerializeField] private string userName;
        [SerializeField] private int stage = 1;
        [SerializeField] private int baseAtk = 50;
        [SerializeField] private int enemyHp = 50;
        [SerializeField] private int coin = 10;
        [SerializeField] private int dailyGatchaNum;
        [SerializeField] private List<int> enhanceIngredientList;
        [Header("Update")]
        [SerializeField] private int topStage;
        [SerializeField] private int atk = 50;
        [SerializeField] private int clearCoin;
        
        # region PUBLIC
        
        public void Start()
        {
            StartLoadPrefs();
            UpdateAllStatus(true);
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
            dailyGatchaNum += 1;
            PlayerPrefs.SetInt("DailyGatchaNum", dailyGatchaNum);
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
            baseAtk += 10;
            coin += stage;
            UpdateAllStatus(true);
        }
        
        #endregion

        private void StartLoadPrefs()
        {
            userName = PlayerPrefs.GetString("Name", String.Empty);
            id = PlayerPrefs.GetInt("Id", -1);
            userName = PlayerPrefs.GetString("Name", "");
            topStage = PlayerPrefs.GetInt("TopStage", 1);
            baseAtk = PlayerPrefs.GetInt("BaseAtk", 50);
            coin = PlayerPrefs.GetInt("Coin", 10);
            stage = ((int)(topStage / 10.0f) * 10) + 1;
            dailyGatchaNum = PlayerPrefs.GetInt("DailyGatchaNum", 0);
            enhanceIngredientList = new List<int>();
            enhanceIngredientList.Add(0);
            for (int i = 1; i <=8; i++)
                enhanceIngredientList.Add(PlayerPrefs.GetInt($"enhanceIngredient{i}", i));
        }

        private void ClearAllPrefs()
        {
            NetworkManager.Instance.DeleteUser(id).Forget();
            id = -1;
            PlayerPrefs.SetInt("Id", -1);
            userName = string.Empty;
            PlayerPrefs.SetString("Name", string.Empty);
            topStage = 1;
            PlayerPrefs.SetInt("TopStage", 1);
            baseAtk = 50;
            PlayerPrefs.SetInt("BaseAtk", 50);
            coin = 10;
            PlayerPrefs.SetInt("Coin", 10);
            dailyGatchaNum = 0;
            PlayerPrefs.SetInt("DailyGatchaNum", 0);
            for (int i = 1; i <= 8; i++)
            {
                enhanceIngredientList[i] = 0;
                PlayerPrefs.SetInt($"enhanceIngredient{i}", 0);
            }
            UpdateAllStatus(true);
        }
        
        private void SavePrefs()
        {
            if (stage > topStage)
            {
                topStage = stage;
                PlayerPrefs.SetInt("TopStage", topStage);
            }
            PlayerPrefs.SetInt("BaseAtk", baseAtk);
            PlayerPrefs.SetInt("Coin", coin);
            PlayerPrefs.SetInt("DailyGatchaNum", dailyGatchaNum);
            for (int i = 1; i <=8; i++)
                PlayerPrefs.SetInt($"enhanceIngredient{i}", enhanceIngredientList[i]);
        }

        private void UpdateAtk()
        {
            atk = baseAtk;
            CombatManager.Instance.player.UpdateStatus(1, atk);
        }

        private void UpdateClearCoin()
        {
            clearCoin = stage;
        }

        private void UpdateEnemyHp()
        {
            enemyHp = (int)(50 * Math.Pow(1.2f, stage));
            CombatManager.Instance.enemyAI.UpdateStatus(enemyHp, 1);
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
            await UniTask.SwitchToThreadPool();
            try
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
                        else
                        {
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
            }
            catch (Exception e)
            {
                Debug.Log("Error in SaveAllInfos");
                Debug.Log(e);
            }
            await UniTask.Yield();
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
            SavePrefs();
            if (sendToServer)
                SaveAllInfosServer().Forget();
        }
    }
}
