using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Random = System.Random;

namespace NonDestroyObject
{
    public class CombatManager : Singleton<CombatManager>
    {
        [Header("Need Initialize In Unity")] 
        public PlayerEntity player;
        public EnemyEntity[] enemyAIList;
        [SerializeField] private Transform aiStartPosition;
        [SerializeField] private Transform aiStandingPosition;
        [SerializeField] private int currentEnemyIndex;
        [SerializeField] private float strongAttackHoldTime;
        [SerializeField] private float strongAttackHoldRandomMaxTime;
        [SerializeField] private float perfectAttackTimeInterval;
        [SerializeField] private Slider holdSliderUI;
        // [AttackType] per Rate
        [SerializeField] private List<float> playerDamagePerAttackRate;
        [Header("Status")]
        [SerializeField] private bool inputBlocked;
        [SerializeField] private bool timeBlocked;
        [SerializeField] private bool updateDelayed;
        [SerializeField] private float updateInterval;
        [SerializeField] private int randomSeed;
        [SerializeField] private bool isHolded;
        [SerializeField] private float holdTime;
        [Header("Auto Status")]
        [SerializeField] private float afterEndCombat;

        public bool IsInCombat { get; private set; }


        public EnemyEntity CurrentEnemyEntity
        {
            get => enemyAIList[currentEnemyIndex];
        }
        public bool TimeBlocked
        {
            get => timeBlocked;
            set
            {
                timeBlocked = value;
                // 시간이 멈춤
                if (value)
                {
                    if (_afterDeadCoroutine != null)
                    {
                        StopCoroutine(_afterDeadCoroutine);
                    }
                    player.TimeBlocked();
                    CurrentEnemyEntity.TimeBlocked();
                }
                else
                {
                    if (_afterDeadCoroutine != null)
                    {
                        StartCoroutine(_afterDeadCoroutine);
                    }
                    player.TimeResumed();
                    CurrentEnemyEntity.TimeResumed();
                }
            }
        }

        private Random _random;
        private void Start()
        {
            IsInCombat = false;
            inputBlocked = true;
            // Input isHolded
            isHolded = false;
            // Coroutines
            _afterDeadCoroutine = null;
            // Random
            randomSeed = DateTime.Now.Millisecond * 103729;
            _random = new Random(randomSeed);
            UpdateRandomEnemyAI();
            UpdateRandomSeed();
        }

        private void UpdateRandomEnemyAI()
        {
            CurrentEnemyEntity.Hide();
            currentEnemyIndex = _random.Next(0, enemyAIList.Length);
            CurrentEnemyEntity.Show();
        }
        
