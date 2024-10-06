using UnityEngine;

namespace Combat
{
    public class PlayerEntity : CombatEntityParent
    {
        [Header("PlayerEntity")]
        [SerializeField] private float perfectChargeAttackHitDuration;
        [SerializeField] private AttackRangeObject perfectChargeAttackHitBox;
        
        public bool EnemyInPerfectChargeRange => perfectChargeAttackHitBox.OpponentInRange;

        public override bool EntityAction(CombatEntityStatus combatEntityStatus)
        {
            if (base.EntityAction(combatEntityStatus)) return true;

            switch (combatEntityStatus)
            {
                case CombatEntityStatus.PerfectChargeAttack:
                    WaitAndReturnToIdleWithOperation(GetAnimationTime("PerfectChargeAttack"));
                    StartHitJudgeAndEndAfter(perfectChargeAttackHitDuration);
                    return true;
            }

            return false;
        }
    }
}