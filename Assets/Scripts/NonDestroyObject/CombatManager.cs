using System;
using System.Collections;
using Combat;
using Managers;
using UnityEngine;
using Random = System.Random;

namespace NonDestroyObject
{
    public class CombatManager : Singleton<CombatManager>
    {
        public CombatObject AI;

        [SerializeField] private int goFrontPercentage;
        
        [SerializeField] private bool updateDelayed;
        [SerializeField] private float updateInterval;
        [SerializeField] private int randomSeed;

        private Random Random;
        
        private IEnumerator UpdateDelay(float second)
        {
            updateDelayed = true;
            yield return new WaitForSeconds(second);
            updateDelayed = false;
        }

        public void UpdateRandomSeed()
        {
            randomSeed = DateTime.Now.Millisecond;
            randomSeed *= DateTime.UtcNow.DayOfYear * 7 + DateTime.UtcNow.Minute * 3;
            Random = new Random(randomSeed);
        }

        private void FixedUpdate()
        {
            if (updateDelayed) return;
            if (AI.Attacking || AI.Damaging) return;
            
            StartCoroutine(UpdateDelay(updateInterval));
            
            if (AI.EnemyInRange)
            {
                if (Random.Next(1, 2) == 1)
                {
                    AI.Action(ObjectStatus.Attack);
                }
                else
                {
                    AI.Action(ObjectStatus.JumpBack);
                }
            }

            else
            {
                if (Random.Next(1, 100) > goFrontPercentage)
                {
                    AI.Action(ObjectStatus.Running);
                }
                else
                {
                    AI.Action(ObjectStatus.JumpBack);
                }
            }
        }
    }
}