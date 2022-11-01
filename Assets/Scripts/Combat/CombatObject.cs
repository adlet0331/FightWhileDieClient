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
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class CombatObject : MonoBehaviour
    {
        [Header("Initial Setting")]
        [SerializeField] private int atk;
        [SerializeField] private int maxHp;
        [SerializeField] private float attackDelay;
        [SerializeField] private float attackEnd;
        [SerializeField] private float attackAfterDelay;
        [SerializeField] private ObjectType type;

        [Header("If AI, Need to be Initialized")] 
        [SerializeField] private float runningSpeed;
        [SerializeField] private float backJumpSpeed;
        [SerializeField] private float knockBackXInterval;
        [SerializeField] private float knockBackTime;
        
        public bool EnemyInRange => attackHitBox.EnemyInRange;
        public bool Attacking => attacking;
        public bool Dying => dying;
        // Only For AI
        public bool Running => running;
        public float RunningSpeed => runningSpeed;
        public bool BackJumping => backJumping;
        public bool Damaging => damaging;

        [Header("Debugging")] [SerializeField] public ObjectStatus currentStatus;
        [SerializeField] private int currentHp;
        [SerializeField] private bool attacking;
        [SerializeField] private bool hitting;
        [SerializeField] private bool dying;
        [Header("Debugging Only For Enemy")]
        [SerializeField] private bool running;
        [SerializeField] private bool backJumping;
        [SerializeField] private bool damaging;

        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private BoxCollider2D hitBox;
        [SerializeField] private AttackRangeObject attackHitBox;
        [SerializeField] private RuntimeAnimatorController runtimeAnimatorController;

        #region Start, FixedUpdate
        private void Start()
        {
            if (animator.IsUnityNull())
            {
                animator = GetComponent<Animator>();
            }

            if (hitBox.IsUnityNull())
            {
                hitBox = GetComponent<BoxCollider2D>();
            }

            if (attackHitBox.IsUnityNull())
            {
                attackHitBox = GetComponentInChildren<AttackRangeObject>();
            }

            currentHp = maxHp;
            runtimeAnimatorController = animator.runtimeAnimatorController;

            _waitAfterAction = null;
            _blockInput = null;

            SLManager.Instance.Load();
            if (type == ObjectType.Player)
            {
                atk = SLManager.Instance.ATK;
                maxHp = 1;
            }

            if (type == ObjectType.AI)
            {
                atk = 1;
                maxHp = SLManager.Instance.EnemyHp;
            }
            currentStatus = ObjectStatus.Idle;
        }

        private void FixedUpdate()
        {
            if (damaging)
            {
                transform.position += Vector3.right * (knockBackXInterval * Time.fixedDeltaTime);
            }
            else if (running)
            {
                transform.position += Vector3.left * (runningSpeed * Time.fixedDeltaTime);
            }
            else if (backJumping)
            {
                transform.position += Vector3.right * (backJumpSpeed * Time.fixedDeltaTime);
            }

            if (attackHitBox.EnemyInRange && hitting)
            {
                hitting = false;
                if (type == ObjectType.AI)
                {
                    if (_hittingJudgeAction != null)
                    {
                        StopCoroutine(_hittingJudgeAction);
                        _hittingJudgeAction = null;
                    }
                    
                    PlayerManager.Instance.Player.Action(ObjectStatus.Dead);

                    StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(GetAnimationTime("Dead"),
                        () =>
                        {
                            PlayerManager.Instance.PlayerDie();
                        }));
                }
                else if (type == ObjectType.Player)
                {
                    CombatManager.Instance.AI.Damaged(PlayerManager.Instance.Player.atk);
                }
            }
        }
        #endregion

        #region IEnumerator
        // 기다렸다가 IDLE로 전환
        private IEnumerator _waitAfterAction;
        // Input Block 용
        private IEnumerator _blockInput;
        private IEnumerator WaitAndReturnToIdleIEnum(float second)
        {
            yield return new WaitForSeconds(second);
            Action(ObjectStatus.Idle);
            _waitAfterAction = null;
        }
        private void WaitAndReturnToIdle(float sec)
        {
            if (_waitAfterAction != null)
            {
                StopCoroutine(_waitAfterAction);
            }
            _waitAfterAction = WaitAndReturnToIdleIEnum(sec);
            StartCoroutine(_waitAfterAction);
        }

        // 공격 판정변수 설정
        private IEnumerator _hittingJudgeAction;
        private void WaitAndChangeHitting()
        {
            _hittingJudgeAction = CoroutineUtils.WaitAndOperationIEnum(attackDelay, () => { hitting = true; });
            StartCoroutine(_hittingJudgeAction);
            StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(attackEnd, () => { hitting = false; }));
        }
        #endregion
        
        // Animation 시간 이름으로 받아오기
        public float GetAnimationTime(string animationName)
        {
            foreach (var t in runtimeAnimatorController.animationClips)
            {
                if(t.name == animationName) //If it has the same animationName as your clip
                {
                    return t.length; // Because For Delay
                }
            }

            Debug.LogAssertion("No Animation Named " + animationName);
            return 0.0f;
        }

        public void ResetAfterDie()
        {
            attackHitBox.ResetInRange();
            if (_blockInput != null)
            {
                StopCoroutine(_blockInput);
                _blockInput = null;
            }
            if (_hittingJudgeAction != null)
            {
                StopCoroutine(_hittingJudgeAction);
                _hittingJudgeAction = null;
            }
            if (_waitAfterAction != null)
            {
                StopCoroutine(_waitAfterAction);
                _waitAfterAction = null;
                // 플레이어 공격 모션 캔슬 안 되기 위함
                if (type == ObjectType.Player)
                {
                    StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(attackAfterDelay, () =>
                    {
                        attacking = false;
                        SwitchStatus(ObjectStatus.Idle);
                    }));
                    return;
                }
            }
            SwitchStatus(ObjectStatus.Idle);
        }
        
        public void UpdateStatus(int mhp, int at)
        {
            currentHp = mhp;
            maxHp = mhp;
            atk = at;
        }

        private void SwitchStatus(ObjectStatus newStatus)
        {
            if (currentStatus == newStatus) return;
            switch (currentStatus)
            {
                case ObjectStatus.Attack:
                    attacking = false;
                    break;
                case ObjectStatus.AttackSlow:
                    
                    break;
                case ObjectStatus.Damaged:
                    damaging = false;
                    break;
                case ObjectStatus.Dead:
                    dying = false;
                    break;
                case ObjectStatus.Running:
                    running = false;
                    break;
                case ObjectStatus.JumpBack:
                    backJumping = false;
                    break;
            }
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

        private void BlockInput(float sec, CoroutineUtils.AfterWaitOperation operation)
        {
            if (type == ObjectType.AI)
            {
                CombatManager.Instance.Blocked = true;
            }
            else if (type == ObjectType.Player)
            {
                InputManager.Instance.Blocked = true;
            }
            
            if (_blockInput != null) StopCoroutine(_blockInput);
            _blockInput = CoroutineUtils.WaitAndOperationIEnum(sec, () =>
            {
                if (type == ObjectType.AI)
                {
                    CombatManager.Instance.Blocked = false;
                }
                else if (type == ObjectType.Player)
                {
                    InputManager.Instance.Blocked = false;
                }

                _blockInput = null;
                operation();
            });
            StartCoroutine(_blockInput);
        }

        private void Damaged(int damage)
        {
            if (_hittingJudgeAction != null)
            {
                StopCoroutine(_hittingJudgeAction);
                hitting = false;
            }
            
            currentHp = currentHp - damage > 0 ? currentHp - damage : 0;
            UIManager.Instance.UpdateEnemyHp((float)currentHp / maxHp);
            // 죽음
            if (currentHp == 0)
            {
                PlayerManager.Instance.Player.ResetAfterDie();
                ResetAfterDie();

                StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(GetAnimationTime("Dead"), () =>
                {
                    dying = false;
                    CombatManager.Instance.AIDie();
                }));
                
                SwitchStatus(ObjectStatus.Dead);
            }
            // 안 죽고 넉백 당함
            else
            {
                WaitAndReturnToIdle(knockBackTime);
                UIManager.Instance.UpdateEnemyHp((float)currentHp / maxHp);
                
                BlockInput(knockBackTime, () =>
                {
                    damaging = false;
                });
                SwitchStatus(ObjectStatus.Damaged);
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
            // AI Action BlackList
            if (type == ObjectType.AI && 
                (objectStatus is ObjectStatus.Dead or ObjectStatus.Damaged))
            {
                Debug.Log("AI Action Rejected");
                return;
            }

            switch (objectStatus)
            {
                // 공격
                case ObjectStatus.Attack:
                    // 공격중에는 액션 안 받음
                    BlockInput(attackEnd + attackAfterDelay, () =>
                    {
                        attacking = false;
                    });
                    // Hitting 변수
                    WaitAndChangeHitting();
                    // Idle로 리턴
                    WaitAndReturnToIdle(attackEnd + attackAfterDelay);
                    break;
                // 죽음 (플레이어만)
                case ObjectStatus.Dead:
                    WaitAndReturnToIdle(GetAnimationTime("Dead"));
                    break;
                // 아래는 AI 만 사용
                // 느리게 공격
                case ObjectStatus.AttackSlow:

                    break;
                // 움직임
                case ObjectStatus.Running:
                    float time = type == ObjectType.AI
                        ? CombatManager.Instance.updateInterval
                        : StageMoveManager.Instance.UIMovingTime;
                    // Running 변수
                    WaitAndReturnToIdle(time);
                    break;
                case ObjectStatus.JumpBack:
                    float animationTime = GetAnimationTime("JumpBack");
                    StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(animationTime, () => { backJumping = false; }));
                    WaitAndReturnToIdle(animationTime);
                    break;
            }
            
            // Animation 및 변수들 처리
            SwitchStatus(objectStatus);
        }
    }
}