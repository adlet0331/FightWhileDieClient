using System;
using Combat;
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
        [SerializeField] private int _atk = 100;
        [SerializeField] private int _enemyHp = 100;

        private void Start()
        {
            CombatManager.Instance.AI.SetCombatHp(_enemyHp);
            Load();
            UIManager.Instance.UpdateStage(_stage);
            UIManager.Instance.UpdateEnemyHp(_enemyHp);
            CombatManager.Instance.AI.SetCombatHp(_enemyHp);
        }

        private void Load()
        {
            
        }

        public void Save()
        {
            
        }
        
        public void StageCleared()
        {
            _stage += 1;
            UIManager.Instance.UpdateStage(_stage);
            _enemyHp += 50;
            UIManager.Instance.UpdateEnemyHp(_enemyHp);
            _atk += 10;
            UIManager.Instance.UpdateAttackVal(_atk);
            CombatManager.Instance.AI.SetCombatHp(_enemyHp);
            Save();
        }
    }
}