        // Player Input
        private void Update()
        {
            // Spawn Particle - 언제나 터치 파티클은 보여주기
            if (Input.GetMouseButtonDown(0))
            {
                var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                var clickParticle = Resources.Load("Prefabs/UI/TouchParticle") as GameObject;
                Instantiate(clickParticle, mousePosition, new Quaternion(0, 0, 0, 0));
            }

            // Pause 때 Action 막기
            if (timeBlocked) return;

            // Auto 켰을 때 자동조작
            if (AutoManager.Instance.IsAuto)
            {
                // Skip Delay At First Pressed
                if (AutoManager.Instance.IsFirst)
                {
                    afterEndCombat = 3.0f;
                }
                
                if (afterEndCombat < 3.0f)
                {
                    afterEndCombat += Time.deltaTime;
                    return;
                }
                
                if (!IsInCombat)
                {
                    StartCombat();
                }
                // TODO: 오토 켰을 때 플레이어 액션 메커니즘 넣어야함
            }
            
            // 공격 액션
            if (!inputBlocked && !player.Attacking && player.CurrentStatus != CombatEntityStatus.Dying)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    float initrandomtime = _random.Next(0, (int)(strongAttackHoldRandomMaxTime * 100)) * 0.01f;
                    holdTime = initrandomtime;
                    player.EntityAction(CombatEntityStatus.Charge);
                    isHolded = true;
                    UIManager.Instance.UpdateAttackGageSlider(holdTime);
                }
                if (Input.GetMouseButton(0) && isHolded)
                {
                    holdTime += Time.deltaTime;
                    UIManager.Instance.UpdateAttackGageSlider(Math.Min(holdTime, 1.0f));
                }
                if (isHolded && holdTime > 0 && Input.GetMouseButtonUp(0))
                {
                    UIManager.Instance.UpdateAttackGageSlider(0);
                    if (strongAttackHoldTime - perfectAttackTimeInterval <= holdTime && holdTime <= strongAttackHoldTime + perfectAttackTimeInterval)
                    {
                        player.EntityAction(CombatEntityStatus.PerfectChargeAttack);
                    }
                    else if (holdTime < strongAttackHoldTime)
                    {
                        player.EntityAction(CombatEntityStatus.Attack);
                    }
                    else
                    {
                        player.EntityAction(CombatEntityStatus.ChargeAttack);
                    }
                    isHolded = false;
                }
            }
        }

        private IEnumerator _delayedCoroutine;
        private IEnumerator UpdateDelay(float second)
        {
            yield return new WaitForSeconds(second);
            updateDelayed = false;
            _delayedCoroutine = null;
        }

        private void FixedUpdate()
        {
            if (timeBlocked || inputBlocked) return;

            #region CheckHitting
            // enemyAI 피격 판정 우선 - 동시에 때리면 플레이어 판정이 우세 
            if ((player.AttackType == AttackType.Normal && player.Hitting && player.OpponentInRange) ||
                (player.AttackType == AttackType.Charge && player.Hitting && player.EnemyInChargeRange) ||
                (player.AttackType == AttackType.PerfectCharge && player.Hitting && player.EnemyInPerfectChargeRange))
            {
                player.MyAttackHitted();
                int damage = (int)(DataManager.Instance.playerDataManager.Atk * playerDamagePerAttackRate[(int)player.AttackType]);
                
                var dead = CurrentEnemyEntity.Damaged(damage);
                if (dead)
                {
                    // 코인 이펙트
                    UIManager.Instance.ShowCoinEffect();
                    var delay = CurrentEnemyEntity.GetAnimationTime("Dying");
                    _afterDeadCoroutine = CoroutineUtils.WaitAndOperationIEnum(delay, () =>
                    {
                        EndCombat(true);
                        _afterDeadCoroutine = null;
                    });
                    StartCoroutine(_afterDeadCoroutine);
                    // Update enemyAI[currentEnemyIndex]'s Random Seed
                    UpdateRandomSeed();
                }
                UIManager.Instance.UpdateEnemyHp(CurrentEnemyEntity.CurrentHpRatio);
                return;
            }

            // Player 피격 판정
            if (CurrentEnemyEntity.Hitting && CurrentEnemyEntity.OpponentInRange)
            {
                // Player's Hp is always 1
                var dead = player.Damaged(1);
                if (dead)
                {
                    var delay = player.GetAnimationTime("Dying");
                    _afterDeadCoroutine = CoroutineUtils.WaitAndOperationIEnum(delay, () =>
                    {
                        EndCombat(false);
                        _afterDeadCoroutine = null;
                    });
                    StartCoroutine(_afterDeadCoroutine);
                }

                return;
            }
            #endregion

            // enemyAI[currentEnemyIndex] Transform Moving
            if (CurrentEnemyEntity.CurrentStatus == CombatEntityStatus.Damaged)
            {
                CurrentEnemyEntity.transform.position += Vector3.right * (CurrentEnemyEntity.knockBackXInterval * Time.fixedDeltaTime);
                return;
            }
            else if (CurrentEnemyEntity.CurrentStatus == CombatEntityStatus.Running)
            {
                CurrentEnemyEntity.transform.position += Vector3.left * (CurrentEnemyEntity.runningSpeed * Time.fixedDeltaTime);
                return;
            }
            else if (CurrentEnemyEntity.CurrentStatus == CombatEntityStatus.JumpBack)
            {
                CurrentEnemyEntity.transform.position += Vector3.right * (CurrentEnemyEntity.backJumpSpeed * Time.fixedDeltaTime);
                return;
            }

            if (CurrentEnemyEntity.CurrentStatus == CombatEntityStatus.Dying) return;
            
            updateDelayed = true;
            AIAction();
            if (_delayedCoroutine != null)
            {
                StopCoroutine(_delayedCoroutine);
            }
            _delayedCoroutine = UpdateDelay(updateInterval);
            StartCoroutine(_delayedCoroutine);
        }

        private void AIAction()
        {
            if (CurrentEnemyEntity.OpponentInRange)
            {
                int rand = _random.Next(1, 100);
                if (rand <= _random.Next(1, 100))
                {
                    CurrentEnemyEntity.EntityAction(CombatEntityStatus.Attack);
                }
                else
                {
                    CurrentEnemyEntity.EntityAction(CombatEntityStatus.JumpBack);
                }
                UpdateRandomSeed();
            }
            else
            {
                CurrentEnemyEntity.EntityAction(CombatEntityStatus.Running);
            }
        }

        private void InitAiPos(bool playerDie)
        {
            if (playerDie)
                CurrentEnemyEntity.transform.SetLocalPositionAndRotation(aiStandingPosition.localPosition, aiStandingPosition.rotation);
            else
                CurrentEnemyEntity.transform.SetLocalPositionAndRotation(aiStartPosition.localPosition, aiStartPosition.rotation);
        }

        private void InitPlayerEnemyHp()
        {
            player.InitHp(1);
            CurrentEnemyEntity.InitHp(DataManager.Instance.playerDataManager.CurrentEnemyHp);
        }
        
        public void StartCombat()
        {
            DataManager.Instance.playerDataManager.StageReset();
            InitPlayerEnemyHp();

            IsInCombat = true;
            inputBlocked = false;
            InitAiPos(false);
            UIManager.Instance.SwitchMainPage2CombatUI(true);
        }

        private void EndCombat(bool cleared)
        {
            if (cleared)
            {
                // 스테이지 클리어 처리
                UpdateRandomEnemyAI();
                DataManager.Instance.playerDataManager.StageCleared();
                InitAiPos(false);
            }
            else
            {
                IsInCombat = false;
                inputBlocked = true;
                UIManager.Instance.SwitchMainPage2CombatUI(false);
                DataManager.Instance.playerDataManager.StageReset();
                InitAiPos(true);
                afterEndCombat = 0.0f;
            }
            InitPlayerEnemyHp();
        }

        private void UpdateRandomSeed()
        {
            randomSeed = _random.Next();
            _random = new Random(randomSeed);
        }
        
        // IEnumerator
        private IEnumerator _afterDeadCoroutine;
    }
}