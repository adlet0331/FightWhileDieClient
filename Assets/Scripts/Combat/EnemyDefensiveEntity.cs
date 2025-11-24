using System.Linq;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// 방어적인 적 엔티티
    /// - 거리를 유지하려는 성향이 있습니다
    /// - HP가 80% 이하일 때부터 차지 공격을 선호합니다
    /// - 일반 공격보다 차지 공격을 더 자주 사용합니다
    /// </summary>
    public class EnemyDefensiveEntity : EnemyEntity
    {
        [Header("Defensive Enemy Settings")]
        [SerializeField] private float defensiveChargeThreshold = 0.8f; // 80% HP에서 차지 공격 시작
        private float normalAttackCooldown = 0f; // 일반 공격 쿨다운 (런타임 상태)
        [SerializeField] private float normalAttackCooldownTime = 2f; // 일반 공격 쿨다운 시간
        
        protected override void WhenStart()
        {
            base.WhenStart();
            normalAttackCooldown = 0f;
        }
        
        private void Update()
        {
            // 일반 공격 쿨다운 감소
            if (normalAttackCooldown > 0f)
            {
                normalAttackCooldown -= Time.deltaTime;
            }
        }
        
        public override void ActionUpdate()
        {
            if (enemyActionBlockList.Contains(currentStatus)) return;
            
            // 범위 밖에 있으면 달리기
            if (!OpponentInRange && !OpponentInChargeRange)
            {
                EntityAction(CombatEntityStatus.Running);
            }
            // 차지 공격 범위 안에 있고 HP가 80% 이하일 때 - 차지 공격 선호
            else if (OpponentInChargeRange && CurrentHpRatio <= defensiveChargeThreshold)
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
            // 일반 공격 범위 안에 있을 때 - 쿨다운이 없을 때만 공격
            else if (OpponentInRange && normalAttackCooldown <= 0f)
            {
                EntityAction(CombatEntityStatus.Attack);
                normalAttackCooldown = normalAttackCooldownTime;
            }
        }
        
        protected override void WhenDamagedUpdateVal()
        {
            base.WhenDamagedUpdateVal();
            normalAttackCooldown = 0f; // 피해를 받으면 쿨다운 리셋
        }
    }
}
