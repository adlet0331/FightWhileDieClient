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
        [SerializeField] private PlayerEntity player;
        [SerializeField] private EnemyEntity[] enemyAIList;
        [SerializeField] private Transform aiRightStartPosition;
        [SerializeField] private Transform aiRightStandingPosition;
        [SerializeField] private Transform aiLeftStartPosition;
        [SerializeField] private Transform aiLeftStandingPosition;
        [SerializeField] private int currentEnemyIndex;
        [SerializeField] private float strongAttackHoldTime;
        [SerializeField] private float strongAttackHoldRandomMaxTime;
        [SerializeField] private float perfectAttackTimeInterval;
        [SerializeField] private Slider holdSliderUI;
        [SerializeField] private Camera MainCamera;
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
                ActivateCoroutines(value);
                if (value)
                {
                    player.TimeBlocked();
                    CurrentEnemyEntity.TimeBlocked();
                }
                else
                {
                    
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
            _shakeCameraCoroutine = null;
            _stageTransitionCoroutine = null;
            // Random
            randomSeed = DateTime.Now.Millisecond * 103729;
            _random = new Random(randomSeed);
            UpdateRandomEnemyAI();
            UpdateRandomSeed();
        }

        private void ActivateCoroutines(bool active)
        {
            if (active)
            {
                if (_afterDeadCoroutine != null)
                {
                    StopCoroutine(_afterDeadCoroutine);
                }
                if (_shakeCameraCoroutine != null)
                {
                    StopCoroutine(_shakeCameraCoroutine);
                }
                if (_stageTransitionCoroutine != null)
                {
                    StopCoroutine(_stageTransitionCoroutine);
                }
            }
            else
            {
                if (_afterDeadCoroutine != null)
                {
                    StartCoroutine(_afterDeadCoroutine);
                }
                if (_shakeCameraCoroutine != null)
                {
                    StartCoroutine(_shakeCameraCoroutine);
                }
                if (_stageTransitionCoroutine != null)
                {
                    StartCoroutine(_stageTransitionCoroutine);
                }
            }
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
            
            // 공격 액션
            if (!inputBlocked && !player.Attacking && player.CurrentStatus != CombatEntityStatus.Dying)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    holdTime = 0;
                    player.EntityAction(CombatEntityStatus.Charge);
                    isHolded = true;
                    UIManager.Instance.UpdateAttackGageSlider(holdTime);
                }
                if (Input.GetMouseButton(0) && isHolded)
                {
                    holdTime += Time.deltaTime * 2; //TODO: 캐릭터 스텟에 따른 변화
                    UIManager.Instance.UpdateAttackGageSlider(Math.Min(holdTime, 1.0f));
                    if (strongAttackHoldTime - perfectAttackTimeInterval <= holdTime && holdTime <= strongAttackHoldTime + perfectAttackTimeInterval)
                    {
                        UIManager.Instance.ActivePerfectEffect(true);
                    }
                    else
                    {
                        UIManager.Instance.ActivePerfectEffect(false);
                    }
                }
                if (isHolded && holdTime > 0 && Input.GetMouseButtonUp(0))
                {
                    UIManager.Instance.UpdateAttackGageSlider(0);
                    if (holdTime < strongAttackHoldTime)
                    {
                        player.EntityAction(CombatEntityStatus.Attack);
                    }
                    else if (strongAttackHoldTime - perfectAttackTimeInterval <= holdTime && holdTime <= strongAttackHoldTime + perfectAttackTimeInterval)
                    {
                        player.EntityAction(CombatEntityStatus.PerfectChargeAttack);
                    }
                    else
                    {
                        player.EntityAction(CombatEntityStatus.ChargeAttack);
                    }
                    UIManager.Instance.ActivePerfectEffect(false);
                    isHolded = false;
                }
            }

            BackgroundManager.Instance.UpdateBackground(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (timeBlocked || inputBlocked) return;

            #region CheckHitting
            // enemyAI 피격 판정 우선 - 동시에 때리면 플레이어 판정이 우세 
            if (player.PlayerHittingEnemy)
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
                else
                {
                    ShakeCamera(1, 1, 1f);
                }
                UIManager.Instance.UpdateEnemyHp(CurrentEnemyEntity.CurrentHpRatio);
                return;
            }

            // Player 피격 판정
            if (CurrentEnemyEntity.IsEnemyHittingPlayer)
            {
                CurrentEnemyEntity.MyAttackHitted();
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

            CurrentEnemyEntity.ActionUpdate();
            // Enemy Entity Moving
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
        }

        private void InitAiPos(bool playerDie)
        {
            if (playerDie)
            {
                CurrentEnemyEntity.transform.SetLocalPositionAndRotation(aiRightStandingPosition.localPosition, aiRightStandingPosition.rotation);
                // CurrentEnemyEntity.transform.SetLocalPositionAndRotation(aiLeftStandingPosition.localPosition, aiLeftStandingPosition.rotation);
            }
            else
            {
                CurrentEnemyEntity.transform.SetLocalPositionAndRotation(aiRightStartPosition.localPosition, aiRightStartPosition.rotation);
                // CurrentEnemyEntity.transform.SetLocalPositionAndRotation(aiLeftStartPosition.localPosition, aiLeftStartPosition.rotation);
            }
        }

        private void InitPlayerEnemyHp()
        {
            player.InitHp(1);
            CurrentEnemyEntity.InitHp(DataManager.Instance.playerDataManager.CurrentEnemyHp);
        }

        private IEnumerator CameraShake(float roughness, float magnitude, float duration)
        {
            float halfDuration = duration / 2;
            float elapsed = 0f;
            float tick = _random.Next(-10, 10);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime / halfDuration;

                tick += Time.deltaTime * roughness;
                MainCamera.transform.position = new Vector3(
                    Mathf.PerlinNoise(tick, 0) - .5f,
                    Mathf.PerlinNoise(0, tick) - .5f,
                    0) * magnitude * Mathf.PingPong(elapsed, halfDuration) + new Vector3(0, 0, -10);

                yield return null;
            }
            _shakeCameraCoroutine = null;
        }
        
        private void ShakeCamera(float roughness, float magnitude, float duration)
        {
            _shakeCameraCoroutine = CameraShake(roughness, magnitude, duration);
            StartCoroutine(_shakeCameraCoroutine);
        }
        
        public void StartCombat()
        {
            DataManager.Instance.playerDataManager.StageReset();
            InitPlayerEnemyHp();

            IsInCombat = true;
            inputBlocked = false;
            InitAiPos(false);
            UIManager.Instance.SwitchMainPage2CombatUI(true);

            // Start stage transition: player runs, background moves, then enemy spawns
            if (_stageTransitionCoroutine != null)
                StopCoroutine(_stageTransitionCoroutine);
            _stageTransitionCoroutine = StageTransition();
            StartCoroutine(_stageTransitionCoroutine);
            
            // Start background movement when combat begins
            BackgroundManager.Instance.Play();
        }

        private void EndCombat(bool cleared)
        {
            if (cleared)
            {
                // 스테이지 클리어 처리
                UpdateRandomEnemyAI();
                DataManager.Instance.playerDataManager.StageCleared();
                
                // Start stage transition: player runs, background moves, then enemy spawns
                if (_stageTransitionCoroutine != null)
                    StopCoroutine(_stageTransitionCoroutine);
                _stageTransitionCoroutine = StageTransition();
                StartCoroutine(_stageTransitionCoroutine);
            }
            else
            {
                IsInCombat = false;
                inputBlocked = true;
                UIManager.Instance.SwitchMainPage2CombatUI(false);
                DataManager.Instance.playerDataManager.StageReset();
                InitAiPos(true);
                BackgroundManager.Instance.Stop();
                InitPlayerEnemyHp();
            }
        }
        
        private IEnumerator StageTransition()
        {
            // Play player running animation
            player.EntityAction(CombatEntityStatus.Running);
            InitAiPos(false);
            BackgroundManager.Instance.Play();
            
            // Wait 1 second for transition
            yield return new WaitForSeconds(1.0f);
            
            // Stop player running and return to idle
            if (player.CurrentStatus == CombatEntityStatus.Running)
                player.EntityAction(CombatEntityStatus.Idle);
            BackgroundManager.Instance.Stop();
            
            // Initialize HP for new stage
            InitPlayerEnemyHp();
            
            _stageTransitionCoroutine = null;
        }

        private void UpdateRandomSeed()
        {
            randomSeed = _random.Next();
            _random = new Random(randomSeed);
        }
        
        // IEnumerator
        private IEnumerator _afterDeadCoroutine;
        private IEnumerator _shakeCameraCoroutine;
        private IEnumerator _stageTransitionCoroutine;
    }
}