using System;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class AttackRangeObject : MonoBehaviour
    {
        [Header("Set in Unity")]
        [SerializeField] private BoxCollider2D attackRangeCollider;
        [SerializeField] private string opponentTagName;
        
        public bool OpponentInRange => opponentInRange;
        
        [SerializeField] private bool opponentInRange;

        public void WhenShow()
        {
           
        }

        public void WhenHide()
        {
            ResetInRange();
        }
                
        public void ResetInRange()
        {
            opponentInRange = false;
        }

        private void Start()
        {
            ResetInRange();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag(opponentTagName))
            {
                opponentInRange = true;
            }
        }
        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag(opponentTagName))
            {
                opponentInRange = false;
            }
        }
    }
}