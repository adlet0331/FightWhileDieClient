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
        [SerializeField] private float attackBeforeDelay;
        [SerializeField] private float attackEnd;
        [SerializeField] private float attackAfterDelay;
        [SerializeField] private ObjectType type;

        [Header("If AI, Need to be Initialized")] 
        [SerializeField] private float runningSpeed;
        [SerializeField] private float backJumpSpeed;
        [SerializeField] private float knockBackXInterval;
        [SerializeField] private float knockBackTime;

        public int MaxHp => maxHp;
        public bool EnemyInRange => attackHitBox.EnemyInRange;
        public bool Attacking => attacking;
        public bool Dying => dying;
        // Only For AI
        public bool Running => running;
        public float RunningSpeed => runningSpeed;
        public bool BackJumping => backJumping;
        public bool Damaging => damaging;

        [Header("Debuging")] 
        [SerializeField] private int currentHp;
        [SerializeField] private bool attacking;
        [SerializeField] private bool hitting;
        [SerializeField] private bool dying;
        [Header("Debuging Only For Enemy")]
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
            _waitAfterRunningAction = null;

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
                    hitting = false;
                    attacking = false;
                    running = false;
                    backJumping = false;
                    damaging = false;
                    if (_hittingJudgeAction != null)
                    {
                        StopCoroutine(_hittingJudgeAction);
                    }
                    
                    PlayerManager.Instance.Player.Action(ObjectStatus.Dead);
                    
                    InputManager.Instance.Blocked = true;
                    CombatManager.Instance.Blocked = true;
                    
                    StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(GetAnimationTime("Dead"),
                        () =>
                        {
                            PlayerManager.Instance.PlayerDie();
                        }));
                }

                if (type == ObjectType.Player)
                {
                    CombatManager.Instance.AI.Damaged(PlayerManager.Instance.Player.atk);
                }
            }
        }
        #endregion

        #region IEnumerator
        // 기다렸다가 IDLE로 전환
        private IEnumerator _waitAfterAction;
        // Running IEnumerator (달리는 것을 연속적으로 만들기 위해)
        private IEnumerator _waitAfterRunningAction;
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
        private void WaitAndChangeHitting(float start, float end)
        {
            _hittingJudgeAction = CoroutineUtils.WaitAndOperationIEnum(start, () => { hitting = true; });
            StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(start, () =>
            {
                hitting = true;
                _hittingJudgeAction = null;
            }));
            StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(end, () => { hitting = false; }));
        }
        #endregion
        
        // Animation 시간 이름으로 받아오기
        public float GetAnimationTime(string name)
        {
            for(int i = 0; i<runtimeAnimatorController.animationClips.Length; i++) //For all animations
            {
                if(runtimeAnimatorController.animationClips[i].name == name) //If it has the same name as your clip
                {
                    return runtimeAnimatorController.animationClips[i].length; // Because For Delay
                }
            }

            Debug.LogAssertion("No Animation Named " + name);
            return 0.0f;
        }

        public void ResetInRange()
        {
            attackHitBox.ResetInRange();
        }
        
        public void UpdateStatus(int mhp, int at)
        {
            currentHp = mhp;
            maxHp = mhp;
            atk = at;
        }
        
        public void Damaged(int damage)
        {
            hitting = false;
            attacking = false;
            running = false;
            backJumping = false;
            damaging = false;
            if (_hittingJudgeAction != null)
            {
                StopCoroutine(_hittingJudgeAction);
            }
            
            currentHp = currentHp - damage > 0 ? currentHp - damage : 0;
            UIManager.Instance.UpdateEnemyHp((float)currentHp / MaxHp);
            // 죽음
            if (currentHp == 0)
            {
                animator.SetBool("Dead", true);
                PlayerManager.Instance.Player.ResetInRange();
                ResetInRange();
                dying = true;
                
                float deadAnimTime = GetAnimationTime("Dead");
                WaitAndReturnToIdle(deadAnimTime);
                StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(deadAnimTime, () =>
                {
                    dying = false;
                    Action(ObjectStatus.Idle);
                    CombatManager.Instance.AIDie();
                }));
            }
            // 안 죽고 넉백 당함
            else
            {
                animator.SetBool("Damaged", true);
                damaging = true;
                StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(knockBackTime, () =>
                {
                    damaging = false;
                }));
                WaitAndReturnToIdle(knockBackTime);
                UIManager.Instance.UpdateEnemyHp((float)currentHp / maxHp);
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
                // Player, AI 둘 다 사용
                case ObjectStatus.Idle:
                    animator.SetBool("Attack", false);
                    animator.SetBool("Dead", false);
                    animator.SetBool("Running", false); 
                    if (type is ObjectType.Player) return;
                    
                    animator.SetBool("AttackSlow", false);
                    animator.SetBool("JumpBack", false);
                    animator.SetBool("Damaged", false);
                    return;
                // 공격
                case ObjectStatus.Attack:
                    // 공격중에는 추가적인 공격 안 받음
                    if (attacking) return;
                    animator.SetBool("Attack", true);
                    //Attacking 변수
                    attacking = true;
                    StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(attackEnd + attackAfterDelay, () => { attacking = false; }));
                    // Hitting 변수
                    WaitAndChangeHitting(attackBeforeDelay, attackEnd);
                    // Idle로 리턴
                    WaitAndReturnToIdle(attackEnd + attackAfterDelay);
                    return;
                // 죽음 (플레이어만)
                case ObjectStatus.Dead:
                    animator.SetBool("Dead", true);
                    WaitAndReturnToIdle(GetAnimationTime("Dead"));
                    return;
                // 아래는 AI 만 사용
                // 느리게 공격
                case ObjectStatus.AttackSlow:

                    return;
                // 움직임
                case ObjectStatus.Running:
                    animator.SetBool("Running", true);
                    float time = type == ObjectType.AI
                        ? CombatManager.Instance.updateInterval
                        : StageMoveManager.Instance.UIMovingTime;
                    // Running 변수
                    running = true;
                    if (_waitAfterRunningAction != null)
                    {
                        StopCoroutine(_waitAfterRunningAction);
                    }
                    // Combat Manager Update Interval
                    _waitAfterRunningAction = CoroutineUtils.WaitAndOperationIEnum(time, () => 
                    { 
                        running = false;
                        _waitAfterRunningAction = null;
                    });
                    StartCoroutine(_waitAfterRunningAction);
                    WaitAndReturnToIdle(time);
                    return;
                case ObjectStatus.JumpBack:
                    animator.SetBool("JumpBack", true);
                    float animationTime = GetAnimationTime("JumpBack");
                    // BackJumping 변수
                    backJumping = true;
                    StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(animationTime, () => { backJumping = false; }));
                    WaitAndReturnToIdle(animationTime);
                    return;
            }
            
        }
    }
}