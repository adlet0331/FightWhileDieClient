using System;
using System.Collections;
using Combat;
using UnityEngine;
using Utils;

namespace NonDestroyObject
{
    public class StageMoveManager : Singleton<StageMoveManager>
    {
        public float UIMovingTime => uiMovingTime;
        
        [Header("Need To be Initialized")]
        [SerializeField] private float uiMovingTime;
        [SerializeField] private float waitBeforeStartTime;
        [SerializeField] private float updateDelayTime;
        [SerializeField] public Transform enemyStartPosition;
        [SerializeField] private Transform enemyStageStartPosition;
        [SerializeField] private Transform titleShowPosition;
        [SerializeField] private Transform titleHidePosition;

        [Header("Debuging")] 
        [SerializeField] private bool moving;
        [SerializeField] private bool updateDelayed;

        private void Start()
        {
            UIManager.Instance.titleTransform.position = titleShowPosition.position;
            UIManager.Instance.stageHpTransform.position = titleHidePosition.position;
        }

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

            if (Math.Abs(enemyStageStartPosition.transform.position.x - transform.position.x) < 0.1)
            {
                moving = false;
                // 움직임 막기
                CombatManager.Instance.updateDelayed = true;
            }
        }

        private IEnumerator TransformMove(float time, Transform source, Transform target)
        {
            int count = (int)(time / (Time.deltaTime * 2));
            Vector3 interval = (target.position - source.position) / count;
            for (int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(Time.deltaTime * 2);
                source.position += interval;
                
            }
        }

        private void SpawnCombatAIAndWait()
        {
            float time = (enemyStartPosition.position.x - enemyStageStartPosition.position.x) /
                         CombatManager.Instance.AI.RunningSpeed;
            // 지정 위치까지 와서 멈춰있기
            StartCoroutine(CoroutineUtils.WaitAndOperationIEnum(time, () =>
            {
                PlayerCombatManager.Instance.Player.Action(ObjectStatus.Idle);
                // 움직임 막고 IDLE로 만들기
                CombatManager.Instance.Blocked = true;
                CombatManager.Instance.AI.Action(ObjectStatus.Idle);
            }));
            // Blocked 해제
            StartCoroutine(CoroutineUtils.WaitAndOperationIEnum((time > uiMovingTime ? time : uiMovingTime) + waitBeforeStartTime, () =>
            {
                InputManager.Instance.Blocked = false;
                CombatManager.Instance.Blocked = false;
            }));
            CombatManager.Instance.AI.Action(ObjectStatus.Idle);
            CombatManager.Instance.AI.transform.localPosition = enemyStartPosition.localPosition;
            CombatManager.Instance.Blocked = false;
            PlayerCombatManager.Instance.Player.Action(ObjectStatus.Running);
        }

        public void StopCombat(bool startNextStage)
        {
            // Block Input
            InputManager.Instance.Blocked = true;
            CombatManager.Instance.Blocked = true;
            
            CombatManager.Instance.AI.Action(ObjectStatus.Idle);
            PlayerCombatManager.Instance.Player.Action(ObjectStatus.Idle);
            
            if (startNextStage)
            {
                SpawnCombatAIAndWait();
            }
            else
            {
                CombatManager.Instance.AI.transform.localPosition = enemyStageStartPosition.localPosition;
                StartCoroutine(TransformMove(uiMovingTime, UIManager.Instance.stageHpTransform, titleHidePosition));
                StartCoroutine(TransformMove(uiMovingTime, UIManager.Instance.titleTransform, titleShowPosition));
                UIManager.Instance.ShowHideButtons(true);
            }
        }

        public void StartCombat()
        {
            SpawnCombatAIAndWait();
            
            StartCoroutine(TransformMove(uiMovingTime, UIManager.Instance.stageHpTransform, titleShowPosition));
            StartCoroutine(TransformMove(uiMovingTime, UIManager.Instance.titleTransform, titleHidePosition));
            UIManager.Instance.ShowHideButtons(false);
        }
    }
}