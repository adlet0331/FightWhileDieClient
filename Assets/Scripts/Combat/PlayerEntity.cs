using NonDestroyObject;
using UnityEngine;

namespace Combat
{
    public class PlayerEntity : CombatEntityParent
    {
        [Header("PlayerEntity")]
        [SerializeField] private float perfectChargeAttackHitDuration;
        [SerializeField] private AttackRangeObject perfectChargeAttackHitBox;
        
        public bool[] EnemyInPerfectChargeRange => perfectChargeAttackHitBox.OpponentInRanges;
        public bool[] PlayerHittingEnemys()
        {
            if (Attacking && Hitting)
            {
                switch (AttackType)
                {
                    case AttackType.Normal:
                        return OpponentInRange;
                    case AttackType.Charge:
                        return OpponentInChargeRange;
                    case AttackType.PerfectCharge:
                        return EnemyInPerfectChargeRange;
                    default:
                        Debug.Assert(false, "Unknown AttackType in PlayerHittingEnemys");
                        return new bool[OpponentInRange.Length];
                }
            }
            else
            {
                return OpponentInRange.Length > 0 ? new bool[OpponentInRange.Length] : new bool[0];
            }
        }

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
                    SoundManager.Instance.PlayClip(ClipName.PlayerPerfectAttack);
                    return true;
            }

            return false;
        }
    }
}