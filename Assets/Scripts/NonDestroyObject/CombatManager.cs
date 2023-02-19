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
        [SerializeField] private int currentEnemyIndex;
        [Header("Status")]
        [SerializeField] private bool inputBlocked;
        [SerializeField] private bool timeBlocked;
        [SerializeField] private bool updateDelayed;
        [SerializeField] private float updateInterval;
        [SerializeField] private int randomSeed;
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
            #region InputParticle
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    var touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    touchPosition.z = 0;
                    var clickParticle = Resources.Load("Prefabs/UI/TouchParticle") as GameObject;
                    Instantiate(clickParticle, touchPosition, new Quaternion(0, 0, 0, 0));
                }
            }

            if (Input.GetMouseButton(0))
            {
                var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;
                var clickParticle = Resources.Load("Prefabs/UI/TouchParticle") as GameObject;
                Instantiate(clickParticle, mousePosition, new Quaternion(0, 0, 0, 0));
            }
            #endregion

            if (AutoManager.Instance.IsAuto)
            {
                // Skip Delay At First Pressed
                if (AutoManager.Instance.IsFirst)
                {
                    afterEndCombat = 3.0f;
                }
                
                if (afterEndCombat < 3.0f)
                {
                    if (!timeBlocked)
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
            _delayedCoroutine = null;
        }

        private void FixedUpdate()
        {
            if (timeBlocked) return;

            #region CheckHitting
            // enemyAI 피격 판정 우선 - 동시에 때리면 플레이어 판정이 우세 
            if (player.Hitting && player.EnemyInRange)
            {
                player.CancelHittingJudgeCoroutine();
                enemyAIList[currentEnemyIndex].CancelHittingJudgeCoroutine();
                
                var dead = enemyAIList[currentEnemyIndex].Damaged(DataManager.Instance.playerDataManager.Atk);
                if (dead)
                {
                    // 코인 이펙트
                    UIManager.Instance.ShowCoinEffect();
                    var delay = enemyAIList[currentEnemyIndex].GetAnimationTime("Dead");
                    _afterDeadCoroutine = CoroutineUtils.WaitAndOperationIEnum(delay, () =>
                    {
                        EndCombat(true);
                        player.ResetAfterDie();
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