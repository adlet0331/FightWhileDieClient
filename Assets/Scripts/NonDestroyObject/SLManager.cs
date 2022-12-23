 using System;
 using Cysharp.Threading.Tasks;
 using UnityEngine;

 namespace NonDestroyObject
{
    public class SLManager : Singleton<SLManager>
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

        [Header("Current Status")] 
        [SerializeField] private int id;
        [SerializeField] private string userName;
        [SerializeField] private int stage = 1;
        [SerializeField] private int baseAtk = 50;
        [SerializeField] private int enemyHp = 50;
        [SerializeField] private int coin = 10;
        [Header("Update")]
        [SerializeField] private int topStage;
        [SerializeField] private int atk = 50;
        [SerializeField] private int clearCoin;
        
        private void Start()
        {
            StartLoadPrefs();
            UpdateAllStatus(true);
        }

        private void StartLoadPrefs()
        {
            userName = PlayerPrefs.GetString("Name", String.Empty);
            id = PlayerPrefs.GetInt("Id", -1);
            userName = PlayerPrefs.GetString("Name", "");
            topStage = PlayerPrefs.GetInt("TopStage", 1);
            baseAtk = PlayerPrefs.GetInt("BaseAtk", 50);
            coin = PlayerPrefs.GetInt("Coin", 10);
            stage = ((int)(topStage / 10.0f) * 10) + 1;
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

        private void UpdateUI()
        {
            UIManager.Instance.UpdateUserName(userName);
            UIManager.Instance.UpdateStage(stage);
            UIManager.Instance.UpdateEnemyHp(enemyHp);
            UIManager.Instance.UpdateAttackVal(atk);
            UIManager.Instance.UpdateCoinVal(coin);
        }

        private void ClearAllPrefs()
        {
            NetworkManager.Instance.DeleteUser(id).Forget();
            id = -1;
            userName = string.Empty;
            PlayerPrefs.SetInt("Id", -1);
            PlayerPrefs.SetString("Name", string.Empty);
            PlayerPrefs.SetInt("TopStage", 1);
            PlayerPrefs.SetInt("BaseAtk", 50);
            PlayerPrefs.SetInt("Coin", 10);
            UpdateUI();
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
        }
        
        private async UniTaskVoid SaveAllInfos()
        {
            await UniTask.SwitchToThreadPool();
            try
            {
                var checkConnection = await NetworkManager.Instance.CheckConnection();
                switch (checkConnection)
                {
                    // No Connection
                    case CheckConnectionResult.No_Connection_To_Server:
                        break;
                    case CheckConnectionResult.Success:
                        NetworkManager.Instance.FetchUser().Forget();
                        break;
                    case CheckConnectionResult.Success_But_No_Id_In_Server:
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
            UpdateUI();
            // Update Prefs and Server
            SavePrefs();
            if (sendToServer)
                SaveAllInfos().Forget();
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
    }
}
