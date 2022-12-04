 using System;
 using System.Threading.Tasks;
 using UnityEngine;

 namespace NonDestroyObject
{
    public class SLManager : Singleton<SLManager>
    {
        public int Id => _id;
        public string Name => _name;
        public int Stage => _stage;
        public int BaseAtk => _baseAtk;
        public int EnemyHp => _enemyHp;
        public int Coin => _coin;
        public int TopStage => _topStage;
        public int Atk => _atk;
        public int ClearCoin => _clearCoin;

        [Header("Current Status")] 
        [SerializeField] private int _id;
        [SerializeField] private string _name;
        [SerializeField] private int _stage = 1;
        [SerializeField] private int _baseAtk = 50;
        [SerializeField] private int _enemyHp = 50;
        [SerializeField] private int _coin = 10;
        [Header("Update")]
        [SerializeField] private int _topStage;
        [SerializeField] private int _atk = 50;
        [SerializeField] private int _clearCoin;
        
        private void Start()
        {
            LoadPrefs();
            Task.Run(() => SaveAllInfos());
        }

        public void LoadPrefs()
        {
            _name = PlayerPrefs.GetString("Name", String.Empty);
            _id = PlayerPrefs.GetInt("Id", -1);
            _name = PlayerPrefs.GetString("Name", "");
            _topStage = PlayerPrefs.GetInt("TopStage", 1);
            _baseAtk = PlayerPrefs.GetInt("BaseAtk", 50);
            _coin = PlayerPrefs.GetInt("Coin", 10);
            _stage = ((int)(_topStage / 10.0f) * 10) + 1;
            UpdateAllStatus();
        }
        
        public void InitUser(int id, string userName)
        {
            _id = id;
            _name = userName;
            PlayerPrefs.SetInt("Id", id);
            PlayerPrefs.SetString("Name", userName);
            UIManager.Instance.UpdateUserName(_name);
        }

        private void UpdateAtk()
        {
            _atk = _baseAtk;
            CombatManager.Instance.Player.UpdateStatus(1, _atk);
        }

        private void UpdateClearCoin()
        {
            _clearCoin = _stage;
        }

        private void UpdateEnemyHp()
        {
            _enemyHp = (int)(50 * Math.Pow(1.2f, _stage));
            CombatManager.Instance.AI.UpdateStatus(_enemyHp, 1);
        }

        private void UpdateUI()
        {
            UIManager.Instance.UpdateUserName(_name);
            UIManager.Instance.UpdateStage(_stage);
            UIManager.Instance.UpdateEnemyHp(_enemyHp);
            UIManager.Instance.UpdateAttackVal(_atk);
            UIManager.Instance.UpdateCoinVal(_coin);
        }
        
        private void SavePrefs()
        {
            if (_stage > _topStage)
            {
                _topStage = _stage;
                PlayerPrefs.SetInt("TopStage", _topStage);
            }
            PlayerPrefs.SetInt("BaseAtk", _baseAtk);
            PlayerPrefs.SetInt("Coin", _coin);
        }
        
        private void SaveAllInfos()
        {
            try
            {
                var checkConnection = Task.Run(() => NetworkManager.Instance.CheckConnection());
                checkConnection.Wait();
                switch (checkConnection.Result)
                {
                    case CheckConnectionResult.No_Connection_To_Server:
                        break;
                    case CheckConnectionResult.Success:
                        var fetchUser = Task.Run(() => NetworkManager.Instance.FetchUser());
                        fetchUser.Wait();

                        break;
                    case CheckConnectionResult.Success_But_No_Id_In_Server:
                        if (_name == String.Empty)
                        {
                            UIManager.Instance.ShowPopupEnterYourNickname();
                        }
                        else
                        {
                            var createNewUser = Task.Run(() => NetworkManager.Instance.CreateNewUser(_name));
                            createNewUser.Wait();

                            switch (createNewUser.Result)
                            {
                                case CreateNewUserResult.Success:
                                    _id = NetworkManager.Instance.playerId;
                                    PlayerPrefs.SetInt("Id", _id);
                                    break;
                            }
                        }
                        break;
                }
            }
            catch (AggregateException e)
            {
                Debug.Log(e);
            }
        }

        private void UpdateAllStatus()
        {
            // Update variables
            UpdateAtk();
            UpdateEnemyHp();
            UpdateClearCoin();
            // Update UI
            UpdateUI();
            // Update Prefs and Server
            SavePrefs();
            Task.Run(() => SaveAllInfos());
        }

        public void StageReset()
        {
            _stage = ((int)(_topStage / 10.0f) * 10) + 1;
            UpdateAllStatus();
        }
        
        public void StageCleared()
        {
            _stage += 1;
            _baseAtk += 10;
            _coin += _stage;
            UpdateAllStatus();
        }
    }
}
