using System.Collections;
using NonDestroyObject;
using Unity.VisualScripting;
using UnityEngine;

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
        [SerializeField] private float knockBackXInterval;
        [SerializeField] private float knockBackTime;

        public int MaxHp => maxHp;
        public bool EnemyInRange => attackHitBox.EnemyInRange;
        public bool Attacking => attacking;
        public bool Damaging => damaging;

        [Header("Debuging")] 
        [SerializeField] private int currentHp;
        [SerializeField] private bool attacking;
        [SerializeField] private bool attackedAlready;
        [SerializeField] private bool damaging;

        [SerializeField] private Animator animator;
        [SerializeField] private BoxCollider2D hitBox;
        [SerializeField] private AttackRangeObject attackHitBox;
        [SerializeField] private RuntimeAnimatorController runtimeAnimatorController;

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
                transform.localPosition -= Vector3.right * knockBackXInterval;
                return;
            }
            
            if (!attacking || attackedAlready) return;

            if (attackHitBox.EnemyInRange)
            {
                attackedAlready = true;
                if (type == ObjectType.AI)
                {
                    PlayerManager.Instance.Player.Action(ObjectStatus.Dead);
                }

                if (type == ObjectType.Player)
                {
                    CombatManager.Instance.AI.Damaged(PlayerManager.Instance.Player.atk);
                }
            }
        }

        // Animation 시간 이름으로 받아오기
        private float GetAnimationTime(string name)
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
        
        // 기다렸다가 IDLE로 전환
        private IEnumerator _waitAfterAction;
        private IEnumerator WaitAndReturnToIdleIEnum(float second)
        {
            yield return new WaitForSeconds(second);
            PlayerManager.Instance.Player.Action(ObjectStatus.Idle);
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

        // 기다렸다가 Bool 교체 
        private delegate void ChangeBoolValue(bool changeVal);
        private IEnumerator WaitAndChangeBoolValue(float sec, ChangeBoolValue operation, bool changeVal)
        {
            yield return new WaitForSeconds(sec);
            operation(changeVal);
        }
        
        // 공격 판정변수 설정
        private void WaitAndChangeAttacking(float start, float end)
        {
            StartCoroutine(WaitAndChangeBoolValue(start, (bool ch) => { attacking = ch; }, true));
            StartCoroutine(WaitAndChangeBoolValue(end, (bool ch) => { attacking = ch; }, false));
        }
        
        public void Damaged(int damage)
        {
            attacking = false;
            attackedAlready = false;
            currentHp = currentHp - damage > 0 ? currentHp - damage : 0;
            UIManager.Instance.UpdateEnemyHp((float)currentHp / MaxHp);
            // 죽음
            if (currentHp == 0)
            {
                animator.SetBool("Death", true);
                WaitAndReturnToIdle(GetAnimationTime("Dead"));
            }
            // 안 죽고 넉백 당함
            else
            {
                animator.SetBool("Damaged", true);
                StartCoroutine(WaitAndChangeBoolValue(knockBackTime, (bool ch) => { damaging = ch; }, true));
            }
        }
        
        public void Action(ObjectStatus objectStatus)
        {
            // Player Action WhiteList
            if (type == ObjectType.Player &&
                (objectStatus != ObjectStatus.Idle && objectStatus != ObjectStatus.Attack &&
                 objectStatus != ObjectStatus.Dead))
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
                    animator.SetBool("Death", false);
                    if (type is ObjectType.Player) return;
                    
                    animator.SetBool("AttackSlow", false);
                    animator.SetBool("Running", false);
                    animator.SetBool("JumpBack", false);
                    return;
                // 공격
                case ObjectStatus.Attack:
                    animator.SetBool("Attack", true);
                    WaitAndReturnToIdle(attackEnd + attackAfterDelay);
                    WaitAndChangeAttacking(attackBeforeDelay, attackEnd);
                    return;
                // 죽음 (플레이어만)
                case ObjectStatus.Dead:
                    animator.SetBool("Death", true);
                    WaitAndReturnToIdle(GetAnimationTime("Dead"));
                    return;
                // 아래는 AI 만 사용
                // 느리게 공격
                case ObjectStatus.AttackSlow:

                    return;
                // 움직임
                case ObjectStatus.Running:
                    animator.SetBool("Running", true);
                    return;
                case ObjectStatus.JumpBack:

                    return;
            }
            
        }
    }
}