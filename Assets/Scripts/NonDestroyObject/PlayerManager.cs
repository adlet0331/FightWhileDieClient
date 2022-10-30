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
            // Range 처리 초기화
            Player.ResetInRange();
            CombatManager.Instance.AI.ResetInRange();
            
            // 스테이지 리셋
            SLManager.Instance.StageReset();
            // 다음 스테이지 X, Combat End
            StageMoveManager.Instance.StopCombat(false);
        }
    }
}