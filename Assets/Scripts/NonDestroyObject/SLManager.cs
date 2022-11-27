 using System;
 using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class SLManager : Singleton<SLManager>
    {
        public int Id => _id;
        public string Name => _name;
        public int Stage => _stage;
        public int ATK => _atk;
        public int EnemyHp => _enemyHp;
        public int Coin => _coin;

        [Header("Current Status")] 
        [SerializeField] private int _id;
        [SerializeField] private string _name;
        [SerializeField] private int _stage = 1;
        [SerializeField] private int _enemyHp = 50;
        [SerializeField] private int _atk = 50;
        [SerializeField] private int _coin = 10;
        [Header("Update")]
        [SerializeField] private int _topStage;
        [SerializeField] private int _baseAtk;
        
        private void Start()
        {
            Load();
            UpdateUI();
        }

        public void Load()
        {
            _id = PlayerPrefs.GetInt("Id", -1);
            _name = PlayerPrefs.GetString("Name", "");
            _topStage = PlayerPrefs.GetInt("TopStage", 1);
            _baseAtk = PlayerPrefs.GetInt("BaseAtk", 50);
            _coin = PlayerPrefs.GetInt("Coin", 10);
            _stage = (int)(_topStage / 10.0f) + 1;
            _atk = _baseAtk;
            _enemyHp = (int)(_enemyHp * Math.Pow(1.2f, _stage));
        }

        public void Save()
        {
            if (_stage > _topStage)
            {
                _topStage = _stage;
                PlayerPrefs.SetInt("TopStage", _topStage);
            }
            PlayerPrefs.SetInt("BaseAtk", _baseAtk);
            PlayerPrefs.SetInt("Coin", _coin);
        }

        private void UpdateUI()
        {
            UIManager.Instance.UpdateStage(_stage);
            UIManager.Instance.UpdateEnemyHp(_enemyHp);
            UIManager.Instance.UpdateAttackVal(_atk);
            UIManager.Instance.UpdateCoinVal(_coin);
        }

        public void StageReset()
        {
            _stage = (int)(_topStage / 10.0f) + 1;
            _enemyHp = 50;
            // _atk = 50;
            CombatManager.Instance.AI.UpdateStatus(_enemyHp, 1);
            Save();
            UpdateUI();
        }
        
        public void StageCleared()
        {
            _stage += 1;
            _enemyHp = (int)(_enemyHp * 1.2f);
            _baseAtk += 10;
            _coin += _stage;
            PlayerCombatManager.Instance.Player.UpdateStatus(1, _atk);
            CombatManager.Instance.AI.UpdateStatus(_enemyHp, 1);
            UpdateUI();
            Save();
        }
    }
}