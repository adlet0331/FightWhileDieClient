using UnityEngine;

namespace Combat
{
    public class PlayerEntity : CombatEntityParent
    {
        [Header("PlayerEntity")]
        [SerializeField] private float perfectChargeAttackHitDuration;
        [SerializeField] private AttackRangeObject perfectChargeAttackHitBox;
        
        public bool EnemyInPerfectChargeRange => perfectChargeAttackHitBox.EnemyInRange;

        public override bool EntityAction(CombatEntityStatus combatEntityStatus)
        {
            if (base.EntityAction(combatEntityStatus)) return true;

            switch (combatEntityStatus)
            {
                case CombatEntityStatus.PerfectChargeAttack:
                    
                    return true;
            }

            return false;
        }
    }
}