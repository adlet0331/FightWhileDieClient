using System.Collections;
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
        AttackSlow = 2,
        Running = 3,
        Damaged = 4,
        JumpBack = 5,
        Dead = 6,
    }
    public enum ObjectType
    {
        Player = 0,
        AI = 1
    }
    public class CombatEntity : MonoBehaviour
    {
        [Header("Initial Setting")]
        [SerializeField] private int maxHp;
        [SerializeField] private float attackDelay;
        [SerializeField] private float attackEnd;
        [SerializeField] private float attackAfterDelay;
        [SerializeField] private ObjectType type;

        [Header("If enemyAI, Need to be Initialized")] 
        [SerializeField] public float runningSpeed;
        [SerializeField] public float backJumpSpeed;
        [SerializeField] public float knockBackXInterval;

        public bool EnemyInRange => attackHitBox.EnemyInRange;
        // Current Status
        public bool Attacking => attacking;
        public bool Hitting => hitting;
        public bool Dying => dying;
        // Only For enemyAI
        public bool Running => running;
        public bool BackJumping => backJumping;
        public bool Damaging => damaging;

        [Header("Debugging")] 
        [SerializeField] public ObjectStatus currentStatus;
        [SerializeField] private int currentHp;
        [SerializeField] private bool attacking;
        [SerializeField] private bool hitting;
        [SerializeField] private bool dying;
        [Header("Debugging Only For Enemy")]
        [SerializeField] private bool running;
        [SerializeField] private bool backJumping;
        [SerializeField] private bool damaging;

        [Header("Set In Unity")]
        [SerializeField] private Animator animator;
        [SerializeField] private AttackRangeObject attackHitBox;

        #region Start
        private void Start()
        {
            currentHp = maxHp;

            // Coroutines
            _waitAndReturnToIdleCoroutine = null;
            _hittingJudgeCoroutine = null;
            
            currentStatus = ObjectStatus.Idle;
        }
        #endregion

        #region IEnumerator

        public void StopAllCoroutines()
        {
            if (_waitAndReturnToIdleCoroutine != null)
                StopCoroutine(_waitAndReturnToIdleCoroutine);
            if (_hittingJudgeCoroutine != null)
                StopCoroutine(_hittingJudgeCoroutine);
        }

        public void RestartAllCoroutines()
        {
            if (_waitAndReturnToIdleCoroutine != null)
                StartCoroutine(_waitAndReturnToIdleCoroutine);
            if (_hittingJudgeCoroutine != null)
                StartCoroutine(_hittingJudgeCoroutine);
        }
        
        // 기다렸다가 IDLE로 전환
        private IEnumerator _waitAndReturnToIdleCoroutine;
        // 공격 판정변수 설정
        private IEnumerator _hittingJudgeCoroutine;
        private delegate void OperationWaitAndReturnToIdle();
        private void WaitAndReturnToIdleWithOperation(float sec, OperationWaitAndReturnToIdle operation = null)
        {
            CancelWaitAndReturnToIdleCoroutine();
            _waitAndReturnToIdleCoroutine = CoroutineUtils.WaitAndOperationIEnum(sec, () =>
            {
                Action(ObjectStatus.Idle);
                _waitAndReturnToIdleCoroutine = null;
                operation?.Invoke();
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
        private void WaitAndHandleHitting()
        {
            CancelHittingJudgeCoroutine();
            _hittingJudgeCoroutine = CoroutineUtils.WaitAndOperationIEnum(attackDelay, () =>
            {
                hitting = true;
                _hittingJudgeCoroutine = null;
            });
            StartCoroutine(_hittingJudgeCoroutine);
            StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(attackEnd, () => { hitting = false; }));
        }

        public void CancelHittingJudgeCoroutine()
        {
            hitting = false;
            if (_hittingJudgeCoroutine != null)
            {
                StopCoroutine(_hittingJudgeCoroutine);
                _hittingJudgeCoroutine = null;
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
            SwitchStatus(ObjectStatus.Idle);
        }
        
        public void SetHp(int mhp)
        {
            currentHp = mhp;
            maxHp = mhp;
        }

        private void SwitchStatus(ObjectStatus newStatus)
        {
            if (currentStatus == newStatus) return;
            attacking = false;
            damaging = false;
            dying = false;
            running = false;
            backJumping = false;
            animator.SetBool(currentStatus.ToString(), false);
            
            currentStatus = newStatus;
            
            switch (currentStatus)
            {
                case ObjectStatus.Attack:
                    attacking = true;
                    break;
                case ObjectStatus.AttackSlow:
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

        public bool Damaged(int damage)
        {
            CancelHittingJudgeCoroutine();
            
            currentHp = currentHp - damage > 0 ? currentHp - damage : 0;
            UIManager.Instance.UpdateEnemyHp((float)currentHp / maxHp);
            if (currentHp == 0)
            {
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
            // 공격 중이면 Action 안 받음
            if (hitting) return;
            
            // Player Action WhiteList
            if (type == ObjectType.Player &&
                (objectStatus != ObjectStatus.Idle && objectStatus != ObjectStatus.Attack &&
                 objectStatus != ObjectStatus.Dead && objectStatus != ObjectStatus.Running))
            {
                Debug.Log("Player Action Rejected");
                return;
            }
            // enemyAI Action BlackList
            if (type == ObjectType.AI && 
                (objectStatus is ObjectStatus.Dead or ObjectStatus.Damaged))
            {
                Debug.Log("enemyAI Action Rejected");
                return;
            }

            switch (objectStatus)
            {
                // 공격
                case ObjectStatus.Attack:
                    // Hitting 변수
                    WaitAndHandleHitting();
                    // Idle로 리턴
                    WaitAndReturnToIdleWithOperation(attackEnd + attackAfterDelay);
                    break;
                // 죽음 (플레이어만)
                case ObjectStatus.Dead:
                    WaitAndReturnToIdleWithOperation(GetAnimationTime("Dead"));
                    break;
                // 아래는 enemyAI 만 사용
                // 느리게 공격
                case ObjectStatus.AttackSlow:

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
            
            // Animation 및 변수들 처리
            SwitchStatus(objectStatus);
        }
    }
}