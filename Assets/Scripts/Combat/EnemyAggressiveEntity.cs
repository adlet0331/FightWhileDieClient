using System.Linq;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// 공격적인 적 엔티티
    /// - 더 빈번하게 공격합니다
    /// - HP가 70% 이하일 때 차지 공격을 사용합니다
    /// - 일반 공격과 차지 공격을 적극적으로 사용합니다
    /// </summary>
    public class EnemyAggressiveEntity : EnemyEntity
    {
        [Header("Aggressive Enemy Settings")]
        [SerializeField] private float aggressiveChargeThreshold = 0.7f; // 70% HP에서 차지 공격 시작
        
        public override void ActionUpdate()
        {
            if (enemyActionBlockList.Contains(currentStatus)) return;
            
            // 범위 밖에 있으면 달리기
            if (!OpponentInRange && !OpponentInChargeRange)
            {
                EntityAction(CombatEntityStatus.Running);
            }
            // 차지 공격 범위 안에 있고 HP가 70% 이하일 때
            else if (OpponentInChargeRange && CurrentHpRatio <= aggressiveChargeThreshold)
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
            // 일반 공격 범위 안에 있을 때 - 공격적으로 바로 공격
            else if (OpponentInRange)
            {
                EntityAction(CombatEntityStatus.Attack);
            }
        }
    }
}
