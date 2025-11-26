using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Combat
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class AttackRangeObject : MonoBehaviour
    {
        [Header("Set in Unity")]
        [SerializeField] private BoxCollider2D attackRangeCollider;
        [SerializeField] private bool isPlayer;
        private string[] opponentTagNames;
        public bool[] OpponentInRanges => opponentInRanges;
        [SerializeField] private bool[] opponentInRanges;

        public void WhenShow()
        {
           
        }

        public void WhenHide()
        {
            ResetInRange();
        }
                
        public void ResetInRange()
        {
            for (int i = 0; i < opponentInRanges.Length; i++)
            {
                opponentInRanges[i] = false;
            }
        }

        private void Awake()
        {
            if (isPlayer)
            {
                opponentTagNames = new string[] { "EnemyL", "EnemyR" };
            }
            else
            {
                opponentTagNames = new string[] { "Player" };
            }
            opponentInRanges = new bool[opponentTagNames.Length];
        }

        private void Start()
        {
            ResetInRange();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            for (int i = 0; i < opponentTagNames.Length; i++)
            {
                if (col.CompareTag(opponentTagNames[i]))
                {
                    opponentInRanges[i] = true;
                }
            }
        }
        private void OnTriggerExit2D(Collider2D col)
        {
            for (int i = 0; i < opponentTagNames.Length; i++)
            {
                if (col.CompareTag(opponentTagNames[i]))
                {
                    opponentInRanges[i] = false;
                }
            }
        }
    }
}