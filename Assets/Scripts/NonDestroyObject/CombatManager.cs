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
        public CombatEntity player;
        public CombatEntity[] enemyAIList;
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

        public bool TimeBlocked
        {
            get => timeBlocked;
            set
            {
                timeBlocked = value;
                player.EnableAnimation(!timeBlocked);
                enemyAIList[currentEnemyIndex].EnableAnimation(!timeBlocked);
                // 시간이 멈춤
                if (value)
                {
                    if (_afterDeadCoroutine != null)
                    {
                        StopCoroutine(_afterDeadCoroutine);
                    }
                    player.StopAllCoroutines();
                    enemyAIList[currentEnemyIndex].StopAllCoroutines();
                }
                else
                {
                    if (_afterDeadCoroutine != null)
                    {
                        StartCoroutine(_afterDeadCoroutine);
                    }
                    player.RestartAllCoroutines();
                    enemyAIList[currentEnemyIndex].RestartAllCoroutines();
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
            enemyAIList[currentEnemyIndex].Hide();
            currentEnemyIndex = _random.Next(0, enemyAIList.Length);
            enemyAIList[currentEnemyIndex].Show();
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
                
                if (!player.Attacking && !player.Dying)
                {
                    player.Action(ObjectStatus.Attack);
                }
            }
            
            // 공격 액션
            if (!inputBlocked && !player.Hitting && !player.Attacking && !player.Dying)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    float initrandomtime = _random.Next(0, (int)(strongAttackHoldRandomMaxTime * 100)) * 0.01f;
                    holdTime = initrandomtime;
                    player.Action(ObjectStatus.Charge);
                    isHolded = true;
                    holdSliderUI.value = holdTime;
                }
                if (Input.GetMouseButton(0) && isHolded)
                {
                    holdTime += Time.deltaTime;
                    holdSliderUI.value = Math.Min(holdTime, 1.0f);
                }
                if (isHolded && holdTime > 0 && Input.GetMouseButtonUp(0))
                {
                    holdSliderUI.value = 0;
                    if (strongAttackHoldTime - perfectAttackTimeInterval <= holdTime && holdTime <= strongAttackHoldTime + perfectAttackTimeInterval)
                    {
                        player.Action(ObjectStatus.PerfectChargeAttack);
                    }
                    else if (holdTime < strongAttackHoldTime)
                    {
                        player.Action(ObjectStatus.Attack);
                    }
                    else
                    {
                        player.Action(ObjectStatus.ChargeAttack);
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
            if (timeBlocked) return;

            #region CheckHitting
            // enemyAI 피격 판정 우선 - 동시에 때리면 플레이어 판정이 우세 
            if ((player.AttackType == AttackType.Normal && player.Hitting && player.EnemyInRange) ||
                (player.AttackType == AttackType.Charge && player.Hitting && player.EnemyInChargeRange) ||
                (player.AttackType == AttackType.PerfectCharge && player.Hitting && player.EnemyInPerfectChargeRange))
            {
                player.CancelHittingJudgeCoroutine();
                enemyAIList[currentEnemyIndex].CancelHittingJudgeCoroutine();

                int damage = (int)(DataManager.Instance.playerDataManager.Atk * playerDamagePerAttackRate[(int)player.AttackType]);
                
                var dead = enemyAIList[currentEnemyIndex].Damaged(damage);
                if (dead)
                {
                    // 코인 이펙트
                    UIManager.Instance.ShowCoinEffect();
                    var delay = enemyAIList[currentEnemyIndex].GetAnimationTime("Dead");
                    _afterDeadCoroutine = CoroutineUtils.WaitAndOperationIEnum(delay, () =>
                    {
                        EndCombat(true);
                        // player.ResetAfterDie();
                        enemyAIList[currentEnemyIndex].ResetAfterDie();
                        _afterDeadCoroutine = null;
                    });
                    StartCoroutine(_afterDeadCoroutine);
                    // Update enemyAI[currentEnemyIndex]'s Random Seed
                    UpdateRandomSeed();
                }

                return;
            }

            // Player 피격 판정
            if (enemyAIList[currentEnemyIndex].Hitting && enemyAIList[currentEnemyIndex].EnemyInRange)
            {
                player.CancelHittingJudgeCoroutine();
                enemyAIList[currentEnemyIndex].CancelHittingJudgeCoroutine();
                
                // Player's Hp is always 1
                var dead = player.Damaged(1);
                if (dead)
                {
                    var delay = player.GetAnimationTime("Dead");
                    _afterDeadCoroutine = CoroutineUtils.WaitAndOperationIEnum(delay, () =>
                    {
                        EndCombat(false);
                        player.ResetAfterDie();
                        enemyAIList[currentEnemyIndex].ResetAfterDie();
                        _afterDeadCoroutine = null;
                    });
                    StartCoroutine(_afterDeadCoroutine);
                }

                return;
            }
            #endregion
            
            if (inputBlocked) return;

            // enemyAI[currentEnemyIndex] Transform Moving
            if (enemyAIList[currentEnemyIndex].Damaging)
            {
                enemyAIList[currentEnemyIndex].transform.position += Vector3.right * (enemyAIList[currentEnemyIndex].knockBackXInterval * Time.fixedDeltaTime);
            }
            else if (enemyAIList[currentEnemyIndex].Running)
            {
                enemyAIList[currentEnemyIndex].transform.position += Vector3.left * (enemyAIList[currentEnemyIndex].runningSpeed * Time.fixedDeltaTime);
            }
            else if (enemyAIList[currentEnemyIndex].BackJumping)
            {
                enemyAIList[currentEnemyIndex].transform.position += Vector3.right * (enemyAIList[currentEnemyIndex].backJumpSpeed * Time.fixedDeltaTime);
            }

            if (updateDelayed || enemyAIList[currentEnemyIndex].Attacking || enemyAIList[currentEnemyIndex].Damaging || enemyAIList[currentEnemyIndex].BackJumping || enemyAIList[currentEnemyIndex].Dying) return;
            
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
            if (enemyAIList[currentEnemyIndex].EnemyInRange)
            {
                int rand = _random.Next(1, 100);
                if (rand <= _random.Next(1, 100))
                {
                    enemyAIList[currentEnemyIndex].Action(ObjectStatus.Attack);
                }
                else
                {
                    enemyAIList[currentEnemyIndex].Action(ObjectStatus.JumpBack);
                }
                UpdateRandomSeed();
            }
            else
            {
                enemyAIList[currentEnemyIndex].Action(ObjectStatus.Running);
            }
        }

        private void InitAiPos(bool playerDie)
        {
            if (playerDie)
                enemyAIList[currentEnemyIndex].transform.SetLocalPositionAndRotation(aiStandingPosition.localPosition, aiStandingPosition.rotation);
            else
                enemyAIList[currentEnemyIndex].transform.SetLocalPositionAndRotation(aiStartPosition.localPosition, aiStartPosition.rotation);
        }

        private void InitPlayerEnemyHp()
        {
            player.SetHp(1);
            enemyAIList[currentEnemyIndex].SetHp(DataManager.Instance.playerDataManager.CurrentEnemyHp);
        }
        
        public void StartCombat()
        {
            DataManager.Instance.playerDataManager.StageReset();
            InitPlayerEnemyHp();

            IsInCombat = true;
            inputBlocked = false;
            InitAiPos(false);
            UIManager.Instance.TitleEnemyHpSwitch(true);
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
                UIManager.Instance.TitleEnemyHpSwitch(false);
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