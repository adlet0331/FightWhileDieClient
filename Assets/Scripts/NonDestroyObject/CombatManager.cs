using System;
using System.Collections;
using Combat;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

namespace NonDestroyObject
{
    public class CombatManager : Singleton<CombatManager>
    {
        [Header("Need Initialize In Unity")]
        public CombatObject AI;

        [Header("Debuging")]
        [SerializeField] public bool updateDelayed;
        [SerializeField] public float updateInterval;
        [SerializeField] private int randomSeed;

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
            if (updateDelayed) return;
            if (AI.Attacking || AI.Damaging || AI.BackJumping || AI.Running) return;
            
            updateDelayed = true;

            if (AI.EnemyInRange)
            {
                int rand = Random.Next(1, 3);
                if (rand == 2)
                {
                    AI.Action(ObjectStatus.Attack);
                    _delayedCoroutine = UpdateDelay(AI.GetAnimationTime("Attack"));
                }
                else
                {
                    AI.Action(ObjectStatus.JumpBack);
                    _delayedCoroutine = UpdateDelay(AI.GetAnimationTime("JumpBack"));
                }
            }

            else
            {
                AI.Action(ObjectStatus.Running);
                _delayedCoroutine = UpdateDelay(updateInterval);
            }
            StartCoroutine(_delayedCoroutine);
            UpdateRandomSeed();
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
            StageMoveManager.Instance.EnemyDead();
            //AI.gameObject.SetActive(false);
        }
    }
}