using System.Linq;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// 광전사 적 엔티티
    /// - HP가 낮아질수록 더 공격적이 됩니다
    /// - 다단계 분노 시스템으로 여러 차지 공격 단계가 있습니다
    /// - HP 75%, 50%, 25%에서 각각 다른 차지 패턴을 사용합니다
    /// </summary>
    public class EnemyBerserkerEntity : EnemyEntity
    {
        [Header("Berserker Enemy Settings")]
        [SerializeField] private float berserkerPhase1Threshold = 0.75f; // 1단계: 75% HP
        [SerializeField] private float berserkerPhase2Threshold = 0.5f;  // 2단계: 50% HP
        [SerializeField] private float berserkerPhase3Threshold = 0.25f; // 3단계: 25% HP
        [SerializeField] private float phase1ChargeTimeMultiplier = 1.0f; // 1단계 차지 시간 배율
        [SerializeField] private float phase2ChargeTimeMultiplier = 0.7f; // 2단계 차지 시간 배율 (더 빠름)
        [SerializeField] private float phase3ChargeTimeMultiplier = 0.4f; // 3단계 차지 시간 배율 (매우 빠름)
        
        private int currentBerserkerPhase = 0;
        
        protected override void WhenStart()
        {
            base.WhenStart();
            currentBerserkerPhase = 0;
        }
        
        private int GetCurrentPhase()
        {
            if (CurrentHpRatio <= berserkerPhase3Threshold) return 3;
            if (CurrentHpRatio <= berserkerPhase2Threshold) return 2;
            if (CurrentHpRatio <= berserkerPhase1Threshold) return 1;
            return 0;
        }
        
        private float GetCurrentChargeTime()
        {
            switch (currentBerserkerPhase)
            {
                case 1: return chargeFullTime * phase1ChargeTimeMultiplier;
                case 2: return chargeFullTime * phase2ChargeTimeMultiplier;
                case 3: return chargeFullTime * phase3ChargeTimeMultiplier;
                default: return chargeFullTime;
            }
        }
        
        public override void ActionUpdate()
        {
            if (enemyActionBlockList.Contains(currentStatus)) return;
            
            // 현재 분노 단계 업데이트
            int newPhase = GetCurrentPhase();
            if (newPhase != currentBerserkerPhase)
            {
                currentBerserkerPhase = newPhase;
                // 새로운 단계로 진입 시 차지 시간 리셋
                chargeElapsedTime = 0f;
            }
            
            float currentChargeTime = GetCurrentChargeTime();
            
            // 범위 밖에 있으면 달리기
            if (!OpponentInRange && !OpponentInChargeRange)
            {
                EntityAction(CombatEntityStatus.Running);
            }
            // 분노 단계에 진입했고 차지 공격 범위 안에 있을 때
            else if (OpponentInChargeRange && currentBerserkerPhase > 0)
            {
                if (chargeElapsedTime == 0.0f)
                {
                    EntityAction(CombatEntityStatus.Charge);
                    chargeElapsedTime += Time.deltaTime;
                }
                else if (chargeElapsedTime <= currentChargeTime)
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
            // 일반 공격 범위 안에 있을 때
            else if (OpponentInRange)
            {
                EntityAction(CombatEntityStatus.Attack);
            }
        }
        
        protected override void WhenDamagedUpdateVal()
        {
            base.WhenDamagedUpdateVal();
            // 피해를 받아도 분노 단계는 유지됨 (차지만 리셋)
        }
    }
}
