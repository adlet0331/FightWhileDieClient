using System;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class AttackRangeObject : MonoBehaviour
    {
        [Header("Set in Unity")]
        [SerializeField] private PlayerEntity playerEntity;
        [SerializeField] private BoxCollider2D attackRangeCollider;
        [SerializeField] private bool initiallized;
        
        public bool EnemyInRange => enemyInRange;
        
        [SerializeField] private bool enemyInRange;

        public void WhenShow()
        {
            initiallized = false;
        }

        public void WhenHide()
        {
            ResetInRange();
        }
                
        public void ResetInRange()
        {
            enemyInRange = false;
        }

        private void Start()
        {
            initiallized = false;
            ResetInRange();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!initiallized)
            {
                initiallized = true;
                return;
            }
            if (col.CompareTag("Player"))
            {
                enemyInRange = true;
            }
        }
        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                enemyInRange = false;
            }
        }
    }
}