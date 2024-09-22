using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NonDestroyObject;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace Combat
{
    public enum ObjectStatus
    {
        Idle = 0,
        Attack = 1,
        ChargeAttack = 2,
        Running = 3,
        Damaged = 4,
        JumpBack = 5,
        Dead = 6,
        Charge = 7,
        PerfectChargeAttack = 8
    }
    public enum ObjectType
    {
        Player = 0,
        AI = 1
    }
    [Serializable]
    public enum AttackType
    {
        Normal = 0,
        Charge = 1,
        PerfectCharge = 2,
    }
    public class CombatEntity : MonoBehaviour
    {
        [Header("Initial Setting")]
        [SerializeField] private int maxHp;
        [SerializeField] private float attackHitDuration;
        [SerializeField] private float attackAfterDelay;
        [SerializeField] private float chargeAttackHitDuration;
        [SerializeField] private float chargeAttackAfterDelay;
        [SerializeField] private float perfectChargeAttackHitDuration;
        [SerializeField] private float perfectChargeAttackAfterDelay;
        [SerializeField] private ObjectType type;
        [SerializeField] private ClipName hittingClip;
        [SerializeField] private ClipName chargehittingClip;
        [SerializeField] private ClipName perfecthittingClip;
        
        [Header("If enemyAI, Need to be Initialized")] 
        [SerializeField] public float runningSpeed;
        [SerializeField] public float backJumpSpeed;
        [SerializeField] public float knockBackXInterval;

        public bool EnemyInRange => attackHitBox.EnemyInRange;
        public bool EnemyInChargeRange => chargeAttackHitBox.EnemyInRange;
        public bool EnemyInPerfectChargeRange => perfectChargeAttackHitBox.EnemyInRange;
        // Current Status
        public bool Attacking => attacking;
        public AttackType AttackType => attackType;
        public bool Hitting => hitting;
        public bool Dying => dying;
        public bool Charging => charging;
        // Only For enemyAI
        public bool Running => running;
        public bool BackJumping => backJumping;
        public bool Damaging => damaging;

        [Header("Debugging")] 
        [SerializeField] public ObjectStatus currentStatus;
        [SerializeField] private int currentHp;
        [SerializeField] private bool attacking;
        [SerializeField] private AttackType attackType;
        [SerializeField] private bool hitting;
        [SerializeField] private bool dying;
        [SerializeField] private bool charging;
        [Header("Debugging Only For Enemy")]
        [SerializeField] private bool running;
        [SerializeField] private bool backJumping;
        [SerializeField] private bool damaging;

        [Header("Set In Unity")]
        [SerializeField] private Animator animator;
        [SerializeField] private AttackRangeObject attackHitBox;
        [SerializeField] private AttackRangeObject chargeAttackHitBox;
        [SerializeField] private AttackRangeObject perfectChargeAttackHitBox;

        #region Start
        private void Start()
        {
            currentHp = maxHp;

            // Coroutines
            _waitAndReturnToIdleCoroutine = null;
            _hittingCancelCoroutine = null;
            
            currentStatus = ObjectStatus.Idle;
        }
        #endregion

        #region IEnumerator

        public void StopAllCoroutines()
        {
            if (_waitAndReturnToIdleCoroutine != null)
                StopCoroutine(_waitAndReturnToIdleCoroutine);
            if (_hittingCancelCoroutine != null)
                StopCoroutine(_hittingCancelCoroutine);
        }

        public void RestartAllCoroutines()
        {
            if (_waitAndReturnToIdleCoroutine != null)
                StartCoroutine(_waitAndReturnToIdleCoroutine);
            if (_hittingCancelCoroutine != null)
                StartCoroutine(_hittingCancelCoroutine);
        }
        
        // 기다렸다가 IDLE로 전환
        private IEnumerator _waitAndReturnToIdleCoroutine;
        // 공격 판정변수 설정
        private IEnumerator _hittingCancelCoroutine;
        private delegate void OperationWaitAndReturnToIdle();
        private void WaitAndReturnToIdleWithOperation(float sec, OperationWaitAndReturnToIdle operation = null)
        {
            CancelWaitAndReturnToIdleCoroutine();
            _waitAndReturnToIdleCoroutine = CoroutineUtils.WaitAndOperationIEnum(sec, () =>
            {
                operation?.Invoke();
                Action(ObjectStatus.Idle);
                _waitAndReturnToIdleCoroutine = null;
            });
            StartCoroutine(_waitAndReturnToIdleCoroutine);
        }

        private void CancelWaitAndReturnToIdleCoroutine()
        {
            if (_waitAndReturnToIdleCoroutine != null)
            {
                StopCoroutine(_waitAndReturnToIdleCoroutine);
            }
            _waitAndReturnToIdleCoroutine = null;
        }
        private void HandleAttack(AttackType attackType)
        {
            this.attackType = attackType;
            CancelHittingJudgeCoroutine();
            CancelWaitAndReturnToIdleCoroutine();
            attacking = true;
            hitting = true;
            switch (attackType)
            {
                case AttackType.Normal:
                    SoundManager.Instance.PlayClip((int)hittingClip);
                    _hittingCancelCoroutine = CoroutineUtils.WaitAndOperationIEnum(attackHitDuration, () =>
                    {
                        _hittingCancelCoroutine = null;
                        hitting = false;
                    });
                    _waitAndReturnToIdleCoroutine = CoroutineUtils.WaitAndOperationIEnum(attackHitDuration + attackAfterDelay, () =>
                    {
                        Action(ObjectStatus.Idle);
                        attacking = false;
                        _waitAndReturnToIdleCoroutine = null;
                    });
                    break;
                case AttackType.Charge:
                    SoundManager.Instance.PlayClip((int)hittingClip);
                    _hittingCancelCoroutine = CoroutineUtils.WaitAndOperationIEnum(chargeAttackHitDuration, () =>
                    {
                        _hittingCancelCoroutine = null;
                        hitting = false;
                    });
                    _waitAndReturnToIdleCoroutine = CoroutineUtils.WaitAndOperationIEnum(chargeAttackHitDuration + chargeAttackAfterDelay, () =>
                    {
                        Action(ObjectStatus.Idle);
                        attacking = false;
                        _waitAndReturnToIdleCoroutine = null;
                    });
                    break;
                case AttackType.PerfectCharge:
                    SoundManager.Instance.PlayClip((int)hittingClip);
                    _hittingCancelCoroutine = CoroutineUtils.WaitAndOperationIEnum(perfectChargeAttackHitDuration, () =>
                    {
                        _hittingCancelCoroutine = null;
                        hitting = false;
                    });
                    _waitAndReturnToIdleCoroutine = CoroutineUtils.WaitAndOperationIEnum(perfectChargeAttackHitDuration + perfectChargeAttackAfterDelay, () =>
                    {
                        Action(ObjectStatus.Idle);
                        attacking = false;
                        _waitAndReturnToIdleCoroutine = null;
                    });
                    break;
            }
            StartCoroutine(_hittingCancelCoroutine);
            
            StartCoroutine(_waitAndReturnToIdleCoroutine);
        }
        
        private void SwitchStatus(ObjectStatus newStatus)
        {
            if (currentStatus == newStatus) return;
            
            attacking = damaging = dying = running = backJumping = charging = false;
            animator.SetBool(currentStatus.ToString(), false);
            
            currentStatus = newStatus;
            
            switch (currentStatus)
            {
                case ObjectStatus.Charge:
                    charging = true;
                    break;
                case ObjectStatus.Attack:
                    attacking = true;
                    break;
                case ObjectStatus.ChargeAttack:
                    attacking = true;
                    break;
                case ObjectStatus.PerfectChargeAttack:
                    attacking = true;
                    break;
                case ObjectStatus.Damaged:
                    damaging = true;
                    break;
                case ObjectStatus.Dead:
                    dying = true;
                    break;
                case ObjectStatus.Running:
                    running = true;
                    break;
                case ObjectStatus.JumpBack:
                    backJumping = true;
                    break;
            }
            animator.SetBool(currentStatus.ToString(), true);
        }

        public void CancelHittingJudgeCoroutine()
        {
            hitting = false;
            if (_hittingCancelCoroutine != null)
            {
                StopCoroutine(_hittingCancelCoroutine);
                _hittingCancelCoroutine = null;
            }
        }
        #endregion

        public void Show()
        {
            attackHitBox.WhenShow();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            attackHitBox.WhenHide();
        }
        
        public void EnableAnimation(bool enable)
        {
            animator.speed = enable ? 1.0f : 0.0f;
        }
        
        // Animation 시간 이름으로 받아오기
        public float GetAnimationTime(string animationName)
        {
            return AnimatorUtil.GetAnimationTime(animationName, animator.runtimeAnimatorController);
        }

        public void ResetAfterDie()
        {
            attackHitBox.ResetInRange();
            CancelHittingJudgeCoroutine();
            CancelWaitAndReturnToIdleCoroutine();
            Action(ObjectStatus.Idle);
        }
        
        public void SetHp(int mhp)
        {
            currentHp = mhp;
            maxHp = mhp;
        }

        public bool Damaged(int damage)
        {
            currentHp = currentHp - damage > 0 ? currentHp - damage : 0;
            if (currentHp == 0)
            {
                if (type == ObjectType.AI)
                    UIManager.Instance.UpdateEnemyHp(0);
                SwitchStatus(ObjectStatus.Dead);
                WaitAndReturnToIdleWithOperation(GetAnimationTime("Dead"), () =>
                {
                    dying = false;
                });
                return true;
            }
            else
            {
                if (type == ObjectType.AI)
                    UIManager.Instance.UpdateEnemyHp((float)currentHp / maxHp);
                SwitchStatus(ObjectStatus.Damaged);
                WaitAndReturnToIdleWithOperation(GetAnimationTime("Damaged"));
                return false;
            }
        }

        public void Action(ObjectStatus objectStatus)
        {
            // Animation 및 변수들 처리
            SwitchStatus(objectStatus);
            
            // Player Action WhiteList
            if (type == ObjectType.Player &&
                (objectStatus != ObjectStatus.Idle && objectStatus != ObjectStatus.Attack &&
                 objectStatus != ObjectStatus.Dead && objectStatus != ObjectStatus.Running &&
                 objectStatus != ObjectStatus.ChargeAttack && objectStatus != ObjectStatus.PerfectChargeAttack))
            {
                Debug.LogAssertion("Player Action Rejected: " + objectStatus);
                return;
            }
            // enemyAI Action BlackList
            if (type == ObjectType.AI && 
                (objectStatus is ObjectStatus.Dead or ObjectStatus.Damaged))
            {
                Debug.LogAssertion("enemyAI Action Rejected: " + objectStatus);
                return;
            }

            CancelWaitAndReturnToIdleCoroutine();
            switch (objectStatus)
            {
                // 차지
                case ObjectStatus.Charge:
                    // 공격 도중 
                    break;
                // 공격
                case ObjectStatus.Attack:
                    // Hitting 변수
                    HandleAttack(AttackType.Normal);
                    break;
                // 차지 공격
                case ObjectStatus.ChargeAttack:
                    HandleAttack(AttackType.Charge);
                    break;
                case ObjectStatus.PerfectChargeAttack:
                    HandleAttack(AttackType.PerfectCharge);
                    break;
                // 죽음 (플레이어만)
                case ObjectStatus.Dead:
                    WaitAndReturnToIdleWithOperation(GetAnimationTime("Dead"));
                    break;
                // 움직임
                case ObjectStatus.Running:
                    break;
                case ObjectStatus.JumpBack:
                    float animationTime = GetAnimationTime("JumpBack");
                    WaitAndReturnToIdleWithOperation(animationTime, () =>
                    {
                        backJumping = false;
                    });
                    break;
            }
        }
    }
}