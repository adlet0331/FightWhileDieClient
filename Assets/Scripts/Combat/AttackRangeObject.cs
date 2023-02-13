using System;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class AttackRangeObject : MonoBehaviour
    {
        [Header("Set in Unity")]
        [SerializeField] private CombatEntity combatEntity;
        [SerializeField] private BoxCollider2D attackRangeCollider;
        
        public bool EnemyInRange => enemyInRange;
        
        [SerializeField] private bool enemyInRange;

        private void Start()
        {
            ResetInRange();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
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

        public void ResetInRange()
        {
            enemyInRange = false;
        }
    }
}