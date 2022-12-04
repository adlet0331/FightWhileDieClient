using System;
using System.Collections;
using Combat;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Random = System.Random;

namespace NonDestroyObject
{
    public class CombatManager : Singleton<CombatManager>
    {
        [Header("Need Initialize In Unity")] 
        public CombatObject Player;
        public CombatObject AI;

        [Header("Debuging")]
        [SerializeField] private bool blocked;
        [SerializeField] public bool updateDelayed;
        [SerializeField] public float updateInterval;
        [SerializeField] private int randomSeed;

        public bool Blocked
        {
            get => blocked;
            set => blocked = value;
        }
        
        private Random Random;
        private void Start()
        {
            randomSeed = DateTime.Now.Millisecond * 103729;
            Random = new Random(randomSeed);
            UpdateRandomSeed();
        }
        
        // Input
        private void Update()
        {
            // Attack 중일 때는 막기
            if (Blocked || Player.Attacking) return;
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Player.Action(ObjectStatus.Attack);
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                Player.Action(ObjectStatus.Attack);
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
            if (updateDelayed || blocked) return;
            if (Player.Dying || Player.Damaging) return;
            if (AI.Attacking || AI.Damaging || AI.BackJumping || AI.Running || AI.Dying) return;
            
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
            if (AI.EnemyInRange)
            {
                int rand = Random.Next(1, 3);
                if (rand == 2)
                {
                    AI.Action(ObjectStatus.Attack);
                }
                else
                {
                    AI.Action(ObjectStatus.JumpBack);
                }
                UpdateRandomSeed();
            }
            else
            {
                AI.Action(ObjectStatus.Running);
            }
        }

        public void PlayerAction()
        {
            
        }

        public void PlayerDie()
        {
            // Range 처리 초기화
            Player.ResetAfterDie();
            CombatManager.Instance.AI.ResetAfterDie();
            
            // 스테이지 리셋
            SLManager.Instance.StageReset();
            // 다음 스테이지 X, Combat End
            StageMoveManager.Instance.StopCombat(false);
        }
        
        public void AIDie()
        {
            UpdateRandomSeed();
            UIManager.Instance.ShowCoinEffect();
            SLManager.Instance.StageCleared();
            StageMoveManager.Instance.StopCombat(true);
        }
        
        private void UpdateRandomSeed()
        {
            randomSeed = Random.Next();
            Random = new Random(randomSeed);
        }

    }
}