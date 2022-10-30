using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class SLManager : Singleton<SLManager>
    {
        public int Stage => _stage;
        public int ATK => _atk;
        public int EnemyHp => _enemyHp;

        [SerializeField] private int _stage = 1;
        [SerializeField] private int _atk = 50;
        [SerializeField] private int _enemyHp = 50;
        [SerializeField] private int _coin = 10;

        private void Start()
        {
            Load();
            UIManager.Instance.UpdateStage(_stage);
            UIManager.Instance.UpdateEnemyHp(_enemyHp);
            CombatManager.Instance.AI.UpdateStatus(_enemyHp);
            UpdateUI();
        }

        private void Load()
        {
            
        }

        public void Save()
        {
            
        }

        private void UpdateUI()
        {
            UIManager.Instance.UpdateStage(_stage);
            UIManager.Instance.UpdateEnemyHp(_enemyHp);
            UIManager.Instance.UpdateAttackVal(_atk);
        }

        public void StageReset()
        {
            _stage = 1;
            _enemyHp = 50;
            _atk = 50;
            CombatManager.Instance.AI.UpdateStatus(_enemyHp);
            UpdateUI();
        }
        
        public void StageCleared()
        {
            _stage += 1;
            _enemyHp += 50;
            _atk += 10;
            CombatManager.Instance.AI.UpdateStatus(_enemyHp);
            UpdateUI();
            Save();
        }
    }
}