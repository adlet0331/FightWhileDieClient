﻿using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using NonDestroyObject;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace Combat
{
    // Should Named Same As Animation Bool
    public enum CombatEntityStatus
    {
        // Common
        Idle,
        Attack,
        ChargeAttack,
        Running,
        Damaged,
        JumpBack,
        Dying,
        Charge,
        // Only Player
        PerfectChargeAttack
        // Enemy AI
    }
    [Serializable]
    public enum AttackType
    {
        Normal = 0,
        Charge = 1,
        // Only Player
        PerfectCharge = 2,
        // Enemy AI's Attack Type
    }
    public abstract class CombatEntityParent : MonoBehaviour
    {
        [Header("Initial Setting (CombatEntity Parent)")]
        [SerializeField] protected int maxHp;
        [SerializeField] private float attackHitDuration;
        [SerializeField] private float chargeAttackHitDuration;
        [SerializeField] private ClipName hittingClip;
        [SerializeField] private ClipName chargehittingClip;
        [SerializeField] public float runningSpeed;
        [SerializeField] public float backJumpSpeed;
        [SerializeField] public float knockBackXInterval;

        public bool OpponentInRange => attackHitBox.OpponentInRange;
        public bool EnemyInChargeRange => chargeAttackHitBox.OpponentInRange;
        // Current Status
        public AttackType AttackType => attackType;
        public CombatEntityStatus CurrentStatus => currentStatus;
        public bool Attacking => (currentStatus == CombatEntityStatus.Attack) || (currentStatus == CombatEntityStatus.ChargeAttack) || (currentStatus == CombatEntityStatus.PerfectChargeAttack);
        public bool Hitting => hitting;
        [Header("Entity Status (CombatEntity Parent)")]
        [SerializeField] protected CombatEntityStatus currentStatus;
        [SerializeField] protected int currentHp;
        [SerializeField] private AttackType attackType;
        [SerializeField] private bool hitting;
        [Header("Set In Unity (CombatEntity Parent)")]
        [SerializeField] private Animator animator;
        [SerializeField] private AttackRangeObject attackHitBox;
        [SerializeField] private AttackRangeObject chargeAttackHitBox;

        private void Start()
        {
            // Coroutines
            CancelAllCoroutine();
            currentStatus = CombatEntityStatus.Idle;
        }

        // 기다렸다가 IDLE로 전환
        private IEnumerator _waitAndReturnToIdleCoroutine;
        // 공격 판정변수 설정
        private IEnumerator _hittingDisableCoroutine;

        protected delegate void OperationWaitAndDisableHitting();
        protected delegate void OperationWaitAndReturnToIdle();
        protected void WaitAndReturnToIdleWithOperation(float sec, OperationWaitAndReturnToIdle operation = null)
        {
            CancelWaitAndReturnToIdleCoroutine();
            _waitAndReturnToIdleCoroutine = CoroutineUtils.WaitAndOperationIEnum(sec, () =>
            {
                operation?.Invoke();
                EntityAction(CombatEntityStatus.Idle);
                _waitAndReturnToIdleCoroutine = null;
            });
            StartCoroutine(_waitAndReturnToIdleCoroutine);
        }
        protected void StartHitJudgeAndEndAfter(float sec, OperationWaitAndDisableHitting operation = null)
        {
            CancelHittingJudgeCoroutine();
            hitting = true;
            _hittingDisableCoroutine = CoroutineUtils.WaitAndOperationIEnum(sec, () =>
            {
                operation?.Invoke();
                CancelHittingJudgeCoroutine();
            });
            StartCoroutine(_hittingDisableCoroutine);
        }
        
        /// <summary>
        /// Switch "currentStatus" and Animation Variable
        /// </summary>
        /// <param name="newStatus"></param>
        private void SwitchStatusAndAnimation(CombatEntityStatus newStatus)
        {
            if (currentStatus == newStatus) return;
            animator.SetBool(currentStatus.ToString(), false);
            animator.SetBool(newStatus.ToString(), true);
        }

        /// <summary>
        /// Cancel All Coroutine in this Combat Entity
        /// </summary>
        protected virtual void CancelAllCoroutine()
        {
            CancelWaitAndReturnToIdleCoroutine();
            CancelHittingJudgeCoroutine();
        }
        
        private void CancelWaitAndReturnToIdleCoroutine()
        {
            if (_waitAndReturnToIdleCoroutine != null)
            {
                StopCoroutine(_waitAndReturnToIdleCoroutine);
            }
            _waitAndReturnToIdleCoroutine = null;
        }

        private void CancelHittingJudgeCoroutine()
        {
            hitting = false;
            if (_hittingDisableCoroutine != null)
            {
                StopCoroutine(_hittingDisableCoroutine);
                _hittingDisableCoroutine = null;
            }
        }

        // Virtual Functions

        /// <summary>
        /// This Entity Hit is triggered
        /// </summary>
        public void MyAttackHitted()
        {
            hitting = false;
        }
        
        /// <summary>
        /// Called When Game's Time Blocked
        /// </summary>
        public virtual void TimeBlocked()
        {
            animator.speed = 0.0f;
            if (_waitAndReturnToIdleCoroutine != null)
                StopCoroutine(_waitAndReturnToIdleCoroutine);
            if (_hittingDisableCoroutine != null)
                StopCoroutine(_hittingDisableCoroutine);
        }

        /// <summary>
        /// Called When Game's Time Resumed
        /// </summary>
        public virtual void TimeResumed()
        {
            animator.speed = 1.0f;
            if (_waitAndReturnToIdleCoroutine != null)
                StartCoroutine(_waitAndReturnToIdleCoroutine);
            if (_hittingDisableCoroutine != null)
                StartCoroutine(_hittingDisableCoroutine);
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
            attackHitBox.WhenShow();
            chargeAttackHitBox.WhenShow();
        }

        public void Hide()
        {
            attackHitBox.WhenHide();
            chargeAttackHitBox.WhenHide();
            gameObject.SetActive(false);
        }
        
        // Animation 시간 이름으로 받아오기
        public float GetAnimationTime(string animationName)
        {
            return AnimatorUtil.GetAnimationTime(animationName, animator.runtimeAnimatorController);
        }

        public void InitHp(int mhp)
        {
            currentHp = mhp;
            maxHp = mhp;
        }
        
        public virtual void ResetAfterDead()
        {
            attackHitBox.ResetInRange();
            chargeAttackHitBox.ResetInRange();
            CancelAllCoroutine();
        }

        /// <summary>
        /// Return If Entity is Dead After Damaged
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public virtual bool Damaged(int damage)
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

        /// <summary>
        /// Return If Action Success
        /// </summary>
        /// <param name="combatEntityStatus"></param>
        /// <returns></returns>
        public virtual bool EntityAction(CombatEntityStatus combatEntityStatus)
        {
            // Animation 처리
            SwitchStatusAndAnimation(combatEntityStatus);
            currentStatus = combatEntityStatus;
            // 변수 및 Coroutine 처리
            switch (combatEntityStatus)
            {
                // 차지 - 계속해서 차지
                case CombatEntityStatus.Charge:
                    return true;
                // 공격
                case CombatEntityStatus.Attack:
                    // Hitting 변수
                    WaitAndReturnToIdleWithOperation(GetAnimationTime("Attack"));
                    StartHitJudgeAndEndAfter(attackHitDuration);
                    return true;
                // 차지 공격
                case CombatEntityStatus.ChargeAttack:
                    WaitAndReturnToIdleWithOperation(GetAnimationTime("ChargeAttack"));
                    StartHitJudgeAndEndAfter(chargeAttackHitDuration);
                    return true;
                // 움직임
                case CombatEntityStatus.Running:
                    return true;
                case CombatEntityStatus.JumpBack:
                    WaitAndReturnToIdleWithOperation(GetAnimationTime("JumpBack"));
                    return true;
            }
            return false;
        }
    }
}