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
        public CombatObject player;
        public CombatObject enemyAI;
        [SerializeField] private Transform aiStartPosition;
        [SerializeField] private Transform aiStandingPosition;

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
                enemyAI.EnableAnimation(!timeBlocked);
                // 시간이 멈춤
                if (value)
                {
                    if (_afterDeadCoroutine != null)
                    {
                        StopCoroutine(_afterDeadCoroutine);
                    }
                    player.StopAllCoroutines();
                    enemyAI.StopAllCoroutines();
                }
                else
                {
                    if (_afterDeadCoroutine != null)
                    {
                        StartCoroutine(_afterDeadCoroutine);
                    }
                    player.RestartAllCoroutines();
                    enemyAI.RestartAllCoroutines();
                }
            }
        }

        private Random _random;
        private void Start()
        {
            inputBlocked = true;
            // Coroutines
            _afterDeadCoroutine = null;
            _blockInputCoroutine = null;
            // Random
            randomSeed = DateTime.Now.Millisecond * 103729;
            _random = new Random(randomSeed);
            UpdateRandomSeed();
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
             
            // enemyAI 피격 판정 우선. 
            if (player.Hitting && player.EnemyInRange)
            {
                player.CancelHittingJudgeCoroutine();
                var dead = enemyAI.Damaged(player.Atk);
                if (dead)
                {
                    // Update enemyAI's Random Seed
                    UpdateRandomSeed();
                    // 코인 이펙트
                    UIManager.Instance.ShowCoinEffect();
                    _afterDeadCoroutine = CoroutineUtils.WaitAndOperationIEnum(enemyAI.GetAnimationTime("Dead"), () =>
                    {
                        player.ResetAfterDie();
                        enemyAI.ResetAfterDie();
                        EndCombat(true);
                        _afterDeadCoroutine = null;
                    });
                    StartCoroutine(_afterDeadCoroutine);
                }

                return;
            }

            // Player 피격 판정
            if (enemyAI.Hitting && enemyAI.EnemyInRange)
            {
                enemyAI.CancelHittingJudgeCoroutine();
                var dead = player.Damaged(enemyAI.Atk);
                if (dead)
                {
                    inputBlocked = true;
                    _afterDeadCoroutine = CoroutineUtils.WaitAndOperationIEnum(player.GetAnimationTime("Dead"), () =>
                    {
                        player.ResetAfterDie();
                        enemyAI.ResetAfterDie();
                        EndCombat(false);
                        _afterDeadCoroutine = null;
                    });
                    StartCoroutine(_afterDeadCoroutine);
                }
                else
                    Debug.LogAssertion("Player MUST DIE in first hit!!");
                
                return;
            }
            
            // enemyAI Transform Moving
            if (enemyAI.Damaging)
            {
                enemyAI.transform.position += Vector3.right * (enemyAI.knockBackXInterval * Time.fixedDeltaTime);
            }
            else if (enemyAI.Running)
            {
                enemyAI.transform.position += Vector3.left * (enemyAI.runningSpeed * Time.fixedDeltaTime);
            }
            else if (enemyAI.BackJumping)
            {
                enemyAI.transform.position += Vector3.right * (enemyAI.backJumpSpeed * Time.fixedDeltaTime);
            }

            if (enemyAI.Attacking || enemyAI.Damaging || enemyAI.BackJumping || enemyAI.Dying) return;

            // enemyAI 행동 명령 전달
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
            if (enemyAI.EnemyInRange)
            {
                int rand = _random.Next(1, 100);
                if (rand <= _random.Next(1, 100))
                {
                    enemyAI.Action(ObjectStatus.Attack);
                }
                else
                {
                    enemyAI.Action(ObjectStatus.JumpBack);
                }
                UpdateRandomSeed();
            }
            else
            {
                enemyAI.Action(ObjectStatus.Running);
            }
        }

        private void InitAiPos(bool standing)
        {
            if (standing)
                enemyAI.transform.SetLocalPositionAndRotation(aiStandingPosition.localPosition, aiStandingPosition.rotation);
            else
                enemyAI.transform.SetLocalPositionAndRotation(aiStartPosition.localPosition, aiStartPosition.rotation);
        }
        
        public void StartCombat()
        {
            inputBlocked = false;
            InitAiPos(false);
            UIManager.Instance.TitleEnemyHpSwitch(true);
        }

        private void EndCombat(bool cleared)
        {
            if (cleared)
            {
                // 스테이지 클리어 처리
                SLManager.Instance.StageCleared();
                InitAiPos(false);
            }
            else
            {
                UIManager.Instance.TitleEnemyHpSwitch(false);
                SLManager.Instance.StageReset();
                InitAiPos(true);
            }
        }

        private void UpdateRandomSeed()
        {
            randomSeed = _random.Next();
            _random = new Random(randomSeed);
        }
        
        // IEnumerator
        private IEnumerator _afterDeadCoroutine;
        // Input Block 용
        private IEnumerator _blockInputCoroutine;
        
        private void CancelBlockInputCoroutine()
        {
            if (_blockInputCoroutine != null)
            {
                StopCoroutine(_blockInputCoroutine);
                _blockInputCoroutine = null;
            }
        }
        private void BlockInput(float sec, CoroutineUtils.AfterWaitOperation operation)
        {
            inputBlocked = true;
            
            if (_blockInputCoroutine != null) StopCoroutine(_blockInputCoroutine);
            _blockInputCoroutine = CoroutineUtils.WaitAndOperationIEnum(sec, () =>
            {
                inputBlocked = false;

                _blockInputCoroutine = null;
                operation();
            });
            StartCoroutine(_blockInputCoroutine);
        }

    }
}