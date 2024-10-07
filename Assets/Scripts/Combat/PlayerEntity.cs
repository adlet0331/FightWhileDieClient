using UnityEngine;

namespace Combat
{
    public class PlayerEntity : CombatEntityParent
    {
        [Header("PlayerEntity")]
        [SerializeField] private float perfectChargeAttackHitDuration;
        [SerializeField] private AttackRangeObject perfectChargeAttackHitBox;
        
        public bool EnemyInPerfectChargeRange => perfectChargeAttackHitBox.OpponentInRange;
        public bool PlayerHittingEnemy => Attacking && Hitting && ((AttackType == AttackType.Normal && OpponentInRange) ||
                                                                   (AttackType == AttackType.Charge && OpponentInChargeRange) ||
                                                                   (AttackType == AttackType.PerfectCharge && EnemyInPerfectChargeRange));

        public override bool Damaged(int damage)
        {
            CancelAllCoroutine();
            currentHp = currentHp - damage > 0 ? currentHp - damage : 0;
            if (currentHp == 0)
            {
                SwitchStatusAndAnimation(CombatEntityStatus.Dying);
                currentStatus = CombatEntityStatus.Dying;
                WaitAndReturnToIdleWithOperation(GetAnimationTime("Dying"), ResetAfterDead);
                return true;
            }
            else
            {
                SwitchStatusAndAnimation(CombatEntityStatus.Damaged);
                currentStatus = CombatEntityStatus.Damaged;
                WaitAndReturnToIdleWithOperation(GetAnimationTime("Damaged"));
                return false;
            }
        }
        public override bool EntityAction(CombatEntityStatus combatEntityStatus)
        {
            if (base.EntityAction(combatEntityStatus)) return true;

            switch (combatEntityStatus)
            {
                case CombatEntityStatus.PerfectChargeAttack:
                    WaitAndReturnToIdleWithOperation(GetAnimationTime("PerfectChargeAttack"));
                    StartAttack(perfectChargeAttackHitDuration, AttackType.PerfectCharge);
                    return true;
            }

            return false;
        }
    }
}