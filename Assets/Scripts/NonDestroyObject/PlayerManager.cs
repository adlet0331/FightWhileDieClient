using System;
using Combat;
using Unity.VisualScripting;
using UnityEngine;

namespace NonDestroyObject
{
    public class PlayerManager : Managers.Singleton<PlayerManager>
    {
        public CombatObject Player;

        private void Start()
        {
            if (Player.IsUnityNull())
            {
                Player = GetComponent<CombatObject>();
            }
        }

        public void PlayerDie()
        {
            CombatManager.Instance.AI.transform.position =
                StageMoveManager.Instance.enemyStartPosition.transform.position;
            SLManager.Instance.StageReset();
            Player.ResetInRange();
            CombatManager.Instance.AI.ResetInRange();
        }
    }
}