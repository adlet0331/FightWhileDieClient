using System.Linq;
using UnityEngine;

namespace Combat
{
    public class EnemyEntity : CombatEntityParent
    {
        [Header("EnemyEntity Init In Unity Variables")]
        [SerializeField] private float chargeFullTime;
        [Header("EnemyEntity Variables")]
        [SerializeField] private float chargeElapsedTime;
        public float CurrentHpRatio => (float)currentHp / (float)maxHp;
        public virtual bool[] EnemyHittingPlayers()
        {
            if (Attacking && Hitting)
            {
                switch (AttackType)
                {
                    case AttackType.Normal:
                        return OpponentInRange;
                    case AttackType.Charge:
                        return OpponentInChargeRange;
                    default:
                        Debug.Assert(false, "Unknown AttackType in EnemyHittingPlayer");
                        return new bool[OpponentInRange.Length];
                }
            }
            else
            {
                return OpponentInRange.Length > 0 ? new bool[OpponentInRange.Length] : new bool[0];
            }
        }

        protected override void WhenStart()
        {
            base.WhenStart();
            chargeElapsedTime = 0;
        }
        
        public virtual void ActionUpdate()
        {
            if (enemyActionBlockList.Contains(currentStatus)) return;
            if (!OpponentInRange.Any(hit => hit) && !OpponentInChargeRange.Any(hit => hit))
            {
                if (currentStatus != CombatEntityStatus.Running)
                {
                    EntityAction(CombatEntityStatus.Running);
                }
            }
            else if (OpponentInChargeRange.Any(hit => hit) && CurrentHpRatio <= 0.5f)
            {
                if (chargeElapsedTime == 0.0f)
                {
                    EntityAction(CombatEntityStatus.Charge);
                    chargeElapsedTime += Time.deltaTime;
                }
                else if (chargeElapsedTime <= chargeFullTime)
                {
                    chargeElapsedTime += Time.deltaTime;
                }
                else
                {
                    EntityAction(CombatEntityStatus.ChargeAttack);
                    WaitAndReturnToIdleWithOperation(GetAnimationTime("ChargeAttack"));
                    chargeElapsedTime = 0.0f;
                }
            }
            else if (OpponentInRange.Any(hit => hit))
            {
                EntityAction(CombatEntityStatus.Attack);
            }
        }

        protected virtual void WhenDamagedUpdateVal()
        {
            chargeElapsedTime = 0;
        }
        
        protected CombatEntityStatus[] enemyActionBlockList = new CombatEntityStatus[]
        {
            CombatEntityStatus.Damaged,
            CombatEntityStatus.Dying,
            CombatEntityStatus.Attack,
            CombatEntityStatus.ChargeAttack
        };
        public override bool Damaged(int damage)
        {
            CancelAllCoroutine();
            WhenDamagedUpdateVal();
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
            if (base.EntityAction(combatEntityStatus))
            {
                return true;
            }

            return false;
        }
    }
}