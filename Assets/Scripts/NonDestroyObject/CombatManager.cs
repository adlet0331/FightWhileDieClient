using System;
using System.Collections;
using Combat;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Random = System.Random;

namespace NonDestroyObject
{
    public class CombatManager : Singleton<CombatManager>
    {
        [Header("Need Initialize In Unity")]
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

        private IEnumerator _delayedCoroutine;
        private IEnumerator UpdateDelay(float second)
        {
            yield return new WaitForSeconds(second);
            updateDelayed = false;
        }

        private void FixedUpdate()
        {
            if (updateDelayed || blocked) return;
            if (AI.Attacking || AI.Damaging || AI.BackJumping || AI.Running || AI.Dying) return;
            
            updateDelayed = true;

            if (AI.EnemyInRange)
            {
                int rand = Random.Next(1, 3);
                if (rand == 2)
                {
                    AI.Action(ObjectStatus.Attack);
                    _delayedCoroutine = CoroutineUtils.WaitAndOperationIEnum(AI.GetAnimationTime("Attack"), () =>
                    {
                        updateDelayed = false;
                    });
                }
                else
                {
                    AI.Action(ObjectStatus.JumpBack);
                    _delayedCoroutine = CoroutineUtils.WaitAndOperationIEnum(AI.GetAnimationTime("JumpBack"), () =>
                    {
                        updateDelayed = false;
                    });
                }
                UpdateRandomSeed();
            }

            else
            {
                AI.Action(ObjectStatus.Running);
                _delayedCoroutine = CoroutineUtils.WaitAndOperationIEnum(updateInterval, () =>
                {
                    updateDelayed = false;
                });
            }
            StartCoroutine(_delayedCoroutine);
        }
        
        private void UpdateRandomSeed()
        {
            randomSeed = Random.Next();
            Random = new Random(randomSeed);
        }

        public void AIDie()
        {
            UpdateRandomSeed();
            SLManager.Instance.StageCleared();
            StageMoveManager.Instance.StopCombat(true);
        }
    }
}