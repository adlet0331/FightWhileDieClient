using System;
using Combat;
using Unity.VisualScripting;
using UnityEngine;

namespace NonDestroyObject
{
    public class PlayerCombatManager : Singleton<PlayerCombatManager>
    {
        public CombatObject Player;

        public void PlayerDie()
        {
            // Range 처리 초기화
            Player.ResetAfterDie();
            CombatManager.Instance.AI.ResetAfterDie();
            
            // 스테이지 리셋
            SLManager.Instance.StageReset();
            // 다음 스테이지 X, Combat End
            StageMoveManager.Instance.StopCombat(false);
        }
    }
}