using System;
using System.Collections;
using Combat;
using UnityEngine;
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

        [Header("Flags")]
        public bool isInCombat;
        [SerializeField] private int currentEnemyIndex;
        [Header("Debuging")]
        [SerializeField] private bool inputBlocked;
        [SerializeField] private bool timeBlocked;
        [SerializeField] public bool updateDelayed;
        [SerializeField] public float updateInterval;
        [SerializeField] private int randomSeed;

        public bool InputBlocked
        {
            get => inputBlocked;
            set
            {
                inputBlocked = value;
            }
        }
        
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
            isInCombat = false;
            inputBlocked = true;
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
            // Attack 중일 때는 막기
            if (timeBlocked || inputBlocked || player.Attacking || player.Dying) return;
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    player.Action(ObjectStatus.Attack);
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                player.Action(ObjectStatus.Attack);
            }
        }

        private IEnumerator _delayedCoroutine;
        private IEnumerator UpdateDelay(float second)
        {
            yield return new WaitForSeconds(second);
            updateDelayed = false;
        }

        private void FixedUpdate()
        {
            if (timeBlocked) return;
             
            // enemyAI[currentEnemyIndex] 피격 판정 우선. 
            if (player.Hitting && player.EnemyInRange)
            {
                player.CancelHittingJudgeCoroutine();
                var dead = enemyAIList[currentEnemyIndex].Damaged(DataManager.Instance.playerDataManager.Atk);
                if (dead)
                {
                    // Update enemyAI[currentEnemyIndex]'s Random Seed
                    UpdateRandomSeed();
                    // 코인 이펙트
                    UIManager.Instance.ShowCoinEffect();
                    _afterDeadCoroutine = CoroutineUtils.WaitAndOperationIEnum(enemyAIList[currentEnemyIndex].GetAnimationTime("Dead"), () =>
                    {
                        EndCombat(true);
                        player.ResetAfterDie();
                        enemyAIList[currentEnemyIndex].ResetAfterDie();
                        _afterDeadCoroutine = null;
                    });
                    StartCoroutine(_afterDeadCoroutine);
                }

                return;
            }

            // Player 피격 판정
            if (enemyAIList[currentEnemyIndex].Hitting && enemyAIList[currentEnemyIndex].EnemyInRange)
            {
                enemyAIList[currentEnemyIndex].CancelHittingJudgeCoroutine();
                // Player's Hp is always 1
                var dead = player.Damaged(1);
                if (dead)
                {
                    isInCombat = false;
                    inputBlocked = true;
                    _afterDeadCoroutine = CoroutineUtils.WaitAndOperationIEnum(player.GetAnimationTime("Dead"), () =>
                    {
                        EndCombat(false);
                        player.ResetAfterDie();
                        enemyAIList[currentEnemyIndex].ResetAfterDie();
                        _afterDeadCoroutine = null;
                    });
                    StartCoroutine(_afterDeadCoroutine);
                }
                else
                    Debug.LogAssertion("Player MUST DIE in first hit!!");
                
                return;
            }
            
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

            if (enemyAIList[currentEnemyIndex].Attacking || enemyAIList[currentEnemyIndex].Damaging || enemyAIList[currentEnemyIndex].BackJumping || enemyAIList[currentEnemyIndex].Dying) return;

            // enemyAI[currentEnemyIndex] 행동 명령 전달
            if (updateDelayed || inputBlocked) return;
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

            isInCombat = true;
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
                UIManager.Instance.TitleEnemyHpSwitch(false);
                DataManager.Instance.playerDataManager.StageReset();
                InitAiPos(true);
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