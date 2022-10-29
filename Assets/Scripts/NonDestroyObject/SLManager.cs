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

        public void Save()
        {
            
        }
        
        public void StageCleared()
        {
            _stage += 1;
            UIManager.Instance.UpdateStage(_stage);
            _enemyHp = (int)(1.1 * _enemyHp);
            UIManager.Instance.UpdateEnemyHp(_enemyHp);
            Save();
        }
    }
}