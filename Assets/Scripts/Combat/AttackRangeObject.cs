using System;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class AttackRangeObject : MonoBehaviour
    {
        [SerializeField] private CombatObject combatObject;
        [SerializeField] private BoxCollider2D attackRangeCollider;
        
        public bool EnemyInRange => enemyInRange;
        
        [SerializeField] private bool enemyInRange;
        private void Start()
        {
            attackRangeCollider = GetComponent<BoxCollider2D>();
            combatObject = GetComponentInParent<CombatObject>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                // Debug.Log("Trigger In");
                enemyInRange = true;
            }
        }
        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                // Debug.Log("Trigger Out");
                enemyInRange = false;
            }
        }

        public void ResetInRange()
        {
            enemyInRange = false;
        }
    }
}