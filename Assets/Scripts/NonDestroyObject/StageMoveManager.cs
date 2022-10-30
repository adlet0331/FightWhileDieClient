using System;
using System.Collections;
using Combat;
using Managers;
using UnityEngine;

namespace NonDestroyObject
{
    public class StageMoveManager : Singleton<StageMoveManager>
    {
        [Header("Need To be Initialized")] 
        [SerializeField] private float sceneMoveTime;
        [SerializeField] private float waitBeforeStartTime;
        [SerializeField] private float updateDelayTime;
        [SerializeField] public Transform enemyStartPosition;
        [SerializeField] private Transform enemyStageStartPosition;

        [Header("Debuging")] 
        [SerializeField] private bool moving;
        [SerializeField] private bool updateDelayed;

        private IEnumerator UpdateDelay()
        {
            yield return new WaitForSeconds(updateDelayTime);
            updateDelayed = false;
        }
        
        private void FixedUpdate()
        {
            if (!moving || updateDelayed) return;

            updateDelayed = true;
            StartCoroutine(UpdateDelay());

            if (Math.Abs(enemyStageStartPosition.transform.position.x - transform.position.x) < 0.05)
            {
                moving = false;
                // 움직임 막기
                CombatManager.Instance.updateDelayed = true;
            }
        }

        private IEnumerator WaitUntilNewEnemyIEnum(float second)
        {
            CombatManager.Instance.AI.transform.localPosition = enemyStartPosition.localPosition;
            yield return new WaitForSeconds(second);
            
            moving = true;
        }
        
        private IEnumerator UIUpdateIEnum()
        {
            CombatManager.Instance.AI.transform.localPosition = enemyStartPosition.localPosition;
            yield return new WaitForSeconds(sceneMoveTime);

            CombatManager.Instance.AI.gameObject.SetActive(true);
            CombatManager.Instance.AI.Action(ObjectStatus.Idle);
            moving = true;
        }

        private void HideUI()
        {
            
        }
        
        public void EnemyDead()
        {
            StartCoroutine(WaitUntilNewEnemyIEnum(sceneMoveTime));
        }
    }
}