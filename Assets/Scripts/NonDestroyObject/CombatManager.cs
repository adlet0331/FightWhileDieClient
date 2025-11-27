using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public bool SwitchDirection;
        public bool IsInCombat { get; private set; }
        
        // 현재 스테이지에서 적이 오른쪽에서 오는지 왼쪽에서 오는지 (true: 오른쪽, false: 왼쪽)
        private bool _isEnemyFromRight;
        
        // 현재 활성화된 적
        public EnemyEntity CurrentActiveEnemyEntity
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
                    CurrentActiveEnemyEntity.TimeBlocked();
                }
                else
                {
                    
                    player.TimeResumed();
                    CurrentActiveEnemyEntity.TimeResumed();
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
            UpdateRandomSeed();
            
            // 이전 적 숨기기
            if (currentEnemyIndex < enemyAIList.Length)
                CurrentActiveEnemyEntity.Hide();
            
            // 랜덤하게 오른쪽 또는 왼쪽에서 적이 오도록 결정
            _isEnemyFromRight = _random.Next(0, 2) == 0;
            
            // 새로운 적 인덱스 선택 (짝수: 오른쪽, 홀수: 왼쪽)
            if (_isEnemyFromRight)
            {
                currentEnemyIndex = _random.Next(0, enemyAIList.Length / 2) * 2;
            }
            else
            {
                currentEnemyIndex = _random.Next(0, enemyAIList.Length / 2) * 2 + 1;
            }
            
            CurrentActiveEnemyEntity.Show();
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

            if (SwitchDirection)
            {
                SwitchDirection = false;
                player.transform.localScale = new Vector3(-player.transform.localScale.x, player.transform.localScale.y, player.transform.localScale.z);
            }

            #region CheckHitting
            // enemyAI 피격 판정 우선 - 동시에 때리면 플레이어 판정이 우세
            bool[] hittingEnemys = player.PlayerHittingEnemys();
            for (int i = 0; i < hittingEnemys.Length; i++)
            {
                if (!hittingEnemys[i]) continue;
                SwitchDirection = false;
                player.MyAttackHitted();
                int damage = (int)(DataManager.Instance.playerDataManager.Atk * playerDamagePerAttackRate[(int)player.AttackType]);
                
                EnemyEntity targetEnemy = CurrentActiveEnemyEntity;

                var dead = targetEnemy.Damaged(damage);
                if (dead)
                {
                    // 코인 이펙트
                    UIManager.Instance.ShowCoinEffect();
                    var delay = targetEnemy.GetAnimationTime("Dying");
                    _afterDeadCoroutine = CoroutineUtils.WaitAndOperationIEnum(delay, () =>
                    {
                        if (CurrentActiveEnemyEntity.CurrentStatus == CombatEntityStatus.Dying)
                        {
                            // 스테이지가 깨질 때 랜덤으로 왼쪽/오른쪽 결정
                            UpdateRandomEnemyAI();
                            StartCombat();
                            DataManager.Instance.playerDataManager.StageCleared();
                        }
                        _afterDeadCoroutine = null;
                    });
                    StartCoroutine(_afterDeadCoroutine);
                }
                else
                {
                    ShakeCamera(1, 1, 1f);
                }
                UIManager.Instance.UpdateEnemyHp(targetEnemy.CurrentHpRatio);
            }
            if (hittingEnemys.Any(hitting => hitting))
            {
                return;
            }

            // Player 피격 판정
            if (CurrentActiveEnemyEntity.EnemyHittingPlayers().Any(hitting => hitting))
            {
                CurrentActiveEnemyEntity.MyAttackHitted();
                // Player's Hp is always 1
                var dead = player.Damaged(1);
                if (dead)
                {
                    var delay = player.GetAnimationTime("Dying");
                    _afterDeadCoroutine = CoroutineUtils.WaitAndOperationIEnum(delay, () =>
                    {
                        EndCombat();
                        _afterDeadCoroutine = null;
                    });
                    StartCoroutine(_afterDeadCoroutine);
                }
                return;
            }
            #endregion

            CurrentActiveEnemyEntity.ActionUpdate();
            // Enemy Entity Moving (방향에 따라 이동 방향 결정)
            if (CurrentActiveEnemyEntity.CurrentStatus == CombatEntityStatus.Damaged)
            {
                if (_isEnemyFromRight)
                    CurrentActiveEnemyEntity.transform.position += Vector3.right * (CurrentActiveEnemyEntity.knockBackXInterval * Time.fixedDeltaTime);
                else
                    CurrentActiveEnemyEntity.transform.position -= Vector3.right * (CurrentActiveEnemyEntity.knockBackXInterval * Time.fixedDeltaTime);
            }
            else if (CurrentActiveEnemyEntity.CurrentStatus == CombatEntityStatus.Running)
            {
                if (_isEnemyFromRight)
                    CurrentActiveEnemyEntity.transform.position += Vector3.left * (CurrentActiveEnemyEntity.runningSpeed * Time.fixedDeltaTime);
                else
                    CurrentActiveEnemyEntity.transform.position -= Vector3.left * (CurrentActiveEnemyEntity.runningSpeed * Time.fixedDeltaTime);
            }
            else if (CurrentActiveEnemyEntity.CurrentStatus == CombatEntityStatus.JumpBack)
            {
                if (_isEnemyFromRight)
                    CurrentActiveEnemyEntity.transform.position += Vector3.right * (CurrentActiveEnemyEntity.backJumpSpeed * Time.fixedDeltaTime);
                else
                    CurrentActiveEnemyEntity.transform.position -= Vector3.right * (CurrentActiveEnemyEntity.backJumpSpeed * Time.fixedDeltaTime);
            }
        }

        private void InitAiPos()
        {
            // 항상 StartPosition 사용
            if (_isEnemyFromRight)
            {
                CurrentActiveEnemyEntity.transform.SetLocalPositionAndRotation(aiRightStartPosition.localPosition, aiRightStartPosition.rotation);
            }
            else
            {
                CurrentActiveEnemyEntity.transform.SetLocalPositionAndRotation(aiLeftStartPosition.localPosition, aiLeftStartPosition.rotation);
            }
        }

        private void InitPlayerEnemyHp()
        {
            player.InitHp(1);
            CurrentActiveEnemyEntity.InitHp(DataManager.Instance.playerDataManager.CurrentEnemyHp);
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
            player.transform.localScale = new Vector3(Mathf.Abs(player.transform.localScale.x), player.transform.localScale.y, player.transform.localScale.z);

            IsInCombat = true;
            inputBlocked = false;
            InitAiPos();
            UIManager.Instance.SwitchMainPage2CombatUI(true);

            // Start stage transition: player runs, background moves, then enemy spawns
            if (_stageTransitionCoroutine != null)
                StopCoroutine(_stageTransitionCoroutine);
            _stageTransitionCoroutine = StageTransition();
            StartCoroutine(_stageTransitionCoroutine);
            
            // Start background movement when combat begins
            BackgroundManager.Instance.Play();
        }

        private void EndCombat()
        {
            IsInCombat = false;
            inputBlocked = true;
            UIManager.Instance.SwitchMainPage2CombatUI(false);
            DataManager.Instance.playerDataManager.StageReset();
            InitAiPos();  // 항상 StartPosition 사용
            BackgroundManager.Instance.Stop();
            InitPlayerEnemyHp();
            CurrentActiveEnemyEntity.WhenStart();
            player.transform.localScale = new Vector3(Mathf.Abs(player.transform.localScale.x), player.transform.localScale.y, player.transform.localScale.z);
        }
        
        private IEnumerator StageTransition()
        {
            // Play player running animation
            player.EntityAction(CombatEntityStatus.Running);
            InitAiPos();
            CurrentActiveEnemyEntity.WhenStart();
